namespace LoadCoveredStripe.API.Infrastructure.Services;

/// <summary>
/// Configuration settings for Stripe payment processing integration.
/// </summary>
/// <remarks>
/// This class holds the necessary API keys and secrets needed to authenticate
/// with the Stripe API for payment processing capabilities.
/// </remarks>
public class StripeSettings
{
    /// <summary>
    /// Gets or sets the Stripe API secret key used for server-side API calls.
    /// </summary>
    /// <remarks>
    /// This key should be kept secure and never exposed to clients.
    /// It provides full access to your Stripe account.
    /// </remarks>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Gets or sets the Stripe webhook signing secret.
    /// </summary>
    /// <remarks>
    /// This secret is used to verify that webhook events were sent by Stripe.
    /// It helps prevent fraudulent events from being processed.
    /// </remarks>
    public required string WebhookSecret { get; set; }
}
