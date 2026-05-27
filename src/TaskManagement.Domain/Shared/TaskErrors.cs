namespace TaskManagement.Domain.Common;

/// <summary>
/// <c>TaskErrors</c> es un repositorio central para todos los mensajes de error del dominio de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Dominio - Shared/Common)</li>
/// <li>Centraliza mensajes de error para evitar duplicación y asegurar consistencia</li>
/// <li>Representa todas las posibles violaciones de reglas de negocio en gestión de tareas</li>
/// <li>Utilizado en capas de dominio y aplicación para informe de errores</li>
/// </ul>
///
/// Beneficios:
/// <ul>
/// <li>Fuente única de verdad para mensajes de error</li>
/// <li>Fácil de mantener y actualizar mensajes de error</li>
/// <li>Soporta internacionalización (i18n) si es necesario</li>
/// <li>Previene duplicación de cadenas y errores tipográficos</li>
/// <li>Documentación clara de todos los posibles errores relacionados con tareas</li>
/// </ul>
///
/// Categorías de Error:
/// <ul>
/// <li>Errores de transición de estado (AlreadyCompleted, CannotModifyCompletedTask, etc.)</li>
/// <li>Errores de asignación (CannotAssignCompletedTask)</li>
/// <li>Errores de recurso (NotFound)</li>
/// </ul>
/// </remarks>
public static class TaskErrors
{
    public const string AlreadyCompleted = "Task is already completed.";
    public const string CannotAssignCompletedTask = "Cannot assign a completed task.";
    public const string CannotModifyCompletedTask = "Cannot modify a completed task.";

    public static string NotFound(Guid id) => $"Task with ID '{id}' was not found.";
}
