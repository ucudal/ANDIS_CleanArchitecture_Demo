namespace TaskManagement.Domain.Exceptions;

/// <summary>
/// <c>DomainException</c> es la clase de excepción base para todos los errores de capa de dominio.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa de dominio</li>
/// <li>Representa violaciones de reglas de negocio e incumplimientos de invariantes de dominio</li>
/// <li>Distingue errores de dominio de errores técnicos de infraestructura</li>
/// <li>Permite a capas de aplicación manejar fallos de lógica de negocio apropiadamente</li>
/// </ul>
///
/// Uso:
/// <ul>
/// <li>Se lanza cuando los invariantes de dominio se violan</li>
/// <li>Se lanza cuando las reglas de negocio no se satisfacen</li>
/// <li>Capturada y manejada por capa de servicio de aplicación</li>
/// <li>Nunca debe ser lanzada por problemas técnicos/infraestructura</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Separación clara entre errores de negocio y técnicos</li>
/// <li>Contratos explícitos sobre qué puede salir mal en lógica de dominio</li>
/// <li>Habilita manejo de errores adecuado en nivel de aplicación</li>
/// </ul>
/// </remarks>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }

    public DomainException()
    {
    }
}
