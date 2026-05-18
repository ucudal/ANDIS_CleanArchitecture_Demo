// TaskManagement.Application/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Behaviors;

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
        ArgumentNullException.ThrowIfNull(next);

        if (!_validators.Any())
            return await next(cancellationToken).ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Count > 0)
        {
            // Use reflection to create failure result of correct generic type
            return CreateFailureResult(failures);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }

    private static TResponse CreateFailureResult(List<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArgument = responseType.GenericTypeArguments[0];
            var failureMethod = typeof(Result).GetMethods()
                .First(m =>
                    m.Name == nameof(Result.Failure)
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Length == 1
                    && m.GetParameters()[0].ParameterType == typeof(IEnumerable<string>));

            return (TResponse)failureMethod.MakeGenericMethod(genericArgument)
                .Invoke(null, new object[] { errors })!;
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
