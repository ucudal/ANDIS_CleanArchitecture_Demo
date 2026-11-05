namespace TaskManagement.API.ViewModels;

/// <summary>
/// <c>TaskViewModel</c> es la representación de una tarea en las respuestas de la %API REST.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, una %API REST</li>
/// <li>Lo construye GetTaskByIdPresenter a partir de TaskDto</li>
/// <li>Puede cambiar con las necesidades de esta interfaz sin afectar el caso de uso; otra
/// interfaz tendría su propio view model</li>
/// </ul>
/// </remarks>
public sealed class TaskViewModel
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime? DueDate {
        get; init;
    }
    public DateTime CreatedAt {
        get; init;
    }
    public string CreatedBy { get; init; } = string.Empty;
    public string? AssignedTo {
        get; init;
    }
}
