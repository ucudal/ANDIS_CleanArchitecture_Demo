namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskPriority</c> es un Objeto de Valor de Dominio (Enumeración) que representa niveles de prioridad de tarea.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación en la capa de dominio
/// - Representa un concepto de negocio principal sin dependencias externas
/// - Asegura seguridad de tipo para niveles de prioridad en todo el sistema
/// - Utilizado por entidad <see cref="TaskItem"/> para categorizar urgencia de tarea
///
/// Niveles de Prioridad (en orden ascendente):
/// - Low (0): Tarea tiene baja urgencia
/// - Medium (1): Tarea tiene urgencia media
/// - High (2): Tarea tiene alta urgencia y debe priorizarse
/// </summary>
public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}
