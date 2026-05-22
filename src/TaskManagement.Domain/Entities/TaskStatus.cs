namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskStatus</c> es un Objeto de Valor de Dominio (Enumeración) que representa posibles estados de una tarea.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación en la capa de dominio
/// - Representa conceptos de negocio principales sin dependencias externas
/// - Asegura seguridad de tipo para estados de tarea en todo el sistema
/// - Utilizado por entidad <see cref="TaskItem"/> para aplicar transiciones de estado válidas
///
/// Estados Válidos:
/// - Todo (0): Estado inicial para nuevas tareas
/// - InProgress (1): La tarea se está trabajando actualmente
/// - Completed (2): La tarea ha sido finalizada
/// </summary>
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Completed = 2
}
