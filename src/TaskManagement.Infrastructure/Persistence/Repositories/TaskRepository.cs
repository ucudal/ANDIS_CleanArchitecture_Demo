// TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// <c>TaskRepository</c> es el repositorio de escritura para entidades TaskItem.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa interfaz ITaskRepository -definida en Capa del dominio-</li>
/// <li>Implementación de acceso a datos: Traduce operaciones del dominio a consultas de base de datos</li>
/// <li>Inversión de dependencia: El dominio depende de interfaz, no de esta implementación</li>
/// </ul>
///
/// Beneficios del patrón Repository:
/// <ul>
/// <li>Abstrae detalles de Entity Framework de capas del dominio y aplicación</li>
/// <li>Proporciona API orientada al dominio -no enfocada en consultas-</li>
/// <li>Habilita pruebas unitarias a través de implementaciones simuladas</li>
/// <li>Permite intercambiar tecnología de base de datos sin cambiar código de aplicación</li>
/// </ul>
///
/// Operaciones de Escritura vs Operaciones de Lectura:
/// <ul>
/// <li>Este repositorio: Operaciones de escritura -Add, Update, Delete-</li>
/// <li>TaskReadRepository: Operaciones de lectura -consultas optimizadas-</li>
/// <li>Separación sigue CQRS: Estrategias diferentes para lecturas y escrituras</li>
/// </ul>
///
/// Detalles de Implementación:
/// <ul>
/// <li>Utiliza Entity Framework Core para acceso a base de datos</li>
/// <li>Usa <c>AsNoTracking</c> para consultas de sólo lectura -GetByIdAsync, GetByAssigneeAsync, GetOverdueAsync-</li>
/// <li>Seguimiento habilitado para operaciones de escritura -AddAsync, Update, Delete-</li>
/// <li>Devuelve colecciones inmutables -IReadOnlyList- para prevenir modificaciones de cliente</li>
/// </ul>
///
/// Métodos:
/// <ul>
/// <li>GetByIdAsync: Encontrar tarea por ID -lectura-</li>
/// <li>GetByAssigneeAsync: Encontrar tareas asignadas a usuario -lectura-</li>
/// <li>GetOverdueAsync: Encontrar tareas incompletas vencidas -lectura-</li>
/// <li>AddAsync: Persistir nueva entidad de tarea -escritura-</li>
/// <li>Update: Marcar entidad existente para actualización -escritura-</li>
/// <li>Delete: Marcar entidad existente para eliminación -escritura-</li>
/// </ul>
/// </remarks>
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
