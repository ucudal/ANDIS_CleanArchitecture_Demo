// TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs
namespace TaskManagement.Infrastructure.EventDispatching;

using MediatR;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Interfaces;
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
        foreach (var domainEvent in events)
        {
            // Wrap domain events in INotification for MediatR
            var notification = WrapEvent(domainEvent);
            await _mediator.Publish(notification, cancellationToken);
        }
    }
    private INotification WrapEvent(DomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventWrapper<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
    }
}
public sealed class DomainEventWrapper<T> : INotification where T : DomainEvent
{
    public T Event { get; }
    public DomainEventWrapper(T @event) => Event = @event;
}
// Event handler example
public sealed class TaskCompletedNotificationHandler
    : INotificationHandler<DomainEventWrapper<TaskCompletedEvent>>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<TaskCompletedNotificationHandler> _logger;
    public TaskCompletedNotificationHandler(
        IEmailService emailService,
        ILogger<TaskCompletedNotificationHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public async Task Handle(
        DomainEventWrapper<TaskCompletedEvent> notification,
        CancellationToken cancellationToken)
    {
        var @event = notification.Event;

        _logger.LogInformation(
            "Task {TaskId} completed at {CompletedAt}",
            @event.TaskId,
            @event.CompletedAt);
        // Send notification email, update analytics, etc.
        // await _emailService.SendTaskCompletedNotificationAsync(@event.TaskId);
    }
}