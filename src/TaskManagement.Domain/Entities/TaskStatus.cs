namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskStatus</c> es un <a href="https://github.com/ucudal/ANDIS_Conceptos/blob/2e0fbbe729ea47ee6029405a8435c918c9e7c4e6/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Value_Object.md">objeto valor</a> del dominio que representa posibles estados de una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa de dominio</li>
/// <li>Representa conceptos de negocio principales sin dependencias externas</li>
/// <li>Asegura seguridad de tipo para estados de tarea en todo el sistema</li>
/// <li>Utilizado por entidad <see cref="TaskItem"/> para aplicar transiciones de estado válidas</li>
/// </ul>
///
/// Estados Válidos:
/// <ul>
/// <li>Todo (0): Estado inicial para nuevas tareas</li>
/// <li>InProgress (1): La tarea se está trabajando actualmente</li>
/// <li>Completed (2): La tarea ha sido finalizada</li>
/// </ul>
/// </remarks>
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Completed = 2
}
