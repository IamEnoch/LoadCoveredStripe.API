using System.Net;
using LoadCoveredStripe.API.Application.Enums;

namespace LoadCoveredStripe.API.API.Utilities;

/// <summary>
///     Denotes the standard structure of a Server (API) Response body
/// </summary>
public class ServiceResult<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceResult{T}" /> class
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <param name="error"></param>
    /// <param name="statusCode"></param>
    public ServiceResult(bool isSuccess, string message, T? data, ServiceError? error, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        Error = error;
        StatusCode = statusCode;
    }

    /// <summary>
    ///     HTTP Status Code to be returned to the client
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    ///     Denotes the possible error that may arise during a service operation
    /// </summary>
    public ServiceError? Error { get; set; }

    /// <summary>
    ///     Denotes if the operation was successful or not
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    ///     Message to be returned to the client
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Data to be returned to the client
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    ///     Creates a static factory method for a successful response
    /// </summary>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static ServiceResult<T> Success(T? data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>(true, message, data, null, statusCode);
    }

    /// <summary>
    ///     Creates a static factory method for a failed response
    /// </summary>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ServiceResult<T> Fail(string message, T? data, ServiceError? error = ServiceError.Other)
    {
        return new ServiceResult<T>(false, message, data, error, HttpStatusCode.BadRequest);
    }
}