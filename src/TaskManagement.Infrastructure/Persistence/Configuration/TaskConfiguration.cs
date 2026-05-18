// TaskManagement.Infrastructure/Persistence/Configurations/TaskConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

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
