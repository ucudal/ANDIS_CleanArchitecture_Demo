using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Configurations;

namespace TaskManagement.Infrastructure.Persistence;

/// <summary>
/// <c>TaskDbContext</c> es el <see cref="DbContext"/> de Entity Framework Core para gestión de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz <see cref="IUnitOfWork"/> (definida en Núcleo de Aplicación)</li>
/// <li>Abstracción de acceso a datos: Encapsula lógica de acceso a base de datos</li>
/// <li>Mapea entidades de dominio al esquema de base de datos a través de OnModelCreating</li>
/// </ul>
///
/// Responsabilidad Dual:
/// <ul>
/// <li><see cref="DbContext"/> (desde Entity Framework): Gestiona seguimiento de entidades y operaciones de base de datos</li>
/// <li>Implementación de <see cref="IUnitOfWork"/>: Coordina transacción y persistencia</li>
/// </ul>
///
/// Diseño de Arquitectura:
/// <ul>
/// <li>Configurado a través de inyección de dependencias en Program.cs</li>
/// <li>Utilizado por repositorios y manejadores de comando a través de interfaz <see cref="IUnitOfWork"/></li>
/// <li>Implementación de <see cref="IUnitOfWork.SaveChangesAsync"/> delega a Entity Framework</li>
/// <li>Configuraciones fluidas aplicadas a través de TaskConfiguration</li>
/// </ul>
///
/// Beneficios de este enfoque:
/// <ul>
/// <li>Las capas de Aplicación/Dominio dependen solo de interfaz <see cref="IUnitOfWork"/></li>
/// <li>Detalles de infraestructura (Entity Framework) aislados en esta clase</li>
/// <li>Fácil de probar: Simular <see cref="IUnitOfWork"/> sin Entity Framework</li>
/// <li>Fácil de reemplazar: Intercambiar Entity Framework por ORM diferente</li>
/// </ul>
///
/// Entidades Clave:
/// <ul>
/// <li>Tasks: <see cref="DbSet{TEntity}"/> para entidades <see cref="TaskItem"/></li>
/// </ul>
///
/// Configuración:
/// <ul>
/// <li>Aplicada a través de ModelBuilder en OnModelCreating</li>
/// <li>Delega a TaskConfiguration para mapeos de entidades</li>
/// <li>Asegura seguridad de tipo y restricciones de base de datos coincidan con reglas de dominio</li>
/// </ul>
/// </remarks>
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
