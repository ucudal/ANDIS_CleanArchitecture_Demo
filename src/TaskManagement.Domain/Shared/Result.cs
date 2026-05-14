// TaskManagement.Domain/Shared/Result.cs
namespace TaskManagement.Domain.Shared;
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors { get; }
    protected Result(bool isSuccess, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    }
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, new[] { error });
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }
    private Result(T value) : base(true) => Value = value;
    private Result(IEnumerable<string> errors) : base(false, errors) => Value = default;
    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(string error) => new(new[] { error });
    public new static Result<T> Failure(IEnumerable<string> errors) => new(errors);
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IReadOnlyList<string>, TResult> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Errors);
}