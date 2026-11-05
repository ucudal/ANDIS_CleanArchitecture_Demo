namespace TaskManagement.Application.Queries;

/// <summary>
/// <c>IGetTaskByIdOutputBoundary</c> es el puerto de salida por el que el caso de uso de consultar
/// una tarea entrega su resultado. Lo usa GetTaskByIdInteractor y lo implementa un presenter.
/// </summary>
public interface IGetTaskByIdOutputBoundary
{
    void PresentSuccess(TaskDto task);

    void PresentNotFound(string message);
}
