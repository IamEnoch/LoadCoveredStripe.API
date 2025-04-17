using System;

namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Request model for subscribing a customer to a Stripe plan
/// </summary>
public class SubscribeRequest
{
    /// <summary>
    /// The unique identifier of the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The Stripe price ID of the plan to subscribe to
    /// </summary>
    public string PriceId { get; set; }
}
