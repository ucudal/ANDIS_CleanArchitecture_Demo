// TaskManagement.API/Controllers/TasksController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Extensions;
using TaskManagement.API.Requests;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Application.Queries.GetTaskById;

namespace TaskManagement.API.Controllers;

/// <summary>
/// TasksController es el punto final de la API REST para operaciones de gestión de tareas.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte de la capa de presentación (UI Layer)</item>
/// <item>Punto de entrada para solicitudes HTTP</item>
/// <item>Traduce solicitudes HTTP a comandos/consultas de aplicación</item>
/// <item>Maneja preocupaciones específicas de HTTP (códigos de estado, negociación de contenido)</item>
/// <item>Sin lógica de negocio: delega a la capa de aplicación</item>
/// </list>
///
/// Responsabilidades del Controlador:
/// <list type="bullet">
/// <item>Mapear rutas HTTP a operaciones de negocio</item>
/// <item>Aceptar solicitudes HTTP y convertirlas a comandos/consultas</item>
/// <item>Manejar autorización y autenticación</item>
/// <item>Invocar la capa de aplicación a través de <see cref="MediatR"/></item>
/// <item>Transformar resultados en respuestas HTTP apropiadas</item>
/// <item>Manejar excepciones y devolver respuestas de error</item>
/// </list>
///
/// Separación de Responsabilidades:
/// <list type="bullet">
/// <item>NO contiene lógica de negocio (delegada a aplicación)</item>
/// <item>NO interactúa directamente con la base de datos (delegada a infraestructura)</item>
/// <item>NO valida reglas de negocio (delegadas a dominio/aplicación)</item>
/// <item>Solo traduce semántica HTTP a operaciones de aplicación</item>
/// </list>
///
/// Dependencias:
/// <list type="bullet">
/// <item><see cref="IMediator"/>: Enviar comandos y consultas a la capa de aplicación</item>
/// <item>Authorization: Validar permisos de usuario antes de operaciones</item>
/// <item>DTOs (<see cref="CreateTaskRequest"/>, <see cref="TaskDto"/>): Transferencia de datos</item>
/// </list>
///
/// Patrones de Diseño:
/// <list type="bullet">
/// <item>Patrón Command/Query a través de <see cref="MediatR"/></item>
/// <item>Patrón Result para manejo de errores consistente</item>
/// <item>Convenciones REST (POST para creación, DELETE para eliminación, etc.)</item>
/// <item>Códigos de estado HTTP: 200 OK, 201 Created, 204 NoContent, 400 BadRequest, 404 NotFound, etc.</item>
/// </list>
/// </summary>
internal sealed class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTaskByIdQuery(id),
            cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return result.Errors.Any(error => error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            ? NotFound(CreateProblemDetails(result.Errors))
            : BadRequest(CreateProblemDetails(result.Errors));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            User.GetUserId() // Método de extensión
        );
        var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = result.Value
                },
                result.Value);
        }

        return BadRequest(CreateValidationProblemDetails(result.Errors));
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CompleteTaskCommand(id),
            cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        return BadRequest(CreateProblemDetails(result.Errors));
    }

    private static ProblemDetails CreateProblemDetails(IReadOnlyList<string> errors)
    {
        return new ProblemDetails
        {
            Title = "Request failed",
            Detail = string.Join("; ", errors),
            Status = StatusCodes.Status400BadRequest
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        IReadOnlyList<string> errors)
    {
        return new ValidationProblemDetails(
            errors.ToDictionary(
                _ => "General",
                e => new[] { e }))
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest
        };
    }
}
