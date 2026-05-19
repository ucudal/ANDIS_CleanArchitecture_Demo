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
/// TasksController is the REST API endpoint for task management operations.
///
/// Role in Clean Architecture:
/// - Part of the UI Layer (API presentation)
/// - Entry point for HTTP requests
/// - Translates HTTP requests to application commands/queries
/// - Handles HTTP-specific concerns (status codes, content negotiation)
/// - No business logic: delegates to application layer
///
/// Controller Responsibilities:
/// - Map HTTP routes to business operations
/// - Accept HTTP requests and convert to commands/queries
/// - Handle authorization and authentication
/// - Invoke application layer through MediatR
/// - Transform results to appropriate HTTP responses
/// - Handle exceptions and return error responses
///
/// Separation of Concerns:
/// - Does NOT contain business logic (delegated to application)
/// - Does NOT interact directly with database (delegated to infrastructure)
/// - Does NOT validate business rules (delegated to domain/application)
/// - Only translates HTTP semantics to application operations
///
/// Dependencies:
/// - IMediator: Send commands and queries to application layer
/// - Authorization: Validate user permissions before operations
/// - DTOs (CreateTaskRequest, TaskDto): Transfer data
///
/// Design Patterns:
/// - Command/Query pattern through MediatR
/// - Result pattern for consistent error handling
/// - REST conventions (POST for creation, DELETE for removal, etc.)
/// - HTTP status codes: 200 OK, 201 Created, 204 NoContent, 400 BadRequest, 404 NotFound, etc.
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
            User.GetUserId() // Extension method
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
