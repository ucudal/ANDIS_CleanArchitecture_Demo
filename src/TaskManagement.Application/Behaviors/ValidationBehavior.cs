// TaskManagement.Application/Behaviors/ValidationBehavior.cs
using FluentValidation;
using MediatR;
using TaskManagement.Application.Commands;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Behaviors;

/// <summary>
/// <c>ValidationBehavior</c> es un comportamiento de pipeline de <see cref="MediatR"/> para validar comandos y consultas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>Preocupación transversal: Se aplica a todos los comandos y consultas</li>
/// <li>Comportamiento de pipeline: Intercepta todas las solicitudes de <see cref="MediatR"/> antes de que se ejecuten los manejadores</li>
/// <li>Valida entradas utilizando el marco de trabajo FluentValidation</li>
/// </ul>
///
/// Patrón de pipeline de <see cref="MediatR"/>:
/// <ul>
/// <li>Los comportamientos envuelven el manejo de solicitudes (como middleware en ASP.NET Core)</li>
/// <li>El orden de registro determina el orden de ejecución</li>
/// <li>Puede manejar validación, registro, monitoreo de rendimiento, caché, etc.</li>
/// <li>Permite separación de preocupaciones transversales de la lógica de negocio</li>
/// </ul>
///
/// Responsabilidades del Comportamiento de Validación:
/// <ul>
/// <li>Ejecuta todos los validadores registrados para el comando/consulta específicos</li>
/// <li>Agrega errores de validación de todos los validadores</li>
/// <li>Devuelve <c>Failure</c> con errores recopilados si la validación falla</li>
/// <li>Permite que el manejador se ejecute si la validación tiene éxito</li>
/// </ul>
///
/// Beneficios del Patrón de Diseño:
/// <ul>
/// <li>Lógica de validación centralizada (no dispersa en manejadores)</li>
/// <li>Enfoque de validación consistente en todos los comandos</li>
/// <li>Los validadores son reutilizables y componibles</li>
/// <li>Separación de reglas de validación de la lógica de negocio</li>
/// <li>Fácil agregar o modificar validación sin tocar manejadores</li>
/// </ul>
///
/// Integración con FluentValidation:
/// <ul>
/// <li>Validadores registrados por tipo de comando/consulta</li>
/// <li>API fluida para reglas de validación legibles</li>
/// <li>Encadenamiento de reglas de validación para escenarios complejos</li>
/// <li>Soporta reglas de validación personalizadas y validadores asíncronos</li>
/// </ul>
/// </remarks>
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
