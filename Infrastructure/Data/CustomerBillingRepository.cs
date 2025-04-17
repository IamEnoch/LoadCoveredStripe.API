using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Data.AppDbContext;
using LoadCoveredStripe.API.Domain.Enums;
using LoadCoveredStripe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadCoveredStripe.API.Infrastructure.Data
{
    /// <summary>
    /// Repository implementation for CustomerBilling entity operations
    /// </summary>
    public class CustomerBillingRepository : Repository<CustomerBilling>, ICustomerBillingRepository
    {
        /// <summary>
        /// Initializes a new instance of the CustomerBillingRepository class
        /// </summary>
        /// <param name="context">The database context</param>
        public CustomerBillingRepository(AppDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<CustomerBilling> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstAsync(cb => cb.Id == id);
        }       
        
        /// <inheritdoc/>
        public async Task<CustomerBilling> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet.FirstAsync(cb => cb.CustomerId == customerId);
        }
        
        /// <inheritdoc/>
        public async Task<CustomerBilling> GetByStripeCustomerIdAsync(string stripeCustomerId)
        {
            return await _dbSet.FirstAsync(cb => cb.StripeCustomerId == stripeCustomerId);
        }

        /// <inheritdoc/>
        public async Task<CustomerBilling> GetBySubscriptionIdAsync(string subscriptionId)
        {
            return await _dbSet.FirstAsync(cb => cb.StripeSubscriptionId == subscriptionId);
        }
        
        /// <inheritdoc/>
        public async Task<CustomerBilling> CreateSubscriptionAsync(int customerId, string priceId)
        {
            // Check if a billing record already exists for this customer
            var existingBilling = await GetByCustomerIdAsync(customerId);
            if (existingBilling != null)
            {
                // Update existing record with new subscription information
                existingBilling.PriceId = priceId;
                existingBilling.Status = SubscriptionStatus.Active.ToString(); // Set as active by default
                existingBilling.UpdatedAt = DateTime.UtcNow;
                
                await UpdateAsync(existingBilling);
                await SaveChangesAsync();
                
                return existingBilling;
            }
            
            // If no existing billing record exists, we can't create a subscription without a Stripe customer ID
            // Creating a new Stripe customer should be handled at the service layer
            throw new InvalidOperationException(
                $"No customer billing record exists for customer ID {customerId}. Create a Stripe Customer first.");
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateSubscriptionAsync(Expression<Func<CustomerBilling, bool>> predicate, Action<CustomerBilling> updateAction)
        {
            var billing = await _dbSet.FirstOrDefaultAsync(predicate);

            if (billing != null)
            {
                // Apply the updates using the action
                updateAction(billing);

                // Always update the timestamp
                billing.UpdatedAt = DateTime.UtcNow;

                await SaveChangesAsync();
                return true;
            }

            return false;
        }
          
        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Expression<Func<CustomerBilling, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task UpdateCustomerStripeIdAsync(int customerId, string stripeCustomerId)
        {
            var billing = await GetByCustomerIdAsync(customerId);
            if (billing != null)
            {
                billing.StripeCustomerId = stripeCustomerId;
                billing.UpdatedAt = DateTime.UtcNow;

                await UpdateAsync(billing);
                await SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"No customer billing record exists for customer ID {customerId}.");
            }
        }
    }
}
