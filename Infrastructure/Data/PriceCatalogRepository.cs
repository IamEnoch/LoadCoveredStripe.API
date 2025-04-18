using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadCoveredStripe.API.Infrastructure.Data
{
    /// <summary>
    /// Repository implementation for PriceCatalog entity operations
    /// </summary>
    public class PriceCatalogRepository : Repository<PriceCatalog>, IPriceCatalogRepository
    {
        /// <summary>
        /// Initializes a new instance of the PriceCatalogRepository class
        /// </summary>
        /// <param name="context">The database context</param>
        public PriceCatalogRepository(AppDbContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<PriceCatalog> GetByPriceIdAsync(string priceId)
        {
            return await _dbSet.FirstAsync(p => p.PriceId == priceId);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByPriceIdAsync(string priceId)
        {
            return await _dbSet.AnyAsync(p => p.PriceId == priceId);
        }

        // /// <inheritdoc/>
        // public async Task<PriceCatalog> GetByProductIdAsync(string productId)
        // {
        //     return await _dbSet.FirstOrDefaultAsync(p => p. == productId);
        // }

        /// <inheritdoc/>
        public async Task<List<PriceCatalog>> GetActivePlansAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task UpdatePlanStatusAsync(string priceId, bool isActive)
        {
            var plan = await GetByPriceIdAsync(priceId);
            
            if (plan != null)
            {
                plan.IsActive = isActive;
                await SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateRangeAsync(IEnumerable<PriceCatalog> priceCatalogs)
        {
            _dbSet.UpdateRange(priceCatalogs);
            await SaveChangesAsync();
        }
    }
}
