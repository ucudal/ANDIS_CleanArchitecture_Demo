namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>ICreateTaskOutputBoundary</c> es el puerto de salida por el que el caso de uso de crear una
/// tarea entrega su resultado.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Lo usa CreateTaskInteractor</li>
/// <li>Lo implementa un presenter, en un círculo exterior</li>
/// <li>El flujo de control va hacia afuera, pero la dependencia va hacia adentro</li>
/// </ul>
/// </remarks>
public interface ICreateTaskOutputBoundary
{
    void PresentSuccess(CreateTaskOutput output);

    void PresentValidationErrors(IReadOnlyList<string> errors);
}
