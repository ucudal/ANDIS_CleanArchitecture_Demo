// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.EventDispatching;

/// <summary>
/// MediatRDomainEventDispatcher is the implementation of domain event publishing using MediatR.
///
/// Role in Clean Architecture:
/// - Part of the Infrastructure Layer
/// - Implements IDomainEventDispatcher interface (defined in Domain Layer)
/// - Infrastructure concern: Details of how events are dispatched
/// - Decouples domain logic from event handling mechanisms
///
/// Domain Event Dispatching:
/// - Publishes domain events raised by aggregates
/// - Executes registered event handlers asynchronously
/// - Supports cross-cutting concerns (emails, notifications, logging)
/// - Maintains clean domain code free of infrastructure knowledge
///
/// MediatR Integration:
/// - Uses MediatR.Publish for async event distribution
/// - Supports multiple handlers per event
/// - Handlers execute in parallel unless explicitly ordered
/// - Supports transaction handling and error resilience
///
/// Event Flow:
/// 1. Domain entity raises domain event (e.g., TaskCreatedEvent)
/// 2. Application service persists entity changes
/// 3. Application service calls IDomainEventDispatcher.DispatchAsync
/// 4. MediatRDomainEventDispatcher publishes events via MediatR
/// 5. MediatR finds and executes all registered INotificationHandler&lt;TEvent&gt;
/// 6. Handlers execute side effects (send email, update read model, etc.)
///
/// Benefits:
/// - Decouples events from handlers
/// - Supports multiple handlers per event without coordination
/// - Infrastructure can be swapped (MediatR -> other event bus)
/// - Handlers can be added/removed without domain changes
/// - Enables asynchronous and delayed event processing
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
