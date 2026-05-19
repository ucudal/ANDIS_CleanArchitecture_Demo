// TaskManagement.Domain/ValueObjects/Email.cs
using System.Text.RegularExpressions;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.ValueObjects;

/// <summary>
/// Email is a Domain Value Object encapsulating email address logic and validation.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Represents an immutable domain concept with built-in validation
/// - Encapsulates email-specific business rules and validation logic
/// - Used for type-safe email handling throughout the domain
/// - Prevents invalid email addresses from entering the system
///
/// Value Object Characteristics:
/// - Immutable: Email value cannot change after creation
/// - Identity by Value: Two Email instances are equal if their addresses match
/// - Self-validating: Ensures only valid email addresses are created
/// - No direct persistence concerns: Infrastructure handles storage
/// </summary>

public sealed class Email : ValueObject
{
    public string Value
    {
        get;
    }
    private Email(string value) => Value = value;
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email is required.");
        // RFC 5322 compliant validation
        var pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        if (!Regex.IsMatch(email, pattern))
            return Result.Failure<Email>("Invalid email format.");
        return Result.Success(new Email(email.ToUpperInvariant()));
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString() => Value;
}
