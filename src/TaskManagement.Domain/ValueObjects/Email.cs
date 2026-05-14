// TaskManagement.Domain/ValueObjects/Email.cs
namespace TaskManagement.Domain.ValueObjects;

using System.Text.RegularExpressions;
using TaskManagement.Domain.Shared;

public sealed class Email : ValueObject
{
    public string Value { get; }
    private Email(string value) => Value = value;
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure("Email is required.");
        // RFC 5322 compliant validation
        var pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        if (!Regex.IsMatch(email, pattern))
            return Result<Email>.Failure("Invalid email format.");
        return Result<Email>.Success(new Email(email.ToLowerInvariant()));
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString() => Value;
}