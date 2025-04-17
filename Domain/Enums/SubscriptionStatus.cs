namespace LoadCoveredStripe.API.Domain.Enums;

/// <summary>
/// Represents the possible states of a Stripe subscription
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// The subscription is currently in a trial period
    /// </summary>
    Trialing,
    
    /// <summary>
    /// The subscription is active and the customer is being billed according to the plan
    /// </summary>
    Active,
    
    /// <summary>
    /// Payment has failed and the subscription is past due
    /// </summary>
    PastDue,
    
    /// <summary>
    /// The subscription has been paused temporarily
    /// </summary>
    Paused,
    
    /// <summary>
    /// The subscription has been canceled and will end at the current period end
    /// </summary>
    CanceledPendingEnd,
    
    /// <summary>
    /// The subscription has been canceled and is no longer active
    /// </summary>
    Canceled,
    
    /// <summary>
    /// The subscription has ended, usually after a set duration
    /// </summary>
    Ended,
    
    /// <summary>
    /// The subscription has not been paid for and is in grace period awaiting payment
    /// </summary>
    Unpaid,
    
    /// <summary>
    /// The subscription is incomplete and awaiting completion (initial payment or setup)
    /// </summary>
    Incomplete,
    
    /// <summary>
    /// The subscription is incomplete and has expired due to initial payment failure
    /// </summary>
    IncompleteExpired
}
