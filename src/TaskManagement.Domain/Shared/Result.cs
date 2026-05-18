// TaskManagement.Domain/Shared/Result.cs
namespace TaskManagement.Domain.Common;

public class Result
{
    public bool IsSuccess
    {
        get;
    }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors
    {
        get;
    }
    protected Result(bool isSuccess, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    }
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, new[] { error });
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result<TValue> Success<TValue>(TValue value) => new(value);
    public static Result<TValue> Failure<TValue>(string error) => new(new[] { error });
    public static Result<TValue> Failure<TValue>(IEnumerable<string> errors) => new(errors);
}

public class Result<T> : Result
{
    public T? Value
    {
        get;
    }
    internal Result(T value) : base(true) => Value = value;
    internal Result(IEnumerable<string> errors) : base(false, errors) => Value = default;
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IReadOnlyList<string>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess(Value!) : onFailure(Errors);
    }
}
