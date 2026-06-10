using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence.Configurations;

namespace TaskManagement.Infrastructure.Persistence;

#pragma warning disable CS1570 // XML comment has badly formed XML
/// <summary>
/// <c>TaskDbContext</c> es el DbContext de Entity Framework Core para gestión de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz IUnitOfWork -definida en Núcleo de Aplicación-</li>
/// <li>Abstracción de acceso a datos: Encapsula lógica de acceso a base de datos</li>
/// <li>Mapea entidades del dominio al esquema de base de datos a través de OnModelCreating</li>
/// </ul>
///
/// Responsabilidad Dual:
/// <ul>
/// <li>DbContext -desde Entity Framework- Gestiona seguimiento de entidades y operaciones de base de datos</li>
/// <li>Implementación de IUnitOfWork: Coordina transacción y persistencia</li>
/// </ul>
///
/// Diseño de Arquitectura:
/// <ul>
/// <li>Configurado a través de inyección de dependencias en Program.cs</li>
/// <li>Utilizado por repositorios y manejadores de comando a través de interfaz IUnitOfWork</li>
/// <li>Implementación de IUnitOfWork.SaveChangesAsync delega a Entity Framework</li>
/// <li>Configuraciones fluidas aplicadas a través de TaskConfiguration</li>
/// </ul>
///
/// Beneficios de este enfoque:
/// <ul>
/// <li>Las capas de Aplicación/Dominio dependen solo de interfaz IUnitOfWork</li>
/// <li>Detalles de infraestructura -Entity Framework- aislados en esta clase</li>
/// <li>Fácil de probar: Simular IUnitOfWork sin Entity Framework</li>
/// <li>Fácil de reemplazar: Intercambiar Entity Framework por ORM diferente</li>
/// </ul>
///
/// Entidades Clave:
/// <ul>
/// <li>Tasks: DbSet< TEntity > para entidades TaskItem</li>
/// </ul>
///
/// Configuración:
/// <ul>
/// <li>Aplicada a través de ModelBuilder en OnModelCreating</li>
/// <li>Delega a TaskConfiguration para mapeos de entidades</li>
/// <li>Asegura seguridad de tipo y restricciones de base de datos coincidan con reglas del dominio</li>
/// </ul>
/// </remarks>
#pragma warning restore CS1570 // XML comment has badly formed XML
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
