// TaskManagement.Infrastructure/Persistence/Configurations/TaskConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// <c>TaskConfiguration</c> es la configuración de Entity Framework Core para entidad TaskItem.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Traduce entidad del dominio al esquema de base de datos</li>
/// <li>Encapsula todos los detalles de mapeo de Entity Framework</li>
/// <li>Aplicada por TaskDbContext durante creación de modelo</li>
/// </ul>
///
/// Responsabilidades de Configuración:
/// <ul>
/// <li>Mapeo de nombre de tabla y esquema</li>
/// <li>Nombres de columna, tipos y restricciones</li>
/// <li>Definición de clave primaria</li>
/// <li>Creación de índice para optimización de consulta</li>
/// <li>Conversiones de objetos de valor -si es necesario-</li>
/// <li>Restricciones de validación de entidad</li>
/// </ul>
///
/// Beneficios de Diseño:
/// <ul>
/// <li>Centraliza mapeo de base de datos en un lugar</li>
/// <li>Los cambios de modelo del dominio pueden afectar mapeo en aislamiento</li>
/// <li>Fácil de entender requisitos de esquema de base de datos</li>
/// <li>Soporta escenarios de mapeo complejos</li>
/// <li>Sigue convenciones de Entity Framework</li>
/// </ul>
///
/// Separación de responsabilidades:
/// <ul>
/// <li>Capa del dominio: Define reglas de negocio y comportamiento</li>
/// <li>Capa de configuración: Define cómo dominio se mapea a base de datos</li>
/// <li>Capa de infraestructura: Implementa operaciones reales de base de datos</li>
/// <li>Permite cambiar esquema de base de datos sin cambios del dominio</li>
/// </ul>
///
/// Ítems de Configuración Típicos:
/// <ul>
/// <li><c>HasKey</c>: Especificar clave primaria -usualmente propiedad Id-</li>
/// <li><c>Property</c>: Configurar columnas individuales -longitud, precisión, etc.-</li>
/// <li><c>HasIndex</c>: Crear índices para rendimiento</li>
/// <li><c>ToTable</c>: Establecer nombre de tabla si difiere de nombre de clase</li>
/// </ul>
/// </remarks>
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

        // Los eventos del dominio están solo en memoria y no deben ser persistidos por EF.
        builder.Ignore(t => t.DomainEvents);

        builder.HasIndex(t => t.AssignedTo);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate);
        // Filtro de consulta para eliminación suave (si está implementada)
        // builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
