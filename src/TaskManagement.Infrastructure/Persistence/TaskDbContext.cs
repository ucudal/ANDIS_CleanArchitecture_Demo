using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Configurations;

namespace TaskManagement.Infrastructure.Persistence;

/// <summary>
/// TaskDbContext is the Entity Framework Core DbContext for task management.
///
/// Role in Clean Architecture:
/// - Part of the Infrastructure Layer
/// - Implements IUnitOfWork interface (defined in Application Core)
/// - Data access abstraction: Encapsulates database access logic
/// - Maps domain entities to database schema through OnModelCreating
///
/// Dual Responsibility:
/// - DbContext (from Entity Framework): Manages entity tracking and database operations
/// - IUnitOfWork implementation: Coordinates transaction and persistence
///
/// Architecture Design:
/// - Configured through dependency injection in Program.cs
/// - Used by repositories and command handlers through IUnitOfWork interface
/// - SaveChangesAsync implementation delegates to Entity Framework
/// - Fluent configurations applied through TaskConfiguration
///
/// Benefits of this approach:
/// - Application/Domain layers depend only on IUnitOfWork interface
/// - Infrastructure details (Entity Framework) isolated in this class
/// - Easy to test: Mock IUnitOfWork without Entity Framework
/// - Easy to replace: Swap Entity Framework with different ORM
///
/// Key Entities:
/// - Tasks: DbSet for TaskItem entities
///
/// Configuration:
/// - Applied through ModelBuilder in OnModelCreating
/// - Delegates to TaskConfiguration for entity mappings
/// - Ensures type safety and database constraints match domain rules
/// </summary>
public sealed class TaskDbContext : DbContext, IUnitOfWork
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
