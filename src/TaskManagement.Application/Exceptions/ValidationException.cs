namespace TaskManagement.Application.Exceptions;

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
