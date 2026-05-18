using TaskManagement.Domain.Events;

namespace TaskManagement.Domain.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
}
