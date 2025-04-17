namespace LoadCoveredStripe.API.Application.Enums;

/// <summary>
///     Enumerates the possible error types that can result from service operations.
/// </summary>
public enum ServiceError
{
    /// <summary>
    ///     Indicates that the requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    ///     Indicates that there was a validation error with the provided data.
    /// </summary>
    ValidationError,

    /// <summary>
    ///     Indicates that there is a conflict with the current state of the resource.
    /// </summary>
    Conflict,
    
    /// <summary>
    ///     Indicates that too many requests were made to the service.
    /// </summary>
    TooManyRequests,

    /// <summary>
    ///     Represents any other type of error not covered by the specific cases.
    /// </summary>
    Other
}