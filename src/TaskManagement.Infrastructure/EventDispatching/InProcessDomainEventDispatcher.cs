// TaskManagement.Infrastructure/EventDispatching/InProcessDomainEventDispatcher.cs
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Events;
using TaskManagement.Application.Ports;
using TaskManagement.Domain.Events;

namespace TaskManagement.Infrastructure.EventDispatching;

/// <summary>
/// <c>InProcessDomainEventDispatcher</c> entrega cada evento del dominio a sus manejadores, en el
/// mismo proceso y en el mismo alcance de la solicitud.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa IDomainEventDispatcher -definida en la capa de aplicación-</li>
/// <li>Encapsula el uso del contenedor de inyección de dependencias para encontrar los
/// manejadores: la capa de aplicación no lo conoce</li>
/// <li>Podría reemplazarse por otra implementación, por ejemplo, una cola de mensajes, sin cambiar
/// los casos de uso</li>
/// </ul>
///
/// Los manejadores implementan IDomainEventHandler y se registran en Program.cs.
/// </remarks>
public sealed class InProcessDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InProcessDomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(
        IReadOnlyCollection<DomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var domainEvent in events)
        {
            // El tipo concreto del evento se conoce recién en tiempo de ejecución.
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<DomainEvent>.HandleAsync))!;

            foreach (var handler in _serviceProvider.GetServices(handlerType))
            {
                await ((Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!).ConfigureAwait(false);
            }
        }
    }
}
