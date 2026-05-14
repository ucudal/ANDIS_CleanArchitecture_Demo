// TaskManagement.Application/Behaviors/ValidationBehavior.cs
namespace TaskManagement.Application.Behaviors;

using FluentValidation;
using MediatR;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Shared;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();
        if (failures.Any())
        {
            // Use reflection to create failure result of correct generic type
            return CreateFailureResult<TResponse>(failures);
        }
        return await next();
    }
    private static TResponse CreateFailureResult<TResponse>(List<string> errors)
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(Result<>).MakeGenericType(responseType.GenericTypeArguments[0]);
            var failureMethod = resultType.GetMethod("Failure", new[] { typeof(IEnumerable<string>) })!;
            return (TResponse)failureMethod.Invoke(null, new object[] { errors })!;
        }
        return (TResponse)(object)Result.Failure(errors);
    }
}
// FluentValidation validator
public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
        RuleFor(x => x.DueDate)
            .Must(date => !date.HasValue || date.Value >= DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");
        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.");
    }
}