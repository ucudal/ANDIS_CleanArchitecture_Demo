// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;

namespace TaskManagement.Infrastructure.EventDispatching;

/// <summary>
/// <c>MediatRDomainEventDispatcher</c> es la implementación de publicación de eventos de dominio utilizando <see cref="MediatR"/>.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz <see cref="IDomainEventDispatcher"/> (definida en Capa de Aplicación)</li>
/// <li>Preocupación de infraestructura: Detalles de cómo se envían los eventos</li>
/// <li>Desacopla lógica de dominio de mecanismos de manejo de eventos</li>
/// </ul>
///
/// Envío de Eventos de Dominio:
/// <ul>
/// <li>Publica eventos de dominio elevados por agregados</li>
/// <li>Ejecuta manejadores de eventos registrados de forma asíncróna</li>
/// <li>Soporta preocupaciones transversales (correos, notificaciones, registro)</li>
/// <li>Mantiene código de dominio limpio libre de conocimiento de infraestructura</li>
/// </ul>
///
/// Integración con <see cref="MediatR"/>:
/// <ul>
/// <li>Utiliza método <c>Publish</c> para distribución de eventos asíncróna</li>
/// <li>Soporta múltiples manejadores por evento</li>
/// <li>Los manejadores se ejecutan en paralelo a menos que se ordene explícitamente</li>
/// <li>Soporta manejo de transacciones y resiliencia de errores</li>
/// </ul>
///
/// Flujo de Eventos:
/// <ol>
/// <li>Entidad de dominio eleva evento de dominio (ej. TaskCreatedEvent)</li>
/// <li>Servicio de aplicación persiste cambios de entidades</li>
/// <li>Servicio de aplicación llama a <see cref="IDomainEventDispatcher"/>.<see cref="IDomainEventDispatcher.DispatchAsync"/></li>
/// <li><see cref="MediatRDomainEventDispatcher"/> publica eventos a través de <see cref="MediatR"/></li>
/// <li><see cref="MediatR"/> encuentra y ejecuta todos los <see cref="INotificationHandler{TEvent}"/> registrados</li>
/// <li>Los manejadores ejecutan efectos secundarios (enviar correo, actualizar modelo de lectura, etc.)</li>
/// </ol>
///
/// Beneficios:
/// <ul>
/// <li>Desacopla eventos de manejadores</li>
/// <li>Soporta múltiples manejadores por evento sin coordinación</li>
/// <li>La infraestructura puede ser intercambiada (<see cref="MediatR"/> -&gt; otro bus de eventos)</li>
/// <li>Los manejadores pueden ser agregados/eliminados sin cambios de dominio</li>
/// <li>Habilita procesamiento de eventos asíncróno y retrasado</li>
/// </ul>
/// </remarks>
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
            // Envolver eventos de dominio en INotification para MediatR
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
    public T Event
    {
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
