using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LoadCoveredStripe.API.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Generic repository interface that defines standard operations for entity access
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its primary key
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        /// <returns>The entity with the specified primary key</returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// Retrieves all entities of type T
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entities that match the specified condition
        /// </summary>
        /// <param name="predicate">The condition to filter entities</param>
        /// <returns>Collection of entities that match the condition</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Retrieves the first entity that matches the specified condition, or null if none found
        /// </summary>
        /// <param name="predicate">The condition to filter entities</param>
        /// <returns>The first entity that matches or null</returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the repository
        /// </summary>
        /// <param name="entities">The entities to add</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the repository
        /// </summary>
        /// <param name="entity">The entity to update</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        Task RemoveAsync(T entity);

        /// <summary>
        /// Removes multiple entities from the repository
        /// </summary>
        /// <param name="entities">The entities to remove</param>
        Task RemoveRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Determines whether any entity matches the specified condition
        /// </summary>
        /// <param name="predicate">The condition to check</param>
        /// <returns>True if any entity matches the condition, otherwise false</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts entities that match the specified condition
        /// </summary>
        /// <param name="predicate">The condition to count entities (optional)</param>
        /// <returns>The number of entities that match the condition</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// Persists all changes to the database
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Returns a queryable collection of entities
        /// </summary>
        /// <returns>Queryable collection of entities</returns>
        IQueryable<T> Query();
    }
}
