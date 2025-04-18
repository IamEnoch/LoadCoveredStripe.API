namespace LoadCoveredStripe.API.Application.Interfaces.IServices;

using API.Utilities;
using System.Threading.Tasks;

/// <summary>
/// Service interface for customer-related operations
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Checks if a customer exists in the system
    /// </summary>
    /// <param name="customerId">The ID of the customer to check</param>
    /// <returns>A ServiceResult indicating whether the customer exists</returns>
    Task<ServiceResult<bool>> CustomerExistsAsync(int customerId);
}