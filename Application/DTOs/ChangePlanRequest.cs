namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Request model for changing a customer's subscription plan
/// </summary>
public class ChangePlanRequest
{
    /// <summary>
    /// The Stripe subscription ID to modify
    /// </summary>
    public string SubId { get; set; }
    
    /// <summary>
    /// The new Stripe price ID to switch to
    /// </summary>
    public string PriceId { get; set; }
}
