// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.EventDispatching;

#pragma warning disable CS1570 // XML comment has badly formed XML
/// <summary>
/// <c>MediatRDomainEventDispatcher</c> es la implementación de publicación de eventos del dominio utilizando <a href="https://mediatr.io">MediatR</a>.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz IDomainEventDispatcher -definida en Capa de Dominio-</li>
/// <li>Preocupación de infraestructura: Detalles de cómo se envían los eventos</li>
/// <li>Desacopla lógica del dominio de mecanismos de manejo de eventos</li>
/// </ul>
///
/// Envío de Eventos del dominio:
/// <ul>
/// <li>Publica eventos del dominio elevados por agregados</li>
/// <li>Ejecuta manejadores de eventos registrados de forma asíncróna</li>
/// <li>Soporta preocupaciones transversales -correos, notificaciones, registro-</li>
/// <li>Mantiene código del dominio limpio libre de conocimiento de infraestructura</li>
/// </ul>
///
/// Integración con <a href="https://mediatr.io">MediatR</a>:
/// <ul>
/// <li>Utiliza método <c>Publish</c> para distribución de eventos asíncróna</li>
/// <li>Soporta múltiples manejadores por evento</li>
/// <li>Los manejadores se ejecutan en paralelo a menos que se ordene explícitamente</li>
/// <li>Soporta manejo de transacciones y resiliencia de errores</li>
/// </ul>
///
/// Flujo de Eventos:
/// <ol>
/// <li>Entidad del dominio eleva evento del dominio -ej. TaskCreatedEvent-</li>
/// <li>Servicio de aplicación persiste cambios de entidades</li>
/// <li>Servicio de aplicación llama a IDomainEventDispatcher.DispatchAsync</li>
/// <li>MediatRDomainEventDispatcher publica eventos a través de <a href="https://mediatr.io">MediatR</a></li>
/// <li><a href="https://mediatr.io">MediatR</a> encuentra y ejecuta todos los INotificationHandler< TEvent > registrados</li>
/// <li>Los manejadores ejecutan efectos secundarios -enviar correo, actualizar modelo de lectura, etc.-</li>
/// </ol>
///
/// Beneficios:
/// <ul>
/// <li>Desacopla eventos de manejadores</li>
/// <li>Soporta múltiples manejadores por evento sin coordinación</li>
/// <li>La infraestructura puede ser intercambiada -<a href="https://mediatr.io">MediatR</a> -&gt; otro bus de eventos-</li>
/// <li>Los manejadores pueden ser agregados/eliminados sin cambios del dominio</li>
/// <li>Habilita procesamiento de eventos asíncróno y retrasado</li>
/// </ul>
/// </remarks>
#pragma warning restore CS1570 // XML comment has badly formed XML
public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(
        IReadOnlyCollection<DomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var domainEvent in events)
        {
            // Envolver eventos del dominio en INotification para MediatR
            var notification = WrapEvent(domainEvent);
            await _mediator.Publish(notification, cancellationToken).ConfigureAwait(false);
        }
    }

    private static INotification WrapEvent(DomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventWrapper<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
    }
}

public sealed class DomainEventWrapper<T> : INotification where T : DomainEvent
{
    public T Event {
        get;
    }

    public DomainEventWrapper(T @event) => Event = @event;
}

// Ejemplo de manejador de eventos
public sealed class TaskCompletedNotificationHandler
    : INotificationHandler<DomainEventWrapper<TaskCompletedEvent>>
{
    private static readonly Action<ILogger, Guid, DateTime, Exception?> LogTaskCompleted =
        LoggerMessage.Define<Guid, DateTime>(
            LogLevel.Information,
            new EventId(1001, nameof(TaskCompletedNotificationHandler)),
            "Task {TaskId} completed at {CompletedAt}");

    private readonly IEmailService _emailService;
    private readonly ILogger<TaskCompletedNotificationHandler> _logger;

    public TaskCompletedNotificationHandler(
        IEmailService emailService,
        ILogger<TaskCompletedNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public Task Handle(
        DomainEventWrapper<TaskCompletedEvent> notification,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var @event = notification.Event;

        LogTaskCompleted(_logger, @event.TaskId, @event.CompletedAt, null);
        return Task.CompletedTask;
        // Send notification email, update analytics, etc.
        // await _emailService.SendTaskCompletedNotificationAsync(@event.TaskId);
    }
}
