// TaskManagement.API/Controllers/TasksController.cs
namespace TaskManagement.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Requests;
using TaskManagement.API.Extensions;
using TaskManagement.Application.Commands.CompleteTask;
using TaskManagement.Application.Commands.CreateTask;
using TaskManagement.Application.Queries.GetTaskById;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
            cancellationToken);

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
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            User.GetUserId() // Extension method
        );
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
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
            cancellationToken);

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