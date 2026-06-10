namespace TaskManagement.Application.Exceptions;

#pragma warning disable CS1570 // XML comment has badly formed XML
/// <summary>
/// <c>ValidationException</c> se lanza cuando la validación de entrada falla en la capa de aplicación.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Representa fallos de validación de entrada -no violaciones de regla de negocio del dominio-</li>
/// <li>Distinta de TaskManagement.Domain.Exceptions.DomainException -reglas de negocio- y excepciones técnicas</li>
/// <li>Permite a manejadores capturar y devolver códigos de estado HTTP apropiados</li>
/// </ul>
///
/// Uso:
/// <ul>
/// <li>Lanzada por TaskManagement.Application.Behaviors.ValidationBehavior< TRequest, TResponse > cuando falla FluentValidation</li>
/// <li>Capturada por middleware de manejo de excepciones</li>
/// <li>Devuelve estado HTTP 400 BadRequest a cliente</li>
/// </ul>
///
/// Categorías de Error:
/// <ul>
/// <li>ValidationException: Fallos de validación de entrada -esta clase-</li>
/// <li>TaskManagement.Domain.Exceptions.DomainException: Violaciones de reglas de negocio</li>
/// <li>Otras excepciones: Fallos de infraestructura/técnicos</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Distincción clara entre diferentes tipos de error</li>
/// <li>Respuestas de error apropiadas por categoría de error</li>
/// <li>Preocupaciones separadas: validación vs reglas de negocio</li>
/// <li>Depuración y registro más fáciles de diferentes tipos de error</li>
/// </ul>
/// </remarks>
#pragma warning restore CS1570 // XML comment has badly formed XML

public sealed class ValidationException : Exception
{
    private static readonly IDictionary<string, string[]> EmptyErrors =
        new Dictionary<string, string[]>();

    public IDictionary<string, string[]> Errors {
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
