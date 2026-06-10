namespace TaskManagement.Domain.Common;

/// <summary>
/// <c>TaskErrors</c> es un repositorio central para todos los mensajes de error
/// del dominio de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core en la capa del dominio</li>
/// <li>Centraliza mensajes de error para evitar duplicación y asegurar
/// consistencia</li>
/// <li>Representa todas las posibles violaciones de reglas de negocio en
/// gestión de tareas</li>
/// <li>Utilizado en capas del dominio y aplicación para informe de errores</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Fuente única de verdad para mensajes de error</li>
/// <li>Fácil de mantener y actualizar mensajes de error</li>
/// <li>Soporta internacionalización -i18n- si es necesario</li>
/// <li>Previene duplicación de cadenas y errores tipográficos</li>
/// <li>Documentación clara de todos los posibles errores relacionados con
/// tareas</li>
/// </ul>
///
/// Categorías de error:
/// <ul>
/// <li>Errores de transición de estado: <c>TaskErrors.AlreadyCompleted</c>,
/// <c>TaskErrors.CannotModifyCompletedTask</c>, etc.</li>
/// <li>Errores de asignación: <c>TaskErrors.CannotAssignCompletedTask</c></li>
/// <li>Errores de recurso: <c>TaskErrors.NotFound</c></li>
/// </ul>
/// </remarks>
public class TaskErrors
{
    public const string AlreadyCompleted = "Task is already completed.";
    public const string CannotAssignCompletedTask = "Cannot assign a completed task.";
    public const string CannotModifyCompletedTask = "Cannot modify a completed task.";

    public static string NotFound(Guid id) => $"Task with ID '{id}' was not found.";
}
