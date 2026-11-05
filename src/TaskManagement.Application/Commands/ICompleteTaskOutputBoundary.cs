namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>ICompleteTaskOutputBoundary</c> es el puerto de salida por el que el caso de uso de completar
/// una tarea entrega su resultado. Lo usa CompleteTaskInteractor y lo implementa un presenter.
/// </summary>
public interface ICompleteTaskOutputBoundary
{
    void PresentSuccess();

    void PresentNotFound(string message);

    void PresentRuleViolation(IReadOnlyList<string> errors);
}
