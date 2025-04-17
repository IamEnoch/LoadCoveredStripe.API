namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Base request model for subscription operations that only require a subscription ID
/// </summary>
public class SubscriptionRequest
{
    /// <summary>
    /// The Stripe subscription ID to operate on
    /// </summary>
    public string SubId { get; set; }
}
