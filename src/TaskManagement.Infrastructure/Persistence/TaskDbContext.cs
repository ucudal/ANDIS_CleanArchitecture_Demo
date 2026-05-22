using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Configurations;

namespace TaskManagement.Infrastructure.Persistence;

/// <summary>
/// <c>TaskDbContext</c> es el <see cref="DbContext"/> de Entity Framework Core para gestión de tareas.
///
/// Rol en Clean Architecture:
/// - Parte de la capa de Infraestructura
/// - Implementa interfaz <see cref="IUnitOfWork"/> (definida en Núcleo de Aplicación)
/// - Abstracción de acceso a datos: Encapsula lógica de acceso a base de datos
/// - Mapea entidades de dominio al esquema de base de datos a través de OnModelCreating
///
/// Responsabilidad Dual:
/// - <see cref="DbContext"/> (desde Entity Framework): Gestiona seguimiento de entidades y operaciones de base de datos
/// - Implementación de <see cref="IUnitOfWork"/>: Coordina transacción y persistencia
///
/// Diseño de Arquitectura:
/// - Configurado a través de inyección de dependencias en Program.cs
/// - Utilizado por repositorios y manejadores de comando a través de interfaz <see cref="IUnitOfWork"/>
/// - Implementación de <see cref="IUnitOfWork.SaveChangesAsync"/> delega a Entity Framework
/// - Configuraciones fluidas aplicadas a través de TaskConfiguration
///
/// Beneficios de este enfoque:
/// - Las capas de Aplicación/Dominio dependen solo de interfaz <see cref="IUnitOfWork"/>
/// - Detalles de infraestructura (Entity Framework) aislados en esta clase
/// - Fácil de probar: Simular <see cref="IUnitOfWork"/> sin Entity Framework
/// - Fácil de reemplazar: Intercambiar Entity Framework por ORM diferente
///
/// Entidades Clave:
/// - Tasks: <see cref="DbSet{TEntity}"/> para entidades <see cref="TaskItem"/>
///
/// Configuración:
/// - Aplicada a través de ModelBuilder en OnModelCreating
/// - Delega a TaskConfiguration para mapeos de entidades
/// - Asegura seguridad de tipo y restricciones de base de datos coincidan con reglas de dominio
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
