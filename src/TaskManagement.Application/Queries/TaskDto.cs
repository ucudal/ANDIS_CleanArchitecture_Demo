namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>TaskDto</c> es el DTO de salida del caso de uso de consultar una tarea.
/// </summary>
/// <remarks>
/// Optimizado para lecturas. Se mantiene como tipo basado en propiedades para que Dapper pueda
/// materializarlo sin requerir una firma de constructor exacta de tipos de proveedor.
/// </remarks>
public sealed class TaskDto
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
