using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Configurations;

namespace TaskManagement.Infrastructure.Persistence;

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
