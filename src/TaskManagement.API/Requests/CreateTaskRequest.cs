using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Requests;

/// <summary>
/// CreateTaskRequest is the input DTO for creating a new task via HTTP API.
///
/// Role in Clean Architecture:
/// - Part of the UI Layer (API presentation)
/// - Input DTO: Transfers data from HTTP request to application layer
/// - Request binding: ASP.NET Core deserializes JSON to this type
/// - Decouples API contract from application layer
///
/// Request DTOs vs Commands:
/// - CreateTaskRequest: HTTP request format, API-specific structure
/// - CreateTaskCommand: Application command, domain-aware structure
/// - Controller transforms request to command
/// - Separation allows independent evolution of API and application
///
/// Benefits of Input DTOs:
/// - API contracts separate from application models
/// - Can add API-specific validation (attribute-based)
/// - Can include API documentation (DataAnnotations)
/// - Can transform to different command structures
/// - Backwards compatibility when changing internal models
///
/// Data Flow:
/// 1. HTTP client sends JSON in request body
/// 2. ASP.NET Core deserializes JSON to CreateTaskRequest
/// 3. Model validation applied (data annotations)
/// 4. Controller transforms to CreateTaskCommand
/// 5. Command sent to application layer via MediatR
/// 6. Application layer validation (FluentValidation, domain rules)
/// 7. Domain logic executed
///
/// Notes:
/// - Properties should match JSON field names (or use [JsonPropertyName])
/// - Type safety ensures invalid data is rejected early
/// - Data annotations document required fields and constraints
/// </summary>

internal sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate
);
