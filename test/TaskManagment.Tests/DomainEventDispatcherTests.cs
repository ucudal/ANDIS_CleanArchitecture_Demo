using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Events;
using TaskManagement.Domain.Events;
using TaskManagement.Infrastructure.EventDispatching;

namespace TaskManagment.Tests;

/// <summary>
/// <c>DomainEventDispatcherTests</c> verifica que InProcessDomainEventDispatcher entregue cada
/// evento solo a los manejadores de su tipo.
/// </summary>
[TestFixture]
public class DomainEventDispatcherTests
{
    private sealed class SpyHandler<TEvent> : IDomainEventSubscriber<TEvent>
        where TEvent : DomainEvent
    {
        public List<TEvent> Received { get; } = [];

        public Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default)
        {
            Received.Add(domainEvent);
            return Task.CompletedTask;
        }
    }

    [Test]
    public async Task DispatchAsync_DeliversEachEventToTheHandlersOfItsType()
    {
        var completedHandler = new SpyHandler<TaskCompletedEvent>();
        var createdHandler = new SpyHandler<TaskCreatedEvent>();
        using var services = new ServiceCollection()
            .AddSingleton<IDomainEventSubscriber<TaskCompletedEvent>>(completedHandler)
            .AddSingleton<IDomainEventSubscriber<TaskCreatedEvent>>(createdHandler)
            .BuildServiceProvider();
        var completedEvent = new TaskCompletedEvent(Guid.NewGuid(), DateTime.UtcNow);
        var dispatcher = new InProcessDomainEventDispatcher(services);

        await dispatcher.DispatchAsync([completedEvent]);

        Assert.That(completedHandler.Received.Single(), Is.SameAs(completedEvent));
        Assert.That(createdHandler.Received, Is.Empty);
    }

    [Test]
    public void DispatchAsync_WithEventWithoutHandlers_DoesNothing()
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        var dispatcher = new InProcessDomainEventDispatcher(services);

        Assert.DoesNotThrowAsync(
            () => dispatcher.DispatchAsync([new TaskCreatedEvent(Guid.NewGuid(), "Title", Guid.NewGuid())]));
    }
}
