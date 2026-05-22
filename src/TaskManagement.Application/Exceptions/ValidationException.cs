namespace TaskManagement.Application.Exceptions;

/// <summary>
/// <c>ValidationException</c> se lanza cuando la validación de entrada falla en la capa de aplicación.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Representa fallos de validación de entrada (no violaciones de regla de negocio de dominio)
/// - Distinta de <see cref="TaskManagement.Domain.Exceptions.DomainException"/> (reglas de negocio) y excepciones técnicas
/// - Permite a manejadores capturar y devolver códigos de estado HTTP apropiados
///
/// Uso:
/// - Lanzada por <see cref="TaskManagement.Application.Behaviors.ValidationBehavior{TRequest, TResponse}"/> cuando falla FluentValidation
/// - Capturada por middleware de manejo de excepciones
/// - Devuelve estado HTTP 400 BadRequest a cliente
///
/// Categorías de Error:
/// - <see cref="ValidationException"/>: Fallos de validación de entrada (esta clase)
/// - <see cref="TaskManagement.Domain.Exceptions.DomainException"/>: Violaciones de reglas de negocio
/// - Otras excepciones: Fallos de infraestructura/técnicos
///
/// Beneficios:
/// - Distincción clara entre diferentes tipos de error
/// - Respuestas de error apropiadas por categoría de error
/// - Preocupaciones separadas: validación vs reglas de negocio
/// - Depuración y registro más fáciles de diferentes tipos de error
/// </summary>
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
