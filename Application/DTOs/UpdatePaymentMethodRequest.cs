namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Request model for updating the default payment method for a subscription
/// </summary>
public class UpdatePaymentMethodRequest
{
    /// <summary>
    /// The Stripe subscription ID to update
    /// </summary>
    public string SubId { get; set; }
    
    /// <summary>
    /// The new Stripe payment method ID to assign to the subscription
    /// </summary>
    public string PaymentMethodId { get; set; }
}
