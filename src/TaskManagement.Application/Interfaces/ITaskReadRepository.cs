using TaskManagement.Application.Queries;

namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>ITaskReadRepository</c> es la abstracción para consultas de sólo lectura de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Define contrato para recuperación eficiente de datos de tarea</li>
/// <li>Abstracción de implementación: Interfaz en Núcleo de Aplicación, implementación en Infraestructura</li>
/// <li>Habilita patrón CQRS -CommandQueryResponsibilitySeparation-</li>
/// </ul>
///
/// Beneficios de Separación de Repositorio de Lectura:
/// <ul>
/// <li>Optimizado para rendimiento de lectura -puede usar Dapper, SQL sin procesar, modelos de lectura-</li>
/// <li>Independiente del repositorio de escritura <see
/// cref="TaskManagement.Application.Interfaces.ITaskRepository"/></li>
/// <li>Habilita el patrón CQRS para separar lecturas de escrituras</li>
/// <li>Puede desnormalizar datos para eficiencia de consulta</li>
/// <li>Habilita diferentes estrategias de optimización para lecturas vs escrituras</li>
/// </ul>
///
/// Contraste con TaskManagement.Application.Interfaces.ITaskRepository:
/// <ul>
/// <li>TaskManagement.Application.Interfaces.ITaskRepository: Operaciones de escritura -Add, Update, Delete-</li>
/// <li>ITaskReadRepository: Operaciones de lectura optimizadas para consultas</li>
/// <li>Sigue CQRS: Separa responsabilidad de comando -escritura- de consulta -lectura-</li>
/// </ul>
///
/// Estrategia de Implementación:
/// <ul>
/// <li>Puede implementar utilizando Entity Framework <c>AsNoTracking</c> para eficiencia</li>
/// <li>Puede implementar utilizando Dapper para control SQL detallado</li>
/// <li>Puede consultar tablas o vistas desnormalizadas optimizadas para lectura</li>
/// <li>Devuelve DTOs optimizados para casos de uso de consulta específicos</li>
/// </ul>
/// </remarks>
public interface ITaskReadRepository
{
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
