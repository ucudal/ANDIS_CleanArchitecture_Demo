using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>ITaskReadRepository</c> es la abstracción para consultas de sólo lectura de tareas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Define contrato para recuperación eficiente de datos de tarea
/// - Abstracción de implementación: Interfaz en Núcleo de Aplicación, implementación en Infraestructura
/// - Habilita patrón CQRS —Command Query Responsibility Segregation—
///
/// Beneficios de Separación de Repositorio de Lectura:
/// - Optimizado para rendimiento de lectura (puede usar Dapper, SQL sin procesar, modelos de lectura)
/// - Independiente del repositorio de escritura <see cref="TaskManagement.Domain.Interfaces.ITaskRepository"/>
/// - Habilita el patrón CQRS para separar lecturas de escrituras
/// - Puede desnormalizar datos para eficiencia de consulta
/// - Habilita diferentes estrategias de optimización para lecturas vs escrituras
///
/// Contraste con <see cref="TaskManagement.Domain.Interfaces.ITaskRepository"/>:
/// - <see cref="TaskManagement.Domain.Interfaces.ITaskRepository"/>: Operaciones de escritura (Add, Update, Delete)
/// - <see cref="ITaskReadRepository"/>: Operaciones de lectura optimizadas para consultas
/// - Sigue CQRS: Separa responsabilidad de comando (escritura) de consulta (lectura)
///
/// Estrategia de Implementación:
/// - Puede implementar utilizando Entity Framework <c>AsNoTracking</c> para eficiencia
/// - Puede implementar utilizando Dapper para control SQL detallado
/// - Puede consultar tablas o vistas desnormalizadas optimizadas para lectura
/// - Devuelve DTOs optimizados para casos de uso de consulta específicos
/// </summary>

public interface ITaskReadRepository
{
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
