namespace TaskManagement.Application.Exceptions;

/// <summary>
/// <c>NotFoundException</c> se lanza cuando una entidad solicitada no puede ser encontrada.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Representa fallo en encontrar recurso solicitado
/// - Distinta de otros tipos de excepción (validación, dominio, técnico)
/// - Permite a manejadores capturar y devolver códigos de estado HTTP apropiados
///
/// Uso:
/// - Lanzada por manejadores de consulta cuando entidad no se encuentra
/// - Lanzada por manejadores de comando cuando agregado requerido falta
/// - Capturada por middleware de manejo de excepciones
/// - Devuelve estado HTTP 404 NotFound a cliente
///
/// Categorías de Error:
/// - <c>NotFoundException</c>: Entidad solicitada no existe (esta clase)
/// - <see cref="ValidationException"/>: Fallos de validación de entrada
/// - <see cref="TaskManagement.Domain.Exceptions.DomainException"/>: Violaciones de reglas de negocio
/// - Otras excepciones: Fallos de infraestructura/técnicos
///
/// Beneficios:
/// - Semántica clara: Distingue recursos faltantes de otros errores
/// - Respuestas HTTP apropiadas: 404 en lugar de 400 o 500
/// - Manejo de errores consistente entre manejadores
/// - Mejora usabilidad y predictibilidad de API
/// </summary>
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
