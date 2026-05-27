// TaskManagement.API/Controllers/TasksController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Requests;
using TaskManagement.Application.Commands;
using TaskManagement.Application.Queries;

namespace TaskManagement.API.Controllers;

/// <summary>
/// <c>TasksController</c> es el endpoint de la API REST para operaciones de gestión de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, API REST</li>
/// <li>Punto de entrada para solicitudes HTTP</li>
/// <li>Traduce solicitudes HTTP a comandos/consultas de aplicación</li>
/// <li>Maneja cuestiones específicas de HTTP: códigos de estado, negociación de contenido</li>
/// <li>Sin lógica de negocio: delega a la capa de aplicación</li>
/// </ul>
///
/// Responsabilidades del controlador:
/// <ul>
/// <li>Aceptar solicitudes HTTP y convertirlas a comandos/consultas</li>
/// <li>Manejar autorización y autenticación</li>
/// <li>Invocar la capa de aplicación a través de <see cref="MediatR"/></li>
/// <li>Transformar resultados en respuestas HTTP apropiadas</li>
/// <li>Manejar excepciones y devolver respuestas de error</li>
/// </ul>
///
/// Separación de responsabilidades:
/// <ul>
/// <li>NO contiene lógica de negocio, se delega a aplicación</li>
/// <li>NO interactúa directamente con la base de datos, se delega a infraestructura)</li>
/// <li>NO valida reglas de negocio, se delegadan a dominio o aplicación</li>
/// <li>Solo traduce semántica HTTP a casos de uso</li>
/// </ul>
///
/// Dependencias:
/// <ul>
/// <li><see cref="IMediator"/>: Enviar comandos y consultas a la capa de aplicación</li>
/// <li>Authorization: Validar permisos de usuario antes de operaciones</li>
/// <li>DTOs <see cref="CreateTaskRequest"/>, <see cref="TaskDto"/>: Transferencia de datos</li>
/// </ul>
///
/// Patrones de diseño y convenciones:
/// <ul>
/// <li>Patrón Command/Query a través de <see cref="MediatR"/></li>
/// <li>Patrón Result para manejo de errores consistente</li>
/// <li>Convenciones REST (POST para creación, DELETE para eliminación, etc.)</li>
/// <li>Códigos de estado HTTP: 200 OK, 201 Created, 204 NoContent, 400 BadRequest, 404 NotFound, etc.</li>
/// </ul>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
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
            GetUserId(User)
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
