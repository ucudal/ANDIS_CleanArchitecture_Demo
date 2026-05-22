namespace TaskManagement.Domain.Common;

/// <summary>
/// TaskErrors es un repositorio central para todos los mensajes de error del dominio de tareas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Dominio - Shared/Common)
/// - Centraliza mensajes de error para evitar duplicación y asegurar consistencia
/// - Representa todas las posibles violaciones de reglas de negocio en gestión de tareas
/// - Utilizado en capas de dominio y aplicación para informe de errores
///
/// Beneficios:
/// - Fuente única de verdad para mensajes de error
/// - Fácil de mantener y actualizar mensajes de error
/// - Soporta internacionalización (i18n) si es necesario
/// - Previene duplicación de cadenas y errores tipográficos
/// - Documentación clara de todos los posibles errores relacionados con tareas
///
/// Categorías de Error:
/// - Errores de transición de estado (AlreadyCompleted, CannotModifyCompletedTask, etc.)
/// - Errores de asignación (CannotAssignCompletedTask)
/// - Errores de recurso (NotFound)
/// </summary>
public static class TaskErrors
{
    public const string AlreadyCompleted = "Task is already completed.";
    public const string CannotAssignCompletedTask = "Cannot assign a completed task.";
    public const string CannotModifyCompletedTask = "Cannot modify a completed task.";

    public static string NotFound(Guid id) => $"Task with ID '{id}' was not found.";
}
