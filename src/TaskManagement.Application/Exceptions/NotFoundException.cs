namespace TaskManagement.Application.Exceptions;

/// <summary>
/// NotFoundException is thrown when a requested entity cannot be found.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Represents failure to find requested resource
/// - Distinct from other exception types (validation, domain, technical)
/// - Allows handlers to catch and return appropriate HTTP status codes
///
/// Usage:
/// - Thrown by query handlers when entity is not found
/// - Thrown by command handlers when required aggregate is missing
/// - Caught by exception handling middleware
/// - Returns HTTP 404 NotFound status to client
///
/// Error Categories:
/// - NotFoundException: Requested entity doesn't exist (this class)
/// - ValidationException: Input validation failures
/// - DomainException: Business rule violations
/// - Other exceptions: Infrastructure/technical failures
///
/// Benefits:
/// - Clear semantics: distinguishes missing resources from other errors
/// - Appropriate HTTP responses: 404 instead of 400 or 500
/// - Consistent error handling across handlers
/// - Improves API usability and predictability
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
