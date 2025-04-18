using LoadCoveredStripe.API.API.Utilities;
using LoadCoveredStripe.API.Application.DTOs;
using LoadCoveredStripe.API.Application.Interfaces.IServices;
using LoadCoveredStripe.API.Infrastructure.Data;
using LoadCoveredStripe.API.Infrastructure.Interfaces;
using LoadCoveredStripe.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadCoveredStripe.API.API.Controllers;

/// <summary>
/// Handles Stripe-related operations such as creating setup intents, managing subscriptions, and handling webhooks.
/// </summary>
/// <param name="stripeService">
/// The service responsible for interacting with Stripe's API.
/// </param>
/// <param name="context">
///  The database context for accessing application data.
/// </param>
[ApiController]
[Route("api/[controller]")]
public class StripeController(IStripeService stripeService, ICustomerService customerService, AppDbContext context) : ControllerBase
{
    /// <summary>
    ///     Get all active plans from the price catalog.
    /// </summary>
    /// <returns>
    ///     A list of active pricing plans.
    /// </returns>
    [HttpGet("plans")]
    public async Task<ActionResult<List<PriceCatalog>>> GetPlans()
    {
        var plans = await context.PriceCatalogs
            .Where(p => p.IsActive)
            .ToListAsync();
            
        return Ok(plans);
    }
    
    /// <summary>
    /// Create a setup intent for a customer to save their payment method.
    /// </summary>
    /// <param name="request">
    ///     The request containing the customer ID for whom the setup intent is created.
    ///     The customer must exist in the database and have a corresponding Stripe customer ID.
    /// </param>
    /// <returns>
    ///     A response containing the client secret and ephemeral key for the setup intent. 
    /// </returns>
    [HttpPost("create-setup-intent")]
    public async Task<ActionResult<(string clientSecret, string ephemeralKey)>> CreateSetupIntent([FromBody] CreateSetupIntentRequest request)
    {
        // Validate customer ID
        var customerExists = await customerService.CustomerExistsAsync(request.CustomerId);
        if (customerExists.IsSuccess == false)
        {
            // Return an object getting the status code fomr the ServiceResult
            return StatusCode((int)customerExists.StatusCode, customerExists);
        }
        
        // Ensure customer exists in Stripe
        var stripeCustomerId = await stripeService.EnsureStripeCustomerExistsAsync(request.CustomerId);
            
        if(stripeCustomerId == null)
            return BadRequest("Stripe customer not found");
            
        // Create setup intent and ephemeral key
        var intentResult = await stripeService.CreateSetupIntentAsync(stripeCustomerId);
        if (intentResult.IsSuccess == false)
        {
            return ResponseUtils.HandleServiceResult(intentResult);
        }
            
        return Ok(new SetupIntentResponse
        {
            CustomerId = request.CustomerId,
            StripeCustomerId = stripeCustomerId,
            ClientSecret = intentResult.Data.clientSecret,
            EphemeralKey = intentResult.Data.ephemeralKey
        });
    }

    /// <summary>
    /// Create a new subscription for a customer.
    /// </summary>
    /// <param name="request">
    ///     The request containing the customer ID and price ID for the subscription.
    /// </param>
    /// <returns>
    ///     A response containing the subscription ID if successful, or an error message if not.
    /// </returns>
    [HttpPost("subscribe")]
    public async Task<ActionResult<string>> Subscribe([FromBody] SubscribeRequest request)
    {
        var subscriptionId = await stripeService.CreateSubscriptionAsync(request.CustomerId, request.PriceId);
        return ResponseUtils.HandleServiceResult(subscriptionId);

    }
    
    /// <summary>
    /// Change the subscription plan for a customer.
    /// </summary>
    /// <param name="request">
    ///     The request containing the subscription ID and new price ID for the plan change.
    /// </param>
    /// <returns>
    ///     A response indicating whether the plan change was successful or not.
    ///     Returns true if the plan was changed successfully, false otherwise.
    /// </returns>
    [HttpPost("subscription/change")]
    public async Task<ActionResult<bool>> ChangePlan([FromBody] ChangePlanRequest request)
    {
        var result = await stripeService.SwapPlanAsync(request.SubId, request.PriceId);
        return ResponseUtils.HandleServiceResult(result);
    }

    /// <summary>
    /// Pause a customer's subscription.
    /// </summary>
    /// <param name="request">
    ///     The request containing the subscription ID to be paused.
    /// </param>
    /// <returns>
    ///     A response indicating whether the subscription was paused successfully or not.
    ///     Returns true if the subscription was paused successfully, false otherwise.
    /// </returns>
    [HttpPost("subscription/pause")]
    public async Task<ActionResult<bool>> PauseSubscription([FromBody] SubscriptionRequest request)
    {
        var result = await stripeService.PauseAsync(request.SubId);
        return ResponseUtils.HandleServiceResult(result);
    }

    /// <summary>
    /// Cancel a customer's subscription.
    /// </summary>
    /// <param name="request">
    ///     The request containing the subscription ID and whether to cancel immediately or at period end.
    /// </param>
    /// <returns>
    ///     A response indicating whether the subscription was canceled successfully or not.
    ///     Returns true if the subscription was canceled successfully, false otherwise.
    /// </returns>
    [HttpPost("subscription/cancel")]
    public async Task<ActionResult<bool>> CancelSubscription([FromBody] CancelSubscriptionRequest request)
    {
        var result = await stripeService.CancelAsync(request.SubId, request.Immediate);
        return ResponseUtils.HandleServiceResult(result);            
    }
    
    /// <summary>
    /// Reactivate a customer's subscription.
    /// </summary>
    /// <param name="request">
    ///     The request containing the subscription ID to be reactivated.
    /// </param>
    /// <returns>
    ///     A response indicating whether the subscription was reactivated successfully or not.
    /// </returns>
    [HttpPost("subscription/reactivate")]
    public async Task<ActionResult<bool>> ReactivateSubscription([FromBody] SubscriptionRequest request)
    {
        var result = await stripeService.ReactivateAsync(request.SubId);
        return ResponseUtils.HandleServiceResult(result);
    }
    
    /// <summary>
    /// Update the default payment method for a subscription.
    /// </summary>
    /// <param name="request">
    ///     The request containing the subscription ID and the new payment method ID.
    ///     The payment method must be valid and associated with the customer.
    /// </param>
    /// <returns>
    ///     A response indicating whether the payment method was updated successfully or not.
    ///     Returns true if the payment method was updated successfully, false otherwise.
    /// </returns>
    [HttpPost("subscription/update-payment-method")]
    public async Task<ActionResult<bool>> UpdatePaymentMethod([FromBody] UpdatePaymentMethodRequest request)
    {
        var result = await stripeService.UpdateDefaultPaymentMethodAsync(request.SubId, request.PaymentMethodId);
        return ResponseUtils.HandleServiceResult(result);
    }
    
    /// <summary>
    /// Get the subscription details for a specific customer.
    /// </summary>
    /// <param name="customerId">
    ///     The unique identifier of the customer whose subscription details are to be retrieved.
    /// </param>
    /// <returns>
    ///      A response containing the subscription details if found, or a 404 Not Found status if not.
    /// </returns>
    [HttpGet("subscription")]
    public async Task<ActionResult> GetSubscription([FromQuery] int customerId)
    {
        var billing = await context.CustomerBillings
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
                
        if (billing == null)
        {
            return NotFound();
        }
            
        return Ok(billing);
    }

    /// <summary>
    /// Handles incoming webhooks from Stripe.
    /// </summary>
    /// <returns>
    ///     A response indicating whether the webhook was handled successfully or not.
    ///     Returns true if the webhook was handled successfully, false otherwise.
    /// </returns>
    [HttpPost("webhook")]
    public async Task<ActionResult<bool>> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];
        
        var result = await stripeService.HandleWebhookAsync(json, signatureHeader!);
        
        return ResponseUtils.HandleServiceResult(result);
    }
}
