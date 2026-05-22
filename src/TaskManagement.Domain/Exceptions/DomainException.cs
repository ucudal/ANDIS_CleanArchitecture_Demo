namespace TaskManagement.Domain.Exceptions;

/// <summary>
/// <c>DomainException</c> es la clase de excepción base para todos los errores de capa de dominio.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte del core de la aplicación en la capa de dominio</item>
/// <item>Representa violaciones de reglas de negocio e incumplimientos de invariantes de dominio</item>
/// <item>Distingue errores de dominio de errores técnicos de infraestructura</item>
/// <item>Permite a capas de aplicación manejar fallos de lógica de negocio apropiadamente</item>
/// </list>
///
/// Uso:
/// <list type="bullet">
/// <item>Se lanza cuando los invariantes de dominio se violan</item>
/// <item>Se lanza cuando las reglas de negocio no se satisfacen</item>
/// <item>Capturada y manejada por capa de servicio de aplicación</item>
/// <item>Nunca debe ser lanzada por problemas técnicos/infraestructura</item>
/// </list>
///
/// Beneficios:
/// <list type="bullet">
/// <item>Separación clara entre errores de negocio y técnicos</item>
/// <item>Contratos explícitos sobre qué puede salir mal en lógica de dominio</item>
/// <item>Habilita manejo de errores adecuado en nivel de aplicación</item>
/// </list>
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }

    public DomainException()
    {
    }
}
