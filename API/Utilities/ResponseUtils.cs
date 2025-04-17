using System.Net;
using LoadCoveredStripe.API.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LoadCoveredStripe.API.API.Utilities;

/// <summary>
///     Provides utilities for handling service results in ASP.NET Core.
/// </summary>
public static class ResponseUtils
{
    /// <summary>
    ///     Handles the result of a service operation, converting it to an appropriate ActionResult.
    /// </summary>
    /// <typeparam name="T">The type of the data contained in the service result.</typeparam>
    /// <param name="result">The service result to be handled.</param>
    /// <returns>
    ///     Returns an appropriate ActionResult based on the service result. This includes:
    ///     - OkObjectResult if the operation was successful.
    ///     - NotFoundObjectResult, BadRequestObjectResult, or ConflictObjectResult based on the type of error.
    /// </returns>
    public static ActionResult<T> HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
            return result.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result.Data),
                HttpStatusCode.Created => new CreatedResult("", result.Data),
                _ => new OkObjectResult(result.Data)
            };

        return result.Error switch
        {
            ServiceError.NotFound => new NotFoundObjectResult(result),
            ServiceError.ValidationError => new BadRequestObjectResult(result),
            ServiceError.Conflict => new ConflictObjectResult(result),
            ServiceError.TooManyRequests => new StatusCodeResult((int)HttpStatusCode.TooManyRequests),
            _ => new BadRequestObjectResult(result)
        };
    }
    
    /// <summary>
    ///     Handles the result of a service operation, converting it to an appropriate ActionResult. This method includes the details of the service result.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///     Returns an appropriate ActionResult based on the service result. This includes:
    ///     - OkObjectResult if the operation was successful.
    ///     - NotFoundObjectResult, BadRequestObjectResult, or ConflictObjectResult based on the type of error.
    /// </returns>
    public static ActionResult<ServiceResult<T>> HandleServiceResultWithDetails<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
            return result.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result),
                HttpStatusCode.Created => new CreatedResult("", result),
                _ => new OkObjectResult(result)
            };

        return result.Error switch
        {
            ServiceError.NotFound => new NotFoundObjectResult(result),
            ServiceError.ValidationError => new BadRequestObjectResult(result),
            ServiceError.Conflict => new ConflictObjectResult(result),
            ServiceError.TooManyRequests => new StatusCodeResult((int)HttpStatusCode.TooManyRequests),
            _ => new BadRequestObjectResult(result)
        };
    }
}