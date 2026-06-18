using TaskManagement.Domain.Events;

namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// <c>IDomainEventDispatcher</c> es la interfaz para publicación de eventos del
/// dominio.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de dominio</li>
/// <li>Define el contrato para envío de eventos del dominio</li>
/// <li>Abstracción de implementación: Interfaz en el dominio, la implementación
/// en la infraestructura</li>
/// <li>Habilita orquestación de envío de eventos después de persistencia exitosa</li>
/// </ul>
///
/// Beneficios del patrón de Envío de Eventos:
/// <ul>
/// <li>Desacopla lógica del dominio de efectos secundarios -correos,
/// notificaciones-</li>
/// <li>Permite desacoplamiento de la infraestructura y facilidad del
/// testing.</li>
/// <li>Soporta procesamiento asíncróno de eventos del dominio</li>
/// <li>Proporciona puntos de extensión sin modificar entidades del dominio</li>
/// <li>Mantiene código del dominio limpio libre de preocupaciones de
/// infraestructura</li>
/// </ul>
///
/// Responsabilidades:
/// <ul>
/// <li>Enviar todos los eventos del dominio elevados por un agregado</li>
/// <li>Ejecutar manejadores de eventos registrados de forma asíncróna</li>
/// <li>Manejar cualquier excepción que ocurra durante procesamiento de
/// eventos</li>
/// <li>Asegurar que todos los eventos se procesen antes de devolver a
/// llamador</li>
/// </ul>
///
/// Nota: Los eventos del dominio representan hechos del pasado que no pueden
/// cambiar. Habilitan a la aplicación a reaccionar a ocurrencias importantes
/// del dominio.
/// </remarks>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
}
