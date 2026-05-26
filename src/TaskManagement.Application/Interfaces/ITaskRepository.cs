// TaskManagement.Application/Interfaces/ITaskRepository.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>ITaskRepository</c> es la interfaz del repositorio de escritura para agregado <see cref="TaskItem"/>.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación en la capa de aplicación
/// - Define el contrato para persistencia de entidades de tarea
/// - Abstracción de implementación: La interfaz pertenece a Aplicación, implementación en Infraestructura
/// - Inversión de dependencia: La aplicación define qué abstracción de persistencia necesita
///
/// Beneficios del patrón Repository:
/// - Abstrae lógica de acceso a datos de capas de dominio y aplicación
/// - Habilita intercambio de implementación de base de datos sin cambiar código de aplicación
/// - Simplifica pruebas unitarias al permitir implementaciones simuladas
/// - Proporciona una API familiar orientada al dominio (no enfocada en consultas)
///
/// Operaciones:
/// - GetByIdAsync: Recuperar una sola tarea por su ID
/// - GetByAssigneeAsync: Encontrar todas las tareas asignadas a un usuario
/// - GetOverdueAsync: Encontrar tareas que han vencido y no están completadas
/// - AddAsync: Guardar una nueva tarea en persistencia
/// - Update: Modificar una tarea existente
/// - Delete: Eliminar una tarea de persistencia
///
/// Nota: Este es el repositorio de escritura. Para operaciones de lectura, ver
/// <c>ITaskReadRepository</c> en la capa de aplicación.
/// Esta separación sigue el patrón CQRS —Command Query Responsibility Segregation—.
/// </summary>
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}
