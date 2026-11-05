using TaskManagement.Application.Ports;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>GetTaskByIdInteractor</c> implementa el caso de uso de consultar una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Lado de lectura de CQRS: usa ITaskReadRepository y no carga la entidad TaskItem</li>
/// <li>No modifica estado ni publica eventos</li>
/// </ul>
/// </remarks>
public sealed class GetTaskByIdInteractor : IGetTaskByIdInputBoundary
{
    private readonly ITaskReadRepository _readRepository;
    private readonly IGetTaskByIdOutputBoundary _output;

    public GetTaskByIdInteractor(
        ITaskReadRepository readRepository,
        IGetTaskByIdOutputBoundary output)
    {
        _readRepository = readRepository;
        _output = output;
    }

    public async Task ExecuteAsync(GetTaskByIdInput input, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        var task = await _readRepository.GetByIdAsync(input.TaskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            _output.PresentNotFound(TaskErrors.NotFound(input.TaskId));
            return;
        }

        _output.PresentSuccess(task);
    }
}
