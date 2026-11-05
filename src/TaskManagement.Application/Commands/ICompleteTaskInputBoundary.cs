namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>ICompleteTaskInputBoundary</c> es el puerto de entrada del caso de uso de completar una
/// tarea. Lo usan los adaptadores conductores y lo implementa CompleteTaskInteractor.
/// </summary>
public interface ICompleteTaskInputBoundary
{
    Task ExecuteAsync(CompleteTaskInput input, CancellationToken cancellationToken = default);
}
