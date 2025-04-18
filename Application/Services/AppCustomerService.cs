using System.Net;
using System.Threading.Tasks;
using LoadCoveredStripe.API.API.Utilities;
using LoadCoveredStripe.API.Application.Enums;
using LoadCoveredStripe.API.Application.Interfaces.IRepositories;
using LoadCoveredStripe.API.Application.Interfaces.IServices;

namespace LoadCoveredStripe.API.Application.Services
{
    /// <summary>
    /// Implementation of customer-related service operations
    /// </summary>
    public class AppCustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCustomerService"/> class
        /// </summary>
        /// <param name="customerRepository">The customer repository</param>
        public AppCustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<bool>> CustomerExistsAsync(int customerId)
        {
            if (customerId <= 0)
            {
                return new ServiceResult<bool>(
                    false,
                    "Invalid customer ID provided",
                    false,
                    ServiceError.ValidationError,
                    HttpStatusCode.BadRequest);
            }
            
            var exists = await _customerRepository.CustomerExistsAsync(customerId);
                
            if (exists)
            {
                return new ServiceResult<bool>(
                    true,
                    "Customer exists",
                    true,
                    null,
                    HttpStatusCode.OK);
            }
            else
            {
                return new ServiceResult<bool>(
                    false,
                    $"Customer with ID {customerId} not found",
                    false,
                    ServiceError.NotFound,
                    HttpStatusCode.NotFound);
            }
            
        }
    }
}
