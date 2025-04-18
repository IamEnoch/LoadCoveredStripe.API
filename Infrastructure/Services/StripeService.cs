using System.Net;
using LoadCoveredStripe.API.API.Utilities;
using LoadCoveredStripe.API.Application.Enums;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Domain.Entities;
using LoadCoveredStripe.API.Infrastructure.Interfaces;
using LoadCoveredStripe.API.Models;
using Microsoft.Extensions.Options;
using Stripe;

namespace LoadCoveredStripe.API.Infrastructure.Services;

/// <summary>
/// Service implementation for Stripe API operations
/// </summary>
public class StripeService(
    ICustomerRepository customerRepository,
    ICustomerBillingRepository customerBillingRepository,
    IPriceCatalogRepository priceCatalogRepository,
    IOptions<StripeSettings> stripeSettings)
    : IStripeService
{
    private readonly CustomerService _customerService = new();
    private readonly SubscriptionService _subscriptionService = new();
    private readonly SetupIntentService _setupIntentService = new();
    private readonly PriceService _priceService = new();
    private readonly EphemeralKeyService _ephemeralKeyService = new();
    private readonly string _webhookSecret = stripeSettings.Value.WebhookSecret;

    /// <inheritdoc />
    public async Task<ServiceResult<(string clientSecret, string ephemeralKey)>> CreateSetupIntentAsync(string stripeCustomerId)
    {
        try
        {
            // Create a SetupIntent
            var setupIntentOptions = new SetupIntentCreateOptions
            {
                Customer = stripeCustomerId,
                PaymentMethodTypes = ["card"]
            };
            
            var setupIntent = await _setupIntentService.CreateAsync(setupIntentOptions);
            
            // Create an ephemeral key for the customer
            var ephemeralKeyOptions = new EphemeralKeyCreateOptions
            {
                Customer = stripeCustomerId,
                StripeVersion = "2022-11-15" // Use the current Stripe API version
            };
            
            var ephemeralKey = await _ephemeralKeyService.CreateAsync(ephemeralKeyOptions);
            
            var result = (setupIntent.ClientSecret, ephemeralKey.Secret);
            
            return ServiceResult<(string clientSecret, string ephemeralKey)>.Success(
                result, 
                "Setup intent created successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<(string clientSecret, string ephemeralKey)>.Fail(
                $"Error creating setup intent: {ex.Message}", 
                default);
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<string>> CreateSubscriptionAsync(int customerId, string priceId)
    {
        try
        {
            // Ensure customer exists in Stripe
            var customerResult = await EnsureStripeCustomerExistsAsync(customerId);
            if (customerResult == null)
            {
                return ServiceResult<string>.Fail(
                    "Failed to get Stripe customer ID. Customer may not exist. Try creating a customer first.",
                    null,
                    ServiceError.NotFound);
            }
            
            // Check if the price exists in our database. If not try to sync it and after is still not esisting return an error
            var priceExists = await priceCatalogRepository.ExistsByPriceIdAsync(priceId);
            if (!priceExists)
            {
                var syncResult = await SyncPricesAsync();
                if (!syncResult.IsSuccess)
                {
                    return ServiceResult<string>.Fail(
                        "Failed to sync prices from Stripe. Please try again later.",
                        null,
                        ServiceError.Other);
                }
                
                priceExists = await priceCatalogRepository.ExistsByPriceIdAsync(priceId);
                if (!priceExists)
                {
                    return ServiceResult<string>.Fail(
                        "Price not found in our database. Please check the price ID.",
                        null,
                        ServiceError.NotFound);
                }
            }

            // Get the price from repository to check for trial period
            var price = await priceCatalogRepository.GetByPriceIdAsync(priceId);
            
            // Create subscription with payment_behavior = default_incomplete
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customerResult,
                Items =
                [
                    new SubscriptionItemOptions
                    {
                        Price = priceId
                    }
                ],
                PaymentBehavior = "default_incomplete",
                PaymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    SaveDefaultPaymentMethod = "on_subscription"
                },
                TrialEnd = price?.TrialDays > 0 ? 
                    DateTime.UtcNow.AddDays(price.TrialDays.Value) : 
                    null
            };
            
            var subscription = await _subscriptionService.CreateAsync(subscriptionOptions);
            
            // Check if a billing record already exists for this customer
            var billingExists = await customerBillingRepository.ExistsAsync(b => b.CustomerId == customerId);
            if (billingExists)
            {
                // Update existing billing record
                var billing = await customerBillingRepository.GetByCustomerIdAsync(customerId);
                billing.StripeSubscriptionId = subscription.Id;
                billing.PriceId = priceId;
                billing.Status = subscription.Status;
                billing.CurrentPeriodStart = subscription.Items.Data[0].CurrentPeriodStart;
                billing.CurrentPeriodEnd = subscription.Items.Data[0].CurrentPeriodEnd;
                billing.CancelAt = subscription.CancelAt;
                billing.UpdatedAt = DateTime.UtcNow;
                
                await customerBillingRepository.UpdateAsync(billing);
                await customerBillingRepository.SaveChangesAsync();
                
                return ServiceResult<string>.Success(
                    subscription.Id, 
                    "Subscription created successfully",
                    HttpStatusCode.Created);
            }
            
            // If no existing billing record exists, create a new one
            var newBilling = new CustomerBilling
            {
                CustomerId = customerId,
                StripeCustomerId = customerResult,
                StripeSubscriptionId = subscription.Id,
                PriceId = priceId,
                Status = subscription.Status,
                CurrentPeriodStart = subscription.Items.Data[0].CurrentPeriodStart,
                CurrentPeriodEnd = subscription.Items.Data[0].CurrentPeriodEnd,
                CancelAt = subscription.CancelAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await customerBillingRepository.AddAsync(newBilling);
            await customerBillingRepository.SaveChangesAsync();
            return ServiceResult<string>.Success(
                subscription.Id, 
                "Subscription created successfully",
                HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail(
                $"Error creating subscription: {ex.Message}", 
                null, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> SwapPlanAsync(string subscriptionId, string newPriceId)
    {
        try
        {
            // Check if the subscription exists in our database.
            var subExists = await customerBillingRepository.ExistsAsync(b => b.StripeSubscriptionId == subscriptionId);
            if (!subExists)
            {
                return ServiceResult<bool>.Fail(
                    $"Subscription with ID {subscriptionId} not found", 
                    false, 
                    ServiceError.NotFound);
            }
            
            // Check if the new price id is a valid on from our database. If not try syncing the db then check again
            var priceExists = await priceCatalogRepository.ExistsByPriceIdAsync(newPriceId);
            if (!priceExists)
            {
                var syncResult = await SyncPricesAsync();
                if (!syncResult.IsSuccess)
                {
                    return ServiceResult<bool>.Fail(
                        "Failed to sync prices from Stripe. Please try again later.",
                        false,
                        ServiceError.Other);
                }
                
                priceExists = await priceCatalogRepository.ExistsByPriceIdAsync(newPriceId);
                if (!priceExists)
                {
                    return ServiceResult<bool>.Fail(
                        "New price not found in our database. Please check the price ID.",
                        false,
                        ServiceError.NotFound);
                }
            }
            
            var subscription = await _subscriptionService.GetAsync(subscriptionId);
            var subscriptionItemId = subscription.Items.Data[0].Id;
            
            var options = new SubscriptionItemUpdateOptions
            {
                Price = newPriceId
            };
            
            await new SubscriptionItemService().UpdateAsync(subscriptionItemId, options);
            
            // Update customer billing
            var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscriptionId);

            billing.PriceId = newPriceId;
            billing.UpdatedAt = DateTime.UtcNow;
                
            await customerBillingRepository.UpdateAsync(billing);
            await customerBillingRepository.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Plan swapped successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error swapping plan: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> PauseAsync(string subscriptionId)
    {
        try
        {
            var options = new SubscriptionUpdateOptions
            {
                PauseCollection = new SubscriptionPauseCollectionOptions
                {
                    Behavior = "keep_as_draft"
                }
            };
            
            await _subscriptionService.UpdateAsync(subscriptionId, options);
            
            // Update customer billing (actual data will be updated via webhook)
            var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscriptionId);
                
            if (billing != null)
            {
                billing.PausedFrom = DateTime.UtcNow;
                billing.UpdatedAt = DateTime.UtcNow;
                
                await customerBillingRepository.UpdateAsync(billing);
                await customerBillingRepository.SaveChangesAsync();
            }
            
            return ServiceResult<bool>.Success(true, "Subscription paused successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error pausing subscription: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> CancelAsync(string subscriptionId, bool immediate)
    {
        try
        {
            if (immediate)
            {
                // Cancel immediately
                await _subscriptionService.CancelAsync(subscriptionId);
            }
            else
            {
                // Cancel at period end
                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
                };
                
                await _subscriptionService.UpdateAsync(subscriptionId, options);
                
                // Update customer billing
                var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscriptionId);
                    
                if (billing != null)
                {
                    var subscription = await _subscriptionService.GetAsync(subscriptionId);
                    billing.CancelAt = subscription.Items.Data[0].CurrentPeriodEnd;
                    billing.UpdatedAt = DateTime.UtcNow;
                    
                    await customerBillingRepository.UpdateAsync(billing);
                    await customerBillingRepository.SaveChangesAsync();
                }
            }
            
            return ServiceResult<bool>.Success(
                true, 
                immediate ? "Subscription canceled immediately" : "Subscription will be canceled at period end");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error canceling subscription: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> ReactivateAsync(string subscriptionId)
    {
        try
        {
            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                PauseCollection = null // This will resume a paused subscription
            };
            
            await _subscriptionService.UpdateAsync(subscriptionId, options);
            
            // Update customer billing
            var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscriptionId);

            billing.CancelAt = null;
            billing.PausedFrom = null;
            billing.PausedUntil = null;
            billing.UpdatedAt = DateTime.UtcNow;
                
            await customerBillingRepository.UpdateAsync(billing);
            await customerBillingRepository.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Subscription reactivated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error reactivating subscription: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> UpdateDefaultPaymentMethodAsync(string subscriptionId, string paymentMethodId)
    {
        try
        {
            var options = new SubscriptionUpdateOptions
            {
                DefaultPaymentMethod = paymentMethodId
            };
            
            await _subscriptionService.UpdateAsync(subscriptionId, options);
            
            return ServiceResult<bool>.Success(true, "Default payment method updated successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error updating default payment method: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc/>
    public async Task<ServiceResult<List<PriceCatalog>>> ListActivePricesAsync()
    {
        try
        {
            // Get all active prices from our database
            var activePrices = await priceCatalogRepository.GetActivePlansAsync();
            return ServiceResult<List<PriceCatalog>>.Success(activePrices, "Successfully retrieved active prices");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PriceCatalog>>.Fail(
                $"Error retrieving active prices: {ex.Message}", 
                null);
        }
    }
    
    /// <summary>
    /// Synchronizes prices from Stripe with the local database
    /// </summary>
    public async Task<ServiceResult<bool>> SyncPricesAsync()
    {
        try
        {
            var options = new PriceListOptions
            {
                Active = true,
                Expand = ["data.product"]
            };
            
            var prices = await _priceService.ListAsync(options);
            var existingPrices = await priceCatalogRepository.GetAllAsync();
            var priceCatalogs = existingPrices.ToList();
            var existingPricesDict = priceCatalogs.ToDictionary(p => p.PriceId);
            
            var updatedPrices = new List<PriceCatalog>();
            var newPrices = new List<PriceCatalog>();
            var pricesToRemove = new List<PriceCatalog>();
            
            // Keep track of which Stripe price IDs exist
            var stripePriceIds = new HashSet<string>(prices.Select(p => p.Id));
            
            foreach (var price in prices)
            {
                var productName = price.Product?.Name ?? "Unknown Plan";
                
                if (existingPricesDict.TryGetValue(price.Id, out var existingPrice))
                {
                    // Update existing price
                    existingPrice.Name = productName;
                    existingPrice.UnitAmount = price.UnitAmount ?? 0;
                    existingPrice.Currency = price.Currency;
                    existingPrice.Interval = price.Recurring?.Interval ?? "unknown";
                    existingPrice.TrialDays = price.Recurring?.TrialPeriodDays != null ? 
                        (int?)price.Recurring.TrialPeriodDays : null;
                    existingPrice.IsActive = price.Active;
                    existingPrice.LastSyncedAt = DateTime.UtcNow;
                    
                    updatedPrices.Add(existingPrice);
                }
                else
                {
                    // Add new price
                    var newPrice = new PriceCatalog
                    {
                        PriceId = price.Id,
                        Name = productName,
                        UnitAmount = price.UnitAmount ?? 0,
                        Currency = price.Currency,
                        Interval = price.Recurring?.Interval ?? "unknown",
                        TrialDays = price.Recurring?.TrialPeriodDays != null ? 
                            (int?)price.Recurring.TrialPeriodDays : null,
                        IsActive = price.Active,
                        LastSyncedAt = DateTime.UtcNow
                    };
                    
                    newPrices.Add(newPrice);
                }
            }
            
            // Find prices that exist in the local database but not in Stripe
            foreach (var existingPrice in priceCatalogs)
            {
                if (!stripePriceIds.Contains(existingPrice.PriceId))
                {
                    pricesToRemove.Add(existingPrice);
                }
            }
            
            // Update existing prices
            if (updatedPrices.Count != 0)
                await priceCatalogRepository.UpdateRangeAsync(updatedPrices);
            
            // Add new prices
            if (newPrices.Count != 0)
                await priceCatalogRepository.AddRangeAsync(newPrices);
            
            // Remove prices that no longer exist in Stripe
            if (pricesToRemove.Count != 0)
                await priceCatalogRepository.RemoveRangeAsync(pricesToRemove);
            
            await priceCatalogRepository.SaveChangesAsync();
            
            return ServiceResult<bool>.Success(
                true, 
                $"Successfully synced {prices.Count()} prices from Stripe. Updated: {updatedPrices.Count}, Added: {newPrices.Count}, Removed: {pricesToRemove.Count}");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error syncing prices from Stripe: {ex.Message}", 
                false,
                ServiceError.Other);
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<bool>> HandleWebhookAsync(string rawJson, string signatureHeader)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                rawJson,
                signatureHeader,
                _webhookSecret);
                  // Handle the event based on its type
            switch (stripeEvent.Type)
            {
                case "customer.subscription.created":
                case "customer.subscription.updated":
                    await HandleSubscriptionEventAsync((stripeEvent.Data.Object as Subscription)!);
                    break;
                    
                case "customer.subscription.deleted":
                    await HandleSubscriptionDeletedEventAsync((stripeEvent.Data.Object as Subscription)!);
                    break;
                    
                case "price.created":
                case "price.updated":
                case "price.deleted":
                    // Trigger a price sync when price catalog changes in Stripe
                    await SyncPricesAsync();
                    break;
            }
            
            return ServiceResult<bool>.Success(true, $"Webhook {stripeEvent.Type} processed successfully");
        }
        catch (StripeException ex)
        {
            return ServiceResult<bool>.Fail(
                $"Error handling Stripe webhook: {ex.Message}", 
                false, 
                ServiceError.Other);
        }
    }
    
    
    /// <summary>
    /// Handles the subscription created or updated event from Stripe
    /// </summary>
    /// <param name="subscription">
    /// Subscription object from Stripe webhook event
    /// </param>
    /// <returns></returns>
    private async Task HandleSubscriptionEventAsync(Subscription subscription)
    {
        try
        {
            // Check if billing exists for this subscription. If not create a local subscription else get the subscription
            var billingExists = await customerBillingRepository.ExistsAsync(b => b.StripeSubscriptionId == subscription.Id);
            if (!billingExists)
            {
                // Check if the stripe customer ID exists in our database. iF yes proceed to create a new billing record else return
                var customer = await customerRepository.GetByStripeCustomerIdAsync(subscription.CustomerId);
                if (customer == null)
                    return;

                var newBilling = new CustomerBilling
                {
                    CustomerId = customer.CustomerId,
                    StripeCustomerId = subscription.CustomerId,
                    StripeSubscriptionId = subscription.Id,
                    PriceId = subscription.Items.Data[0].Price.Id,
                    Status = subscription.Status,
                    CurrentPeriodStart = subscription.Items.Data[0].CurrentPeriodStart,
                    CurrentPeriodEnd = subscription.Items.Data[0].CurrentPeriodStart,
                    CancelAt = subscription.CancelAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await customerBillingRepository.AddAsync(newBilling);
                await customerBillingRepository.SaveChangesAsync();

                return;
            }

            // Find the customer billing record by subscription ID
            var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscription.Id);

            // Update subscription status
            billing.Status = subscription.Status;
            billing.CurrentPeriodStart = subscription.Items.Data[0].CurrentPeriodStart;
            billing.CurrentPeriodEnd = subscription.Items.Data[0].CurrentPeriodStart;
            billing.CancelAt = subscription.CancelAt;

            if (subscription.Items?.Data?.Count > 0)
            {
                // Update price if it changed
                billing.PriceId = subscription.Items.Data[0].Price.Id;
            }

            billing.UpdatedAt = DateTime.UtcNow;

            await customerBillingRepository.UpdateAsync(billing);
            await customerBillingRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the error but don't throw during webhook handling
            Console.WriteLine($"Error handling subscription event: {ex.Message}");
        }
    }
    
    /// <summary>
    ///  Handles the subscription deleted event from Stripe
    /// </summary>
    /// <param name="subscription">
    /// Subscription object from Stripe webhook event
    /// </param>
    private async Task HandleSubscriptionDeletedEventAsync(Subscription subscription)
    {
        try
        {
            // Find the customer billing record by subscription ID
            var billing = await customerBillingRepository.GetBySubscriptionIdAsync(subscription.Id);

            // Mark the subscription as canceled
            billing.Status = "canceled";
            billing.CancelAt = DateTime.UtcNow;
            billing.UpdatedAt = DateTime.UtcNow;
                
            await customerBillingRepository.UpdateAsync(billing);
            await customerBillingRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the error but don't throw during webhook handling
            Console.WriteLine($"Error handling subscription deleted event: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Ensures that stripe customer for exists in our database for an existing customer when creating a subscription
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public async Task<string?> EnsureStripeCustomerExistsAsync(int customerId)
    {
        try
        {
            // Check if a billing exists using the customer id
            var billingExists = await customerBillingRepository.ExistsAsync(b => b.CustomerId == customerId);
            if (billingExists)
            {
                // If billing exists, get the existing billing record
                var billing = await customerBillingRepository.GetByCustomerIdAsync(customerId);
                
                // If we already have a Stripe customer ID, return it
                if (!string.IsNullOrEmpty(billing.StripeCustomerId))
                {
                    return billing.StripeCustomerId;
                }
            }
            
            // Otherwise, fetch customer details and create a Stripe customer
            var customer = await customerRepository.GetByCustomerIdAsync(customerId);        
            var customerOptions = new CustomerCreateOptions
            {
                Name = $"{customer.Name1} {customer.Name2}".Trim(),
                Email = customer.Email,
                Phone = customer.PrimaryPhone,
                Address = new AddressOptions
                {
                    Line1 = customer.Address1,
                    Line2 = customer.Address2,
                    City = customer.City,
                    State = customer.State,
                    PostalCode = customer.ZipCode,
                    Country = customer.Country
                },
                Metadata = new Dictionary<string, string>
                {
                    { "CustomerId", customerId.ToString() }
                }
            };
            
            var stripeCustomer = await _customerService.CreateAsync(customerOptions);
            if (stripeCustomer == null)
                return null;
            
            // Create a customer billing record
            var newBilling = new CustomerBilling
            {
                CustomerId = customerId,
                StripeCustomerId = stripeCustomer.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await customerBillingRepository.AddAsync(newBilling);
            await customerBillingRepository.SaveChangesAsync();
            
            return stripeCustomer.Id;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
