namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>ValidatingCreateTaskInteractor</c> valida la entrada antes de ejecutar el caso de uso de
/// crear una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Es un decorador de ICreateTaskInputBoundary: ni el controller ni CreateTaskInteractor
/// saben que existe; se conecta en Program.cs</li>
/// <li>Si la entrada no es válida, presenta los errores y no llama al caso de uso</li>
/// </ul>
///
/// Valida la forma de la entrada para fallar temprano. La entidad TaskItem sigue protegiendo sus
/// invariantes, porque es la dueña de esas reglas.
/// </remarks>
public sealed class ValidatingCreateTaskInteractor : ICreateTaskInputBoundary
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;

    private readonly ICreateTaskInputBoundary _next;
    private readonly ICreateTaskOutputBoundary _output;

    public ValidatingCreateTaskInteractor(
        ICreateTaskInputBoundary next,
        ICreateTaskOutputBoundary output)
    {
        _next = next;
        _output = output;
    }

    public Task ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var errors = Validate(input);
        if (errors.Count > 0)
        {
            _output.PresentValidationErrors(errors);
            return Task.CompletedTask;
        }

        return _next.ExecuteAsync(input, cancellationToken);
    }

    private static List<string> Validate(CreateTaskInput input)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(input.Title))
            errors.Add("Title is required.");
        else if (input.Title.Length > MaxTitleLength)
            errors.Add("Title cannot exceed 200 characters.");
        if (input.Description?.Length > MaxDescriptionLength)
            errors.Add("Description cannot exceed 2000 characters.");
        if (!Enum.IsDefined(input.Priority))
            errors.Add("Priority is not valid.");
        if (input.DueDate.HasValue && input.DueDate.Value < DateTime.UtcNow.Date)
            errors.Add("Due date cannot be in the past.");
        if (input.CreatedBy == Guid.Empty)
            errors.Add("CreatedBy is required.");
        return errors;
    }
}
