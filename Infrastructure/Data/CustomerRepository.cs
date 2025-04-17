using System;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Data.AppDbContext;
using LoadCoveredStripe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadCoveredStripe.API.Infrastructure.Data
{
    /// <summary>
    /// Repository implementation for Customer entity operations
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the CustomerRepository class
    /// </remarks>
    /// <param name="context">The database context</param>
public class CustomerRepository(AppDbContext context) : Repository<Customer>(context), ICustomerRepository
    {

        /// <inheritdoc/>
        public async Task<Customer> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet.FirstAsync(c => c.CustomerId == customerId);

        }
        
        /// <inheritdoc/>
        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _dbSet.AnyAsync(c => c.CustomerId == customerId);
        }
        
        /// <inheritdoc/>
        public async Task<Customer?> GetByStripeCustomerIdAsync(string stripeCustomerId)
        {
            // Join customer with customer_billing to find the customer matching the Stripe customer ID
            var customer = await _dbSet
                .Join(_context.CustomerBillings,
                    c => c.CustomerId,
                    cb => cb.CustomerId,
                    (c, cb) => new { Customer = c, CustomerBilling = cb })
                .Where(x => x.CustomerBilling.StripeCustomerId == stripeCustomerId)
                .Select(x => x.Customer)
                .FirstOrDefaultAsync();
                
            return customer;
        }
    }
}
