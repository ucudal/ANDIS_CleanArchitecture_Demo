using TaskManagement.Domain.Events;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>IDomainEventDispatcher</c> es la interfaz para publicación de eventos de dominio.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Define el contrato para envío de eventos de dominio
/// - Abstracción de implementación: Interfaz en Aplicación, implementación en Infraestructura
/// - Habilita orquestación de envío de eventos después de persistencia exitosa
///
/// Beneficios del Patrón de Envío de Eventos:
/// - Desacopla lógica de dominio de efectos secundarios (correos, notificaciones)
/// - Permite desacoplamiento de la infraestructura y facilidad del testing.
/// - Soporta procesamiento asíncróno de eventos de dominio
/// - Proporciona puntos de extensión sin modificar entidades de dominio
/// - Mantiene código de dominio limpio libre de preocupaciones de infraestructura
///
/// Responsabilidades:
/// - Enviar todos los eventos de dominio elevados por un agregado
/// - Ejecutar manejadores de eventos registrados de forma asíncróna
/// - Manejar cualquier excepción que ocurra durante procesamiento de eventos
/// - Asegurar que todos los eventos se procesen antes de devolver a llamador
///
/// Nota: Los eventos de dominio representan hechos del pasado que no pueden cambiar.
/// Habilitan a la aplicación a reaccionar a ocurrencias importantes del dominio.
/// </summary>

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
}
