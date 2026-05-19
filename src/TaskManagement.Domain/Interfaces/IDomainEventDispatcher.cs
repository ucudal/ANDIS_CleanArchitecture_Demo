using TaskManagement.Domain.Events;

namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// IDomainEventDispatcher is the interface for publishing domain events.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Domain Layer)
/// - Defines how domain events are communicated to the rest of the system
/// - Implementation abstraction: Interface belongs to Domain, implementation in Infrastructure
/// - Enables decoupled communication between aggregates and application services
///
/// Event Dispatching Pattern Benefits:
/// - Decouples domain logic from side effects (emails, notifications)
/// - Enables eventual consistency across the system
/// - Supports asynchronous processing of domain events
/// - Provides extension points without modifying domain entities
/// - Maintains clean domain code free of infrastructure concerns
///
/// Responsibilities:
/// - Dispatch all domain events raised by an aggregate
/// - Execute registered event handlers asynchronously
/// - Handle any exceptions that occur during event processing
/// - Ensure all events are processed before returning to caller
///
/// Note: Domain events represent facts about the past that cannot be changed.
/// They enable the application to react to important domain occurrences.
/// </summary>

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default);
}
