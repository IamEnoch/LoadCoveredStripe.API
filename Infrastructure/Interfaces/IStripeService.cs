using LoadCoveredStripe.API.API.Utilities;
using LoadCoveredStripe.API.Models;

namespace LoadCoveredStripe.API.Infrastructure.Interfaces;

/// <summary>
/// Interface for Stripe service operations
/// </summary>
public interface IStripeService
{
    /// <summary>
    /// Creates a setup intent for the customer to save their payment method
    /// </summary>
    Task<ServiceResult<(string clientSecret, string ephemeralKey)>> CreateSetupIntentAsync(string stripeCustomerId);
    
    /// <summary>
    /// Creates a new subscription for the customer
    /// </summary>
    Task<ServiceResult<string>> CreateSubscriptionAsync(int customerId, string priceId);
    
    /// <summary>
    /// Swaps the plan for an existing subscription
    /// </summary>
    Task<ServiceResult<bool>> SwapPlanAsync(string subscriptionId, string newPriceId);
    
    /// <summary>
    /// Pauses a subscription
    /// </summary>
    Task<ServiceResult<bool>> PauseAsync(string subscriptionId);
    
    /// <summary>
    /// Cancels a subscription either immediately or at period end
    /// </summary>
    Task<ServiceResult<bool>> CancelAsync(string subscriptionId, bool immediate);
    
    /// <summary>
    /// Reactivates a subscription by clearing cancelation or resuming from pause
    /// </summary>
    Task<ServiceResult<bool>> ReactivateAsync(string subscriptionId);
    
    /// <summary>
    /// Updates the default payment method for a subscription
    /// </summary>
    Task<ServiceResult<bool>> UpdateDefaultPaymentMethodAsync(string subscriptionId, string paymentMethodId);
    
    /// <summary>
    /// Retrieves all active prices from the database
    /// </summary>
    /// <returns>A service result containing a list of price catalog entries</returns>
    Task<ServiceResult<List<PriceCatalog>>> ListActivePricesAsync();
    
    /// <summary>
    /// Synchronizes prices from Stripe with the local database
    /// </summary>
    /// <returns>A service result indicating success or failure of the sync operation</returns>
    Task<ServiceResult<bool>> SyncPricesAsync();
      /// <summary>
    /// Handles webhook events from Stripe
    /// </summary>
    Task<ServiceResult<bool>> HandleWebhookAsync(string rawJson, string signatureHeader);
    
    /// <summary>
    /// Ensures a customer exists in Stripe and returns their Stripe customer ID
    /// </summary>
    /// <param name="customerId">The internal customer ID</param>
    /// <returns>The Stripe customer ID if found or created, null otherwise</returns>
    Task<string?> EnsureStripeCustomerExistsAsync(int customerId);
}
