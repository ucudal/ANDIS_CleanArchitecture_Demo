// TaskManagement.Domain/ValueObjects/Email.cs
using System.Text.RegularExpressions;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.ValueObjects;

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
