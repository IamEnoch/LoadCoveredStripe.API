using System;

namespace LoadCoveredStripe.API.Application.DTOs;

/// <summary>
/// Request model for creating a setup intent for a customer to save their payment method
/// </summary>
public class CreateSetupIntentRequest
{
    /// <summary>
    /// The unique identifier of the customer
    /// </summary>
    public int CustomerId { get; set; }
}
