using System;

namespace LoadCoveredStripe.API.Application.DTOs
{
    /// <summary>
    /// Response DTO for the setup intent creation
    /// </summary>
    public class SetupIntentResponse
    {
        /// <summary>
        /// Customer ID in the application
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// Customer ID in Stripe
        /// </summary>
        public string StripeCustomerId { get; set; }
        
        /// <summary>
        /// Client secret for the setup intent
        /// </summary>
        public string ClientSecret { get; set; }
        
        /// <summary>
        /// Ephemeral key for the setup intent
        /// </summary>
        public string EphemeralKey { get; set; }
    }
}
