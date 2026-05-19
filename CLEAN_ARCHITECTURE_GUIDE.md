# Clean Architecture Guide - TaskManagement Project

## Overview

This project demonstrates **Clean Architecture** principles as documented by Microsoft and popularized by Uncle Bob (Robert Martin). All classes in this project have been documented to explain their role in the architecture. This guide helps you understand the architecture by navigating through the codebase.

**Documentation Reference:** https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture

---

## Architecture Layers

Clean Architecture uses concentric circles (like an onion) where dependencies flow **inward** toward the business logic. The project is organized into four layers:

### 1. **Domain Layer** (Application Core - Innermost)
**Location:** `src/TaskManagement.Domain/`

Contains pure business logic with **no dependencies** on external libraries or other layers.

#### Key Components:

| Component | Files | Purpose |
|-----------|-------|---------|
| **Entities** | `Entities/TaskItem.cs`, `Entities/TaskStatus.cs`, `Entities/TaskPriority.cs` | Business model classes representing domain concepts |
| **Value Objects** | `ValueObjects/Email.cs`, `Shared/ValueObject.cs` | Immutable objects representing domain values |
| **Domain Events** | `Events/DomainEvent.cs` | Events representing important domain occurrences |
| **Interfaces** | `Interfaces/ITaskRepository.cs`, `Interfaces/IDomainEventDispatcher.cs` | Abstractions for infrastructure concerns |
| **Exceptions** | `Exceptions/DomainException.cs` | Domain-specific exceptions |
| **Shared Types** | `Shared/Result.cs`, `Shared/TaskErrors.cs` | Common types used across domain |

**Key Principle:** Domain layer **never depends** on Application or Infrastructure layers. It contains only business rules and logic.

#### Example - TaskItem Entity:
```csharp
// TaskItem is an Aggregate Root - manages its own state and business rules
public class TaskItem
{
    // Factory method for safe entity creation
    public static Result<TaskItem> Create(string title, string description, ...)
    {
        // Validation happens in domain
        var validation = Validate(title, description, dueDate);
        if (validation.IsFailure)
            return Result.Failure<TaskItem>(validation.Errors);
        // Entity creation with initial domain event
        var task = new TaskItem { ... };
        task.AddDomainEvent(new TaskCreatedEvent(...));
        return Result.Success(task);
    }

    // Business methods enforce invariants
    public Result Complete()
    {
        if (Status == TaskStatus.Completed)
            return Result.Failure(TaskErrors.AlreadyCompleted);
        Status = TaskStatus.Completed;
        AddDomainEvent(new TaskCompletedEvent(...));
        return Result.Success();
    }
}
```

---

### 2. **Application Layer** (Application Core - Middle)
**Location:** `src/TaskManagement.Application/`

Orchestrates domain layer and infrastructure. Implements use cases (application services). **Depends only on Domain layer.**

#### Key Components:

| Component | Files | Purpose |
|-----------|-------|---------|
| **Commands** | `Commands/CreateTaskCommand.cs`, `Commands/CompleteTaskCommand.cs` | Request objects for state-changing operations (CQRS) |
| **Queries** | `Queries/GetTaskByIdQuery.cs` | Request objects for data reads (CQRS) |
| **Handlers** | Inside Command/Query files | Application services implementing use cases |
| **DTOs** | `Queries/GetTaskByIdQuery.cs` (TaskDto), `Requests/CreateTaskRequest.cs` | Data transfer objects for input/output |
| **Interfaces** | `Interfaces/IUnitOfWork.cs`, `Interfaces/ITaskReadRepository.cs`, `Interfaces/IEmailService.cs` | Abstractions for infrastructure services |
| **Behaviors** | `Behaviors/ValidationBehavior.cs` | MediatR pipeline behaviors (cross-cutting concerns) |
| **Exceptions** | `Exceptions/ValidationException.cs`, `Exceptions/NotFoundException.cs` | Application-layer specific exceptions |
| **Shared** | `Shared/PagedResult.cs` | Common DTOs and types |

**Key Principle:** Application layer implements business use cases but **delegates business logic to domain layer**. It's an orchestrator, not a business logic container.

#### CQRS Pattern (Command Query Responsibility Segregation):

```
Commands (State-Changing):
┌─ CreateTaskCommand ──→ CreateTaskCommandHandler ──→ Domain.TaskItem.Create()
├─ CompleteTaskCommand ──→ CompleteTaskCommandHandler ──→ Domain.TaskItem.Complete()

Queries (Read-Only):
├─ GetTaskByIdQuery ──→ GetTaskByIdQueryHandler ──→ ITaskReadRepository.GetById()
```

#### Example - CreateTaskCommandHandler:
```csharp
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    // Delegates to domain for business logic
    var createResult = TaskItem.Create(
        request.Title,
        request.Description,
        request.Priority,
        request.DueDate,
        request.CreatedBy);

    // Persists domain entity
    await _taskRepository.AddAsync(task, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    // Dispatches domain events
    await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);
}
```

---

### 3. **Infrastructure Layer** (Outer layer)
**Location:** `src/TaskManagement.Infrastructure/`

Implements technical concerns and infrastructure services. **Depends on Application and Domain layers** (one-way dependency).

#### Key Components:

| Component | Files | Purpose |
|-----------|-------|---------|
| **DbContext** | `Persistence/TaskDbContext.cs` | Entity Framework Core database context |
| **Repositories** | `Persistence/Repositories/TaskRepository.cs`, `TaskReadRepository.cs` | Data access implementations |
| **Configurations** | `Persistence/Configuration/TaskConfiguration.cs` | Entity Framework model configurations |
| **Event Dispatcher** | `EventDispatching/MediatRDomainEventDispatcher.cs` | Domain event publishing implementation |

**Key Principle:** Infrastructure implements interfaces defined in Domain and Application layers. This inverts dependencies away from core logic.

#### Example - Repository Pattern:
```csharp
// Interface defined in Domain Layer (TaskManagement.Domain)
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    // ... other methods
}

// Implementation in Infrastructure Layer (TaskManagement.Infrastructure)
public sealed class TaskRepository : ITaskRepository
{
    // Infrastructure details (Entity Framework) hidden from domain
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}
```

#### Dependency Inversion:
```
Without Clean Architecture:
Domain → Application → Infrastructure (top-down dependency)
❌ Domain depends on infrastructure details

With Clean Architecture:
Domain ← Application ← Infrastructure (inverted dependency)
✅ Infrastructure depends on abstractions in Domain
```

---

### 4. **API/UI Layer** (Outermost)
**Location:** `src/TaskManagement.API/`

Entry point for requests. Handles HTTP concerns. **Can depend on all layers** (only place Infrastructure is directly referenced).

#### Key Components:

| Component | Files | Purpose |
|-----------|-------|---------|
| **Controllers** | `Controllers/TasksController.cs` | HTTP endpoints |
| **Middleware** | `Middleware/ExceptionHandlingMiddleware.cs` | Cross-cutting pipeline behaviors |
| **Extensions** | `Extensions/ClaimsPrincipalExtensions.cs` | Helper extension methods |
| **Requests** | `Requests/CreateTaskRequest.cs` | Input DTOs for HTTP requests |
| **Startup** | `Program.cs` | Dependency injection composition root |

**Key Principle:** UI layer translates HTTP to application commands/queries. It contains **no business logic** and delegates everything to application layer.

#### Example - Controller:
```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        // Translate HTTP request to application command
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            userId);

        // Delegate to application layer
        var result = await _mediator.Send(command, cancellationToken);

        // Translate result to HTTP response
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

        return BadRequest(CreateProblemDetails(result.Errors));
    }
}
```

#### Composition Root (Program.cs):
```csharp
// Register Domain layer abstractions
builder.Services
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

// Register Application layer services
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))
    .AddValidatorsFromAssembly(...)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Register Infrastructure layer
builder.Services
    .AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(connectionString));
```

---

## Dependency Flow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                      API Layer (UI)                          │
│  Controllers, Middleware, Requests, ExceptionHandling       │
│  ┌────────────────────────────────────────────────────────┐  │
│  │             Can reference all layers ↓               │  │
└──────────────────────────────────────────────────────────────┘
      ↓
┌──────────────────────────────────────────────────────────────┐
│                 Application Layer (Core)                     │
│  Commands, Queries, Handlers, DTOs, Interfaces, Behaviors   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │     References Domain + Infrastructure              │  │
└──────────────────────────────────────────────────────────────┘
      ↑                                    ↓
      │                                    │
      └─────────────────────┬──────────────┘
                            │
┌──────────────────────────────────────────────────────────────┐
│                   Domain Layer (Core)                        │
│  Entities, Value Objects, Events, Exceptions, Interfaces    │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              NO External Dependencies ✓              │  │
└──────────────────────────────────────────────────────────────┘
      ↑
      │ (Implements abstractions)
      │
┌──────────────────────────────────────────────────────────────┐
│                Infrastructure Layer                          │
│  Repositories, DbContext, Event Dispatcher, Configurations   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │           Depends only on abstractions ✓             │  │
└──────────────────────────────────────────────────────────────┘
```

---

## Key Design Patterns Used

### 1. **Result Pattern**
Replace exceptions with explicit success/failure returns:
```csharp
public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }
}

// Usage
var result = TaskItem.Create(...);
if (result.IsSuccess)
    // Use result.Value
else
    // Handle result.Errors
```

### 2. **Repository Pattern**
Abstract data access behind interfaces:
```csharp
// Domain/Infrastructure abstraction
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);
}
```

### 3. **CQRS (Command Query Responsibility Segregation)**
Separate read and write operations:
```csharp
// Write (state-changing)
public sealed record CreateTaskCommand(...) : IRequest<Result<Guid>>;

// Read (query-only)
public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;

// Different repositories for optimization
ITaskRepository (write)
ITaskReadRepository (read-optimized)
```

### 4. **Domain Events**
Enable loose coupling through events:
```csharp
// Domain entity raises event
task.AddDomainEvent(new TaskCreatedEvent(task.Id, task.Title, createdBy));

// Application service dispatches event
await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);

// Handlers react to events (in Infrastructure)
public class SendTaskCreatedEmailHandler : INotificationHandler<TaskCreatedEvent>
{
    public async Task Handle(TaskCreatedEvent @event, CancellationToken cancellationToken)
    {
        await _emailService.SendTaskCreatedEmailAsync(@event.Title, cancellationToken);
    }
}
```

### 5. **Dependency Injection**
Invert control through constructor injection:
```csharp
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    // Dependencies injected through constructor
    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher eventDispatcher)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
    }
}
```

### 6. **Specification Pattern (with CQRS)**
Encapsulate query criteria:
```csharp
public sealed record TaskSearchRequest(
    string? Title = null,
    string? Status = null,
    string? Priority = null,
    Guid? AssignedTo = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "CreatedAt",
    string SortDirection = "DESC");
```

---

## Testing Architecture

Clean Architecture enables easy testing through dependency inversion:

### Unit Testing Domain Layer
```csharp
[Fact]
public void TaskItem_Create_ValidatesTitle()
{
    // Domain layer has no external dependencies
    // Can test pure business logic
    var result = TaskItem.Create("", "description", TaskPriority.High, null, userId);

    Assert.True(result.IsFailure);
    Assert.Contains("Title is required", result.Errors);
}
```

### Unit Testing Application Layer
```csharp
[Fact]
public async Task CreateTaskCommandHandler_ValidCommand_CreatesTask()
{
    // Mock infrastructure interfaces
    var mockRepository = new Mock<ITaskRepository>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockEventDispatcher = new Mock<IDomainEventDispatcher>();

    var handler = new CreateTaskCommandHandler(
        mockRepository.Object,
        mockUnitOfWork.Object,
        mockEventDispatcher.Object);

    var command = new CreateTaskCommand("Task Title", "Description", TaskPriority.High, null, userId);
    var result = await handler.Handle(command, CancellationToken.None);

    Assert.True(result.IsSuccess);
    mockRepository.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Integration Testing Infrastructure Layer
```csharp
[Fact]
public async Task TaskRepository_AddAsync_PersistsTask()
{
    // Test with real database (in-memory SQLite)
    var task = TaskItem.Create("Task", "Description", TaskPriority.High, null, userId).Value!;
    await _repository.AddAsync(task);

    var retrieved = await _repository.GetByIdAsync(task.Id);
    Assert.NotNull(retrieved);
    Assert.Equal(task.Title, retrieved.Title);
}
```

---

## How to Navigate the Code

1. **Start with Domain Layer** (`TaskManagement.Domain/`)
   - Read `Entities/TaskItem.cs` to understand the business model
   - Read `Shared/Result.cs` to understand error handling
   - Read `Events/DomainEvent.cs` to understand event-driven architecture

2. **Move to Application Layer** (`TaskManagement.Application/`)
   - Read `Commands/CreateTaskCommand.cs` to see how use cases are implemented
   - Read `Behaviors/ValidationBehavior.cs` to understand cross-cutting concerns
   - Read `Queries/GetTaskByIdQuery.cs` to see CQRS pattern

3. **Explore Infrastructure Layer** (`TaskManagement.Infrastructure/`)
   - Read `Persistence/Repositories/TaskRepository.cs` to see how domain interfaces are implemented
   - Read `Persistence/TaskDbContext.cs` to see database configuration
   - Read `EventDispatching/MediatRDomainEventDispatcher.cs` to see event handling

4. **Check API Layer** (`TaskManagement.API/`)
   - Read `Controllers/TasksController.cs` to see HTTP endpoints
   - Read `Program.cs` to see dependency injection setup
   - Read `Middleware/ExceptionHandlingMiddleware.cs` to see error handling

---

## Benefits of Clean Architecture

✅ **Testability**: Domain logic has no external dependencies
✅ **Maintainability**: Clear separation of concerns
✅ **Flexibility**: Easy to swap implementations (database, email service, etc.)
✅ **Scalability**: Can optimize each layer independently
✅ **Reusability**: Domain logic can be reused in different UIs (web, console, API, etc.)
✅ **Independence**: Business logic is isolated from frameworks

---

## Further Reading

- Microsoft Clean Architecture Documentation: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/
- Uncle Bob's Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- eShopOnWeb Reference Application: https://github.com/dotnet-architecture/eShopOnWeb
- Ardalis Clean Architecture Template: https://github.com/ardalis/cleanarchitecture

---

## Documentation Comments

All classes in this project include comprehensive XML documentation comments that explain:
- **Role in Clean Architecture**: Where the class fits in the architecture
- **Responsibilities**: What the class is responsible for
- **Dependencies**: What other layers/classes it depends on
- **Design Patterns**: Patterns used in the implementation
- **Benefits**: Why this approach is beneficial

You can view these comments:
- **In Visual Studio/VS Code**: Hover over class/method names to see IntelliSense tooltips
- **In Source Code**: Read the `///` XML comments above each class
- **Generated Docs**: Run `dotnet build --documentation` to generate XML documentation

---

**Last Updated:** May 2026
**Architecture Reference:** Microsoft eBook "Architect Modern Web Applications with ASP.NET Core and Azure"
