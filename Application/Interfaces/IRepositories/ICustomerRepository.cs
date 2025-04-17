using System;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Models;

namespace LoadCoveredStripe.API.Application.Interfaces.IRepositories
{    /// <summary>
    /// Repository interface for Customer entity operations
    /// </summary>
    public interface ICustomerRepository : IRepository<Customer>
    {
        

        /// <summary>
        /// Retrieves a customer by their unique identifier
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer</param>
        /// <returns>The customer with the specified ID, or null if not found</returns>
        Task<Customer> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Checks if a customer exists by their unique identifier
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer</param>
        /// <returns>True if the customer exists, false otherwise</returns>
        Task<bool> CustomerExistsAsync(int customerId);
        
        /// <summary>
        /// Retrieves a customer by their Stripe customer identifier
        /// </summary>
        /// <param name="stripeCustomerId">The Stripe customer ID</param>
        /// <returns>The customer with the specified Stripe ID, or null if not found</returns>
        Task<Customer?> GetByStripeCustomerIdAsync(string stripeCustomerId);
    }
}
