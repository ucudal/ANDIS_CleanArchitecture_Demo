// TaskManagement.Domain/Interfaces/ITaskRepository.cs
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>
/// <c>ITaskRepository</c> es la interfaz del repositorio de escritura para
/// el agregado <c>TaskItem</c>.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa de dominio</li>
/// <li>Define el contrato para persistencia de entidades de tarea</li>
/// <li>Abstracción de implementación: La interfaz pertenece al dominio,
/// la implementación a la infraestructura</li>
/// <li>Inversión de dependencia: El dominio define qué abstracción de
/// persistencia necesita</li>
/// </ul>
///
/// Beneficios del patrón Repository:
/// <ul>
/// <li>Abstrae lógica de acceso a datos de capas del dominio y aplicación</li>
/// <li>Habilita intercambio de implementación de base de datos sin cambiar
/// código de aplicación</li>
/// <li>Simplifica pruebas unitarias al permitir implementaciones simuladas</li>
/// <li>Proporciona una API familiar orientada al dominio -no enfocada en
/// consultas-</li>
/// </ul>
///
/// Operaciones:
/// <ul>
/// <li>GetByIdAsync: Recuperar una sola tarea por su ID</li>
/// <li>GetByAssigneeAsync: Encontrar todas las tareas asignadas a un
/// usuario</li>
/// <li>GetOverdueAsync: Encontrar tareas que han vencido y no están
/// completadas</li>
/// <li>AddAsync: Guardar una nueva tarea en persistencia</li>
/// <li>Update: Modificar una tarea existente</li>
/// <li>Delete: Eliminar una tarea de persistencia</li>
/// </ul>
///
/// Nota: Este es el repositorio de escritura. Para operaciones de lectura, ver
/// <c>ITaskReadRepository</c> en la capa de aplicación.
/// Esta separación sigue el patrón CQRS -CommandQueryResponsibilitySeparation-.
/// </remarks>
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetOverdueAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}
