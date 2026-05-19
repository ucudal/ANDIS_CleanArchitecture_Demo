// TaskManagement.Domain/Shared/Result.cs
namespace TaskManagement.Domain.Common;

/// <summary>
/// Result is a discriminated union type implementing the Result pattern for error handling.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer - Shared/Common)
/// - Replaces exceptions with explicit success/failure returns
/// - Enables functional error handling throughout the application
/// - Prevents hidden errors from being thrown and caught unexpectedly
/// - Provides a consistent way to handle operations that may fail
///
/// Pattern Benefits:
/// - Explicit error handling: Callers must check for success or failure
/// - Type-safe: Generic Result&lt;T&gt; preserves both success value and error information
/// - Functional: Supports composition and chaining of operations
/// - Avoids null checks: Always returns a valid Result object
/// - Performance: Avoids exception stack unwinding overhead
///
/// Usage:
/// - Use Result for operations that should not throw exceptions
/// - Use Result&lt;T&gt; when operation returns a value on success
/// - Pattern match with IsSuccess and Errors to handle both cases
/// - Use Match method for functional-style error handling
/// </summary>
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

/// <summary>
/// Result&lt;T&gt; is a generic discriminated union type for operations that return a value on success.
///
/// Inherits from Result and adds a Value property for the success case.
/// </summary>
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
