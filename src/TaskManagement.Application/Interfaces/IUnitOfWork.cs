namespace TaskManagement.Application.Interfaces;

/// <summary>
/// IUnitOfWork is the abstraction for coordinating persistence of entity changes.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Defines contract for saving changes to persistence store
/// - Implementation abstraction: Interface in Application Core, implementation in Infrastructure
/// - Dependency inversion: Application depends on interface, not concrete persistence technology
///
/// Unit of Work Pattern Benefits:
/// - Coordinates multiple repository changes into single atomic transaction
/// - Ensures all-or-nothing persistence: either all changes succeed or all rollback
/// - Maintains consistency across multiple domain aggregates
/// - Abstracts database transaction management from application code
///
/// Typical Usage:
/// - After modifying domain entities through repositories
/// - Before executing domain events that depend on persistence success
/// - Application services call SaveChangesAsync after coordinating domain operations
///
/// In this implementation:
/// - TaskDbContext implements both DbContext and IUnitOfWork
/// - SaveChangesAsync wraps Entity Framework's SaveChangesAsync
/// - Provides cancellation token support for long-running operations
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
