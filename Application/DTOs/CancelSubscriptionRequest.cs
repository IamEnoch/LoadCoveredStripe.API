namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Request model for canceling a customer's subscription
/// </summary>
public class CancelSubscriptionRequest
{
    /// <summary>
    /// The Stripe subscription ID to cancel
    /// </summary>
    public string SubId { get; set; }
    
    /// <summary>
    /// Whether to cancel the subscription immediately or at the end of the current billing period
    /// </summary>
    /// <remarks>
    /// If set to true, the subscription will be canceled immediately.
    /// If set to false (default), the subscription will be canceled at the end of the current billing period.
    /// </remarks>
    public bool Immediate { get; set; } = false;
}
