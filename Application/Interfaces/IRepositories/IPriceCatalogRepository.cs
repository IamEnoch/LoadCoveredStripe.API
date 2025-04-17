using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Models;

namespace LoadCoveredStripe.API.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for PriceCatalog entity operations
    /// </summary>
    public interface IPriceCatalogRepository : IRepository<PriceCatalog>
    {
        /// <summary>
        /// Retrieves a price catalog entry by its Stripe price identifier
        /// </summary>
        /// <param name="priceId">The Stripe price ID</param>
        /// <returns>The price catalog entry with the specified price ID, or null if not found</returns>
        Task<PriceCatalog> GetByPriceIdAsync(string priceId);

        /// <summary>
        /// Checks if a price catalog entry exists with the given price ID
        /// </summary>
        /// <param name="priceId">The Stripe price ID to check</param>
        /// <returns>True if a price with the specified ID exists, false otherwise</returns>
        Task<bool> ExistsByPriceIdAsync(string priceId);

        /// <summary>
        /// Retrieves all active pricing plans
        /// </summary>
        /// <returns>A list of active price catalog entries</returns>
        Task<List<PriceCatalog>> GetActivePlansAsync();

        /// <summary>
        /// Updates the active status of a pricing plan
        /// </summary>
        /// <param name="priceId">The Stripe price ID of the plan to update</param>
        /// <param name="isActive">The new active status</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task UpdatePlanStatusAsync(string priceId, bool isActive);

        /// <summary>
        /// Updates multiple price catalog entries at once
        /// </summary>
        /// <param name="priceCatalogs">The collection of price catalog entries to update</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task UpdateRangeAsync(IEnumerable<PriceCatalog> priceCatalogs);
    }
}
