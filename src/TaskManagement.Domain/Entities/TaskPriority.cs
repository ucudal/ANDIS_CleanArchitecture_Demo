namespace TaskManagement.Domain.Entities;

/// <summary>
/// <c>TaskPriority</c> es un <a href="https://github.com/ucudal/ANDIS_Conceptos/blob/2e0fbbe729ea47ee6029405a8435c918c9e7c4e6/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Value_Object.md">objeto valor</a> de dominio que representa niveles de prioridad de tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa de dominio</li>
/// <li>Representa un concepto de negocio principal sin dependencias externas</li>
/// <li>Asegura seguridad de tipo para niveles de prioridad en todo el sistema</li>
/// <li>Utilizado por entidad <see cref="TaskItem"/> para categorizar urgencia de tarea</li>
/// </ul>
///
/// Niveles de Prioridad (en orden ascendente):
/// <ul>
/// <li>Low (0): Tarea tiene baja urgencia</li>
/// <li>Medium (1): Tarea tiene urgencia media</li>
/// <li>High (2): Tarea tiene alta urgencia y debe priorizarse</li>
/// </ul>
/// </remarks>
public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}
