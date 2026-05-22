// TaskManagement.Domain/Interfaces/ITaskRepository.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// <c>ITaskRepository</c> es la interfaz del repositorio de escritura para agregado <see cref="TaskItem"/>.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación en la capa de dominio
/// - Define el contrato para persistencia de entidades de tarea
/// - Abstracción de implementación: La interfaz pertenece a Dominio, implementación en Infraestructura
/// - Inversión de dependencia: El dominio no depende de tecnología de persistencia específíca
///
/// Beneficios del patrón Repository:
/// - Abstrae lógica de acceso a datos de capas de dominio y aplicación
/// - Habilita intercambio de implementación de base de datos sin cambiar código de dominio
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
/// <c>ITaskReadRepository</c> e la capa de dominio.
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
