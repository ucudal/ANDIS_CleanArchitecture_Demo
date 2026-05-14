namespace TaskManagement.Domain.Interfaces;

using TaskManagement.Domain.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
}
