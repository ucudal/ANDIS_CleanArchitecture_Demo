namespace TaskManagement.Application.Exceptions;

/// <summary>
/// ValidationException is thrown when input validation fails in the application layer.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Represents input validation failures (not domain business rule violations)
/// - Distinct from DomainException (business rules) and technical exceptions
/// - Allows handlers to catch and return appropriate HTTP status codes
///
/// Usage:
/// - Thrown by ValidationBehavior when FluentValidation fails
/// - Caught by exception handling middleware
/// - Returns HTTP 400 BadRequest status to client
///
/// Error Categories:
/// - ValidationException: Input validation failures (this class)
/// - DomainException: Business rule violations
/// - Other exceptions: Infrastructure/technical failures
///
/// Benefits:
/// - Clear distinction between different error types
/// - Appropriate error responses per error category
/// - Separated concerns: validation vs business rules
/// - Easier debugging and logging of different error types
/// </summary>
public sealed class ValidationException : Exception
{
    private static readonly IDictionary<string, string[]> EmptyErrors =
        new Dictionary<string, string[]>();

    public IDictionary<string, string[]> Errors
    {
        get;
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public ValidationException()
    {
        Errors = EmptyErrors;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = EmptyErrors;
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = EmptyErrors;
    }
}
