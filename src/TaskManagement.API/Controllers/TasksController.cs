// TaskManagement.API/Controllers/TasksController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Presenters;
using TaskManagement.API.Requests;
using TaskManagement.API.ViewModels;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Queries;

namespace TaskManagement.API.Controllers;

/// <summary>
/// <c>TasksController</c> es el endpoint de la %API REST para operaciones de
/// gestión de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, una %API REST</li>
/// <li>Adaptador conductor: punto de entrada para solicitudes HTTP</li>
/// <li>Traduce solicitudes HTTP a los DTO de entrada de los casos de uso</li>
/// <li>Usa los puertos de entrada -input boundaries-; no conoce a los interactores</li>
/// <li>Devuelve la respuesta que construyó el presenter</li>
/// </ul>
///
/// Separación de responsabilidades:
/// <ul>
/// <li>NO contiene lógica de negocio, se delega a la capa de aplicación</li>
/// <li>NO interactúa directamente con la base de datos, se delega a la capa de
/// infraestructura</li>
/// <li>NO decide los códigos de estado ni el formato de las respuestas, se delega a los
/// presenters</li>
/// </ul>
///
/// Los casos de uso y los presenters se reciben con <c>FromServices</c> y hay una instancia por
/// solicitud: el caso de uso escribe en el mismo presenter que el controlador lee.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] IGetTaskByIdInputBoundary getTaskById,
        [FromServices] GetTaskByIdPresenter presenter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(getTaskById);
        ArgumentNullException.ThrowIfNull(presenter);

        await getTaskById.ExecuteAsync(new GetTaskByIdInput(id), cancellationToken).ConfigureAwait(false);
        return presenter.Result;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        [FromServices] ICreateTaskInputBoundary createTask,
        [FromServices] CreateTaskPresenter presenter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(createTask);
        ArgumentNullException.ThrowIfNull(presenter);

        var input = new CreateTaskInput(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            GetUserId(User));
        await createTask.ExecuteAsync(input, cancellationToken).ConfigureAwait(false);
        return presenter.Result;
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
        Guid id,
        [FromServices] ICompleteTaskInputBoundary completeTask,
        [FromServices] CompleteTaskPresenter presenter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completeTask);
        ArgumentNullException.ThrowIfNull(presenter);

        await completeTask.ExecuteAsync(new CompleteTaskInput(id), cancellationToken).ConfigureAwait(false);
        return presenter.Result;
    }

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        // Soportar nombres comunes de reclamación JWT/user-jwts para ID de usuario.
        var candidateValues = user.Claims
            .Where(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "sub" ||
                c.Type == "nameid" ||
                c.Type == "oid" ||
                c.Type == "uid" ||
                c.Type == "userId")
            .Select(c => c.Value);

        foreach (var candidate in candidateValues)
        {
            if (Guid.TryParse(candidate, out var parsedUserId))
            {
                return parsedUserId;
            }
        }

        throw new InvalidOperationException("The current user does not have a valid GUID user id claim.");
    }
}
