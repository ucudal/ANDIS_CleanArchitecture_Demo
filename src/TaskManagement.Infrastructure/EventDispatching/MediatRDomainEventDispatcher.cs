// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.EventDispatching;

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
            // Wrap domain events in INotification for MediatR
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

// Event handler example
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
