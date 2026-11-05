namespace TaskManagement.Application.Commands;

/// <summary>
/// <c>ICreateTaskInputBoundary</c> es el puerto de entrada del caso de uso de crear una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Lo usan los adaptadores conductores, por ejemplo TasksController</li>
/// <li>Lo implementa CreateTaskInteractor, que el controller no conoce</li>
/// <li>No devuelve datos: el resultado sale por ICreateTaskOutputBoundary</li>
/// </ul>
/// </remarks>
public interface ICreateTaskInputBoundary
{
    Task ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default);
}
