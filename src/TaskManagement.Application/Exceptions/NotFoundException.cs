namespace TaskManagement.Application.Exceptions;

/// <summary>
/// <c>NotFoundException</c> se lanza cuando una entidad solicitada no puede ser encontrada.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Representa fallo en encontrar recurso solicitado</li>
/// <li>Distinta de otros tipos de excepción -validación, dominio, técnico-</li>
/// <li>Permite a manejadores capturar y devolver códigos de estado HTTP apropiados</li>
/// </ul>
///
/// Uso:
/// <ul>
/// <li>Lanzada por manejadores de consulta cuando entidad no se encuentra</li>
/// <li>Lanzada por manejadores de comando cuando agregado requerido falta</li>
/// <li>Capturada por middleware de manejo de excepciones</li>
/// <li>Devuelve estado HTTP 404 NotFound a cliente</li>
/// </ul>
///
/// Categorías de error:
/// <ul>
/// <li><c>NotFoundException</c>: Entidad solicitada no existe -esta clase-</li>
/// <li>ValidationException: Fallos de validación de entrada</li>
/// <li>TaskManagement.Domain.Exceptions.DomainException: Violaciones de reglas de negocio</li>
/// <li>Otras excepciones: Fallos de infraestructura/técnicos</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Semántica clara: Distingue recursos faltantes de otros errores</li>
/// <li>Respuestas HTTP apropiadas: 404 en lugar de 400 o 500</li>
/// <li>Manejo de errores consistente entre manejadores</li>
/// <li>Mejora usabilidad y predictibilidad de API</li>
/// </ul>
/// </remarks>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
