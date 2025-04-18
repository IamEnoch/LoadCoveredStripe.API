using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Domain.Entities;
using LoadCoveredStripe.API.Domain.Enums;
using LoadCoveredStripe.API.Models;

namespace LoadCoveredStripe.API.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for CustomerBilling entity operations
    /// </summary>
    public interface ICustomerBillingRepository : IRepository<CustomerBilling>
    {
        /// <summary>
        /// Retrieves billing information by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the billing record</param>
        /// <returns>The customer billing record, or null if not found</returns>
        Task<CustomerBilling> GetByIdAsync(Guid id);
        /// <summary>
        /// Retrieves billing information for a specific customer by integer ID
        /// </summary>
        /// <param name="customerId">The integer identifier of the customer</param>
        /// <returns>The customer billing record, or null if not found</returns>
        Task<CustomerBilling> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Retrieves billing information by Stripe customer identifier
        /// </summary>
        /// <param name="stripeCustomerId">The Stripe customer ID</param>
        /// <returns>The customer billing record with the specified Stripe customer ID, or null if not found</returns>
        Task<CustomerBilling> GetByStripeCustomerIdAsync(string stripeCustomerId);

        /// <summary>
        /// Retrieves billing information by subscription identifier
        /// </summary>
        /// <param name="subscriptionId">The Stripe subscription ID</param>
        /// <returns>The customer billing record with the specified subscription ID, or null if not found</returns>
        Task<CustomerBilling> GetBySubscriptionIdAsync(string subscriptionId);
        /// <summary>
        /// Creates a new subscription for a customer that starts immediately
        /// </summary>
        /// <param name="customerId">The integer identifier of the customer</param>
        /// <param name="priceId">The Stripe price ID</param>
        /// <returns>The newly created CustomerBilling entity</returns>
        Task<CustomerBilling> CreateSubscriptionAsync(int customerId, string priceId);
        /// <summary>
        /// Updates a subscription based on a predicate and specified update action
        /// </summary>
        /// <param name="predicate">The condition to find the subscription to update</param>
        /// <param name="updateAction">The action that defines the updates to apply</param>
        /// <returns>True if a matching record was found and updated, false otherwise</returns>
        Task<bool> UpdateSubscriptionAsync(Expression<Func<CustomerBilling, bool>> predicate, Action<CustomerBilling> updateAction);

        /// <summary>
        /// Checks if a customer billing record exists based on a flexible predicate
        /// </summary>
        /// <param name="predicate">The condition to check for existence</param>
        /// <returns>True if a matching record exists, false otherwise</returns>
        Task<bool> ExistsAsync(Expression<Func<CustomerBilling, bool>> predicate);
        
        // /// <summary>
        // /// Retrieves a customer by their Stripe customer identifier
        // /// </summary>
        // /// <param name="stripeCustomerId">The Stripe customer ID</param>
        // /// <returns>The customer with the specified Stripe ID, or null if not found</returns>
        // Task<Customer> GetCustomerByStripeCustomerIdAsync(string stripeCustomerId);
        
        // /// <summary>
        // /// Retrieves a customer along with their billing information
        // /// </summary>
        // /// <param name="customerId">The unique identifier of the customer</param>
        // /// <returns>The customer with billing information included, or null if not found</returns>
        // Task<Customer> GetCustomerWithBillingInfoAsync(int customerId);
        
        /// <summary>
        /// Updates the Stripe customer ID for an existing customer
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer</param>
        /// <param name="stripeCustomerId">The new Stripe customer ID to set</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task UpdateCustomerStripeIdAsync(int customerId, string stripeCustomerId);
    }
}
