// TaskManagement.Infrastructure/Persistence/Configurations/TaskConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// <c>TaskConfiguration</c> es la configuración de Entity Framework Core para entidad <see cref="TaskItem"/>.
///
/// Rol en Clean Architecture:
/// - Parte de la capa de Infraestructura
/// - Traduce entidad de dominio al esquema de base de datos
/// - Encapsula todos los detalles de mapeo de Entity Framework
/// - Aplicada por <see cref="TaskDbContext"/> durante creación de modelo
///
/// Responsabilidades de Configuración:
/// - Mapeo de nombre de tabla y esquema
/// - Nombres de columna, tipos y restricciones
/// - Definición de clave primaria
/// - Creación de índice para optimización de consulta
/// - Conversiones de objetos de valor (si es necesario)
/// - Restricciones de validación de entidad
///
/// Beneficios de Diseño:
/// - Centraliza mapeo de base de datos en un lugar
/// - Los cambios de modelo de dominio pueden afectar mapeo en aislamiento
/// - Fácil de entender requisitos de esquema de base de datos
/// - Soporta escenarios de mapeo complejos
/// - Sigue convenciones de Entity Framework
///
/// Separación de Responsabilidades:
/// - Capa de dominio: Define reglas de negocio y comportamiento
/// - Capa de configuración: Define cómo dominio se mapea a base de datos
/// - Capa de infraestructura: Implementa operaciones reales de base de datos
/// - Permite cambiar esquema de base de datos sin cambios de dominio
///
/// Ítems de Configuración Típicos:
/// - <c>HasKey</c>: Especificar clave primaria (usualmente propiedad Id)
/// - <c>Property</c>: Configurar columnas individuales (longitud, precisión, etc.)
/// - <c>HasIndex</c>: Crear índices para rendimiento
/// - <c>ToTable</c>: Establecer nombre de tabla si difiere de nombre de clase
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

        // Los eventos de dominio están solo en memoria y no deben ser persistidos por EF.
        builder.Ignore(t => t.DomainEvents);

        builder.HasIndex(t => t.AssignedTo);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate);
        // Filtro de consulta para eliminación suave (si está implementada)
        // builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
