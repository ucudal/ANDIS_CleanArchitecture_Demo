// TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// <c>TaskRepository</c> es el repositorio de escritura para entidades <see cref="TaskItem"/>.
///
/// Rol en Clean Architecture:
/// - Parte de la capa de Infraestructura
/// - Implementa interfaz <see cref="ITaskRepository"/> (definida en Capa de Dominio)
/// - Implementación de acceso a datos: Traduce operaciones de dominio a consultas de base de datos
/// - Inversión de dependencia: El dominio depende de interfaz, no de esta implementación
///
/// Beneficios del patrón Repository:
/// - Abstrae detalles de Entity Framework de capas de dominio y aplicación
/// - Proporciona API orientada al dominio (no enfocada en consultas)
/// - Habilita pruebas unitarias a través de implementaciones simuladas
/// - Permite intercambiar tecnología de base de datos sin cambiar código de aplicación
///
/// Operaciones de Escritura vs Operaciones de Lectura:
/// - Este repositorio: Operaciones de escritura (Add, Update, Delete)
/// - <see cref="TaskReadRepository"/>: Operaciones de lectura (consultas optimizadas)
/// - Separación sigue CQRS: Estrategias diferentes para lecturas y escrituras
///
/// Detalles de Implementación:
/// - Utiliza Entity Framework Core para acceso a base de datos
/// - Usa <c>AsNoTracking</c> para consultas de sólo lectura (GetByIdAsync, GetByAssigneeAsync, GetOverdueAsync)
/// - Seguimiento habilitado para operaciones de escritura (AddAsync, Update, Delete)
/// - Devuelve colecciones inmutables (IReadOnlyList) para prevenir modificaciones de cliente
///
/// Métodos:
/// - GetByIdAsync: Encontrar tarea por ID (lectura)
/// - GetByAssigneeAsync: Encontrar tareas asignadas a usuario (lectura)
/// - GetOverdueAsync: Encontrar tareas incompletas vencidas (lectura)
/// - AddAsync: Persistir nueva entidad de tarea (escritura)
/// - Update: Marcar entidad existente para actualización (escritura)
/// - Delete: Marcar entidad existente para eliminación (escritura)
/// </summary>
public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _dbContext;
    public TaskRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking() // Read-only query
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken).ConfigureAwait(false);
    }
    public async Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.AssignedTo == userId)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task<IReadOnlyList<TaskItem>> GetOverdueAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .Where(t => t.DueDate < DateTime.UtcNow
                && t.Status != TaskManagement.Domain.Entities.TaskStatus.Completed)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
    public async Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Tasks.AddAsync(task, cancellationToken).ConfigureAwait(false);
    }
    public void Update(TaskItem task)
    {
        _dbContext.Tasks.Update(task);
    }
    public void Delete(TaskItem task)
    {
        _dbContext.Tasks.Remove(task);
    }
}
