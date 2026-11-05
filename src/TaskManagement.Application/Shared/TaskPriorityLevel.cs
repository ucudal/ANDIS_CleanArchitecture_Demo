namespace TaskManagement.Application.Common;

/// <summary>
/// <c>TaskPriorityLevel</c> es la prioridad de una tarea tal como la ven los DTO de los casos de
/// uso. Evita que la interfaz de usuario dependa del enum TaskPriority del dominio.
/// </summary>
public enum TaskPriorityLevel
{
    Low = 0,
    Medium = 1,
    High = 2
}
