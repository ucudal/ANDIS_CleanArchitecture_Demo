namespace TaskManagement.Domain.Exceptions;

/// <summary>
/// DomainException is the base exception class for all domain layer errors.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Represents business rule violations and domain invariant breaches
/// - Distinguishes domain errors from technical infrastructure errors
/// - Allows application layers to handle business logic failures appropriately
///
/// Usage:
/// - Thrown when domain invariants are violated
/// - Thrown when business rules are not satisfied
/// - Caught and handled by application service layer
/// - Should never be thrown for technical/infrastructure issues
///
/// Benefits:
/// - Clear separation between business and technical errors
/// - Explicit contracts about what can go wrong in domain logic
/// - Enables proper error handling at application level
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }

    public DomainException()
    {
    }
}
