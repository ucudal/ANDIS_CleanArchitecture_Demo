// TaskManagement.Infrastructure/Persistence/Configurations/TaskConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// TaskConfiguration is the Entity Framework Core configuration for TaskItem entity.
///
/// Role in Clean Architecture:
/// - Part of the Infrastructure Layer
/// - Translates domain entity to database schema
/// - Encapsulates all Entity Framework mapping details
/// - Applied by TaskDbContext during model creation
///
/// Configuration Responsibilities:
/// - Table name and schema mapping
/// - Column names, types, and constraints
/// - Primary key definition
/// - Index creation for query optimization
/// - Value object conversions (if needed)
/// - Entity validation constraints
///
/// Design Benefits:
/// - Centralizes database mapping in one place
/// - Domain model changes can affect mapping in isolation
/// - Easy to understand database schema requirements
/// - Supports complex mapping scenarios
/// - Follows Entity Framework conventions
///
/// Separation of Concerns:
/// - Domain layer: Defines business rules and behavior
/// - Configuration layer: Defines how domain maps to database
/// - Infrastructure layer: Implements actual database operations
/// - Enables changing database schema without domain changes
///
/// Typical Configuration Items:
/// - HasKey: Specify primary key (usually Id property)
/// - Property: Configure individual columns (length, precision, etc.)
/// - HasIndex: Create indexes for performance
/// - ToTable: Set table name if different from class name
/// </summary>

public sealed class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Domain events are in-memory only and should not be persisted by EF.
        builder.Ignore(t => t.DomainEvents);

        builder.HasIndex(t => t.AssignedTo);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate);
        // Query filter for soft delete (if implemented)
        // builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
