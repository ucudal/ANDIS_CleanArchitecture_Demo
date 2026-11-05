using TaskManagement.Domain.Events;

namespace TaskManagement.Application.Events;

/// <summary>
/// <c>IDomainEventHandler</c> es la reacción de la aplicación a un evento del dominio.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Lo implementan las clases que definen qué hacer cuando ocurre un evento</li>
/// <li>Lo invoca la implementación de IDomainEventDispatcher</li>
/// </ul>
/// </remarks>
/// <typeparam name="TEvent">El tipo de evento del dominio que se maneja.</typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : DomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
