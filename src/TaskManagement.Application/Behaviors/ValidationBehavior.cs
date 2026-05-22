// TaskManagement.Application/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Behaviors;

/// <summary>
/// ValidationBehavior es un comportamiento de pipeline de <see cref="MediatR"/> para validar comandos y consultas.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte del core de la aplicación (Capa de Aplicación)</item>
/// <item>Preocupación transversal: Se aplica a todos los comandos y consultas</item>
/// <item>Comportamiento de pipeline: Intercepta todas las solicitudes de <see cref="MediatR"/> antes de que se ejecuten los manejadores</item>
/// <item>Valida entradas utilizando el marco de trabajo FluentValidation</item>
/// </list>
///
/// Patrón de pipeline de <see cref="MediatR"/>:
/// <list type="bullet">
/// <item>Los comportamientos envuelven el manejo de solicitudes (como middleware en ASP.NET Core)</item>
/// <item>El orden de registro determina el orden de ejecución</item>
/// <item>Puede manejar validación, registro, monitoreo de rendimiento, caché, etc.</item>
/// <item>Permite separación de preocupaciones transversales de la lógica de negocio</item>
/// </list>
///
/// Responsabilidades del Comportamiento de Validación:
/// <list type="bullet">
/// <item>Ejecuta todos los validadores registrados para el comando/consulta específicos</item>
/// <item>Agrega errores de validación de todos los validadores</item>
/// <item>Devuelve <c>Failure</c> con errores recopilados si la validación falla</item>
/// <item>Permite que el manejador se ejecute si la validación tiene éxito</item>
/// </list>
///
/// Beneficios del Patrón de Diseño:
/// <list type="bullet">
/// <item>Lógica de validación centralizada (no dispersa en manejadores)</item>
/// <item>Enfoque de validación consistente en todos los comandos</item>
/// <item>Los validadores son reutilizables y componibles</item>
/// <item>Separación de reglas de validación de la lógica de negocio</item>
/// <item>Fácil agregar o modificar validación sin tocar manejadores</item>
/// </list>
///
/// Integración con FluentValidation:
/// <list type="bullet">
/// <item>Validadores registrados por tipo de comando/consulta</item>
/// <item>API fluida para reglas de validación legibles</item>
/// <item>Encadenamiento de reglas de validación para escenarios complejos</item>
/// <item>Soporta reglas de validación personalizadas y validadores asíncronos</item>
/// </list>
/// </summary>
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
            _validators.Select(
                v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => f.ErrorMessage)
            .ToList();

        if (failures.Count > 0)
        {
            // Utilizar reflexión para crear resultado de fracaso de tipo genérico correcto
            return CreateFailureResult(failures);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }

    private static TResponse CreateFailureResult(List<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var responseType = typeof(TResponse);

        if (responseType.IsGenericType
            && responseType.GetGenericTypeDefinition() == typeof(Result<>))
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
