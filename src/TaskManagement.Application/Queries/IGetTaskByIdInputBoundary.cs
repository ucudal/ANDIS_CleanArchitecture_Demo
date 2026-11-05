namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>IGetTaskByIdInputBoundary</c> es el puerto de entrada del caso de uso de consultar una
/// tarea. Lo usan los adaptadores conductores y lo implementa GetTaskByIdInteractor.
/// </summary>
public interface IGetTaskByIdInputBoundary
{
    Task ExecuteAsync(GetTaskByIdInput input, CancellationToken cancellationToken = default);
}
