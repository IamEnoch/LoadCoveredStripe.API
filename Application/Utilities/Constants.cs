namespace LoadCoveredStripe.API.Application.Utilities;

/// <summary>
///     Defines the Constants for the Application
/// </summary>
public static class Constants
{
    /// <summary>
    ///     The name of the SQL database connection string in the configuration.
    /// </summary>
    public const string SQL_DB_CONN_STRING_NAME = "ConnectionStrings:DbConnString";

    /// <summary>
    ///     The name of the Stripe secret key in the configuration.
    /// </summary>
    public const string STRIPE_SECRET_KEY_NAME = "Stripe:SecretKey";
    
    /// <summary>
    ///     The name of the Stripe webhook secret in the configuration.
    /// </summary>
    public const string STRIPE_WEBHOOK_SECRET_NAME = "Stripe:WebhookSecret";
}