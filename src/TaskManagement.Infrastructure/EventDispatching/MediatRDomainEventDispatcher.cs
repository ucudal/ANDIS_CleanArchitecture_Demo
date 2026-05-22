// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;

namespace TaskManagement.Infrastructure.EventDispatching;

/// <summary>
/// <c>MediatRDomainEventDispatcher</c> es la implementación de publicación de eventos de dominio utilizando <see cref="MediatR"/>.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte de la capa de Infraestructura</item>
/// <item>Implementa interfaz <see cref="IDomainEventDispatcher"/> (definida en Capa de Aplicación)</item>
/// <item>Preocupación de infraestructura: Detalles de cómo se envían los eventos</item>
/// <item>Desacopla lógica de dominio de mecanismos de manejo de eventos</item>
/// </list>
///
/// Envío de Eventos de Dominio:
/// <list type="bullet">
/// <item>Publica eventos de dominio elevados por agregados</item>
/// <item>Ejecuta manejadores de eventos registrados de forma asíncróna</item>
/// <item>Soporta preocupaciones transversales (correos, notificaciones, registro)</item>
/// <item>Mantiene código de dominio limpio libre de conocimiento de infraestructura</item>
/// </list>
///
/// Integración con <see cref="MediatR"/>:
/// <list type="bullet">
/// <item>Utiliza método <c>Publish</c> para distribución de eventos asíncróna</item>
/// <item>Soporta múltiples manejadores por evento</item>
/// <item>Los manejadores se ejecutan en paralelo a menos que se ordene explícitamente</item>
/// <item>Soporta manejo de transacciones y resiliencia de errores</item>
/// </list>
///
/// Flujo de Eventos:
/// <list type="number">
/// <item>Entidad de dominio eleva evento de dominio (ej. TaskCreatedEvent)</item>
/// <item>Servicio de aplicación persiste cambios de entidades</item>
/// <item>Servicio de aplicación llama a <see cref="IDomainEventDispatcher"/>.<see cref="IDomainEventDispatcher.DispatchAsync"/></item>
/// <item><see cref="MediatRDomainEventDispatcher"/> publica eventos a través de <see cref="MediatR"/></item>
/// <item><see cref="MediatR"/> encuentra y ejecuta todos los <see cref="INotificationHandler{TEvent}"/> registrados</item>
/// <item>Los manejadores ejecutan efectos secundarios (enviar correo, actualizar modelo de lectura, etc.)</item>
/// </list>
///
/// Beneficios:
/// <list type="bullet">
/// <item>Desacopla eventos de manejadores</item>
/// <item>Soporta múltiples manejadores por evento sin coordinación</item>
/// <item>La infraestructura puede ser intercambiada (<see cref="MediatR"/> -&gt; otro bus de eventos)</item>
/// <item>Los manejadores pueden ser agregados/eliminados sin cambios de dominio</item>
/// <item>Habilita procesamiento de eventos asíncróno y retrasado</item>
/// </list>
/// </summary>

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
