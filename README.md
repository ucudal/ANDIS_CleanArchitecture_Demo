


![Task Management Containers](https://www.plantuml.com/plantuml/png/ZLDDRzD04BtxLmpcW2EXvD8JGgYj7MXGceJ4ACTeR4_2qlMkjRkkY13_dTcrtI3SIit17vitxyrxCtu7duMa3v2ZVRX7XIUgIRb3gfL3eSZOQTFMChgs3etpJs-4AcLR4NnChsRfrGNvAOesZDSgQIsPzn1_vrlCzzGGEhA0ge8wI5XmB6VZIHJjpjPHVRZLhQhGd1DxjbpYku5BEz2XuWdyZe1_Jve9DgA7Nz3Jcyt2wzxiDjFLF6Uq8TXJ0VtvNPvlW5JLQA6SXQCsePs-Dy_0ZVqlqpmZqpHIbEY4Li7cMqWbMWjBKHYs4dSHG8cMs5EMh_dgCs1LCaSioOvqAbtpmrWg2-SlgSoL4qvuaKUKBO4kVbBf-BMZ0wbFeXnf2NKqE9PQ-6FhJ5kwrj1BadxIWcUYIWgUw1TxKYY3kPC9eOpiPKLI5SDqirlFi_ksX0mT5XnIU7Syh7uz0h4XoSjJGSKWvliJHLkI_OPrm-UCxJ3MsTVTUkNNoeF2tzta5kfHFp1lEyo3CK-G8GXF15YmTon3_o3gHePKk84Vn6aCLSsGxL3M9rthYkI_g9xxbUIXhYy9IMq9AplR6-4wZ-ljFSo0GmkVpjrwVrZElynNMJA9hdcZ_VVv5m00)



# Introducción

En [Clean
Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html),
el código se organiza en círculos concéntricos, donde cada círculo representa
diferentes áreas del software[^1]. Los elementos de código de un círculo no
pueden tener referencias a elementos en un círculo exterior, o dicho de otra
forma, las dependencias pueden ir sólo de afuera hacia adentro.

[^1]: Por lo tanto, esta arquitectura hace énfasis en la **separación de
    aspectos** o, en inglés, [*separation of
    concerns*](https://en.wikipedia.org/wiki/Separation_of_concerns).

![](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

Esta demo es una simple aplicación para gestión de tareas y es una versión
extendida del ejemplo de [^2].

[^2]: Maurice, M. (2026, February 1). Clean architecture in .NET: A complete
    production-ready guide.
    [Medium](https://medium.com/@michaelmaurice410/clean-architecture-in-net-a-complete-production-ready-guide-49dcbdb22166).

Todas las clases en esta demo están documentadas para explicar su rol en
la arquitectura. Esta guía te ayuda a entender la arquitectura navegando por el
código.

> Revisa la documentación de la demo [aquí](./docs/html/index.html).

# Estructura de proyectos

La arquitectura en este demo está organizada en proyectos de C#
independientes[^3], donde cada proyecto corresponde a un círculo:

[^3]: En la [documentación de Microsoft sobre Clean
    Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
    el círculo central es `Application Core` e incluye lo que en la
    documentación original de [Clean
    Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
    son los círculos `Entities` y `Use Cases`. A pesar de ser una demo en .NET,
    usamos la terminología original.

1. [TaskManagement.Domain](./src/TaskManagement.Domain/TaskManagement.Domain.csproj)
  es una librería de C# en la que se define el
  [dominio](https://github.com/ucudal/ANDIS_Conceptos/blob/main/4_Conceptos/4_Dominio.md)
  de la aplicación:
  [entidades](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Entity.md)
  y [objetos
  valor](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Value_Object.md),
  eventos y excepciones. El domino utiliza
  [eventos](./src/TaskManagement.Domain/Events/DomainEvent.cs) para informar
  cuando se crea, se completa, o se asigna una tarea, o cuando cambia su
  prioridad —ver por ejemplo la propiedad `TaskItem.DomainEvents` y el método
  `TaskItem.Create` en
  [TaskItem.cs](./src/TaskManagement.Domain/Entities/TaskItem.cs)—; por esto,
  esta aplicación también utiliza una [arquitectura dirigida por
  eventos](https://github.com/ucudal/ANDIS_Conceptos/blob/main/3_Plantillas/3_13_Event_Driven_Architecture.md).
  La capa de dominio tiene la responsabilidad de generar eventos, pero es la
  capa de aplicación la que tienen la responsabilidad de procesarlos —ver por
  ejemplo `CreateTaskCommand.Handle` en
  [CreateTaskCommand.cs](./src/TaskManagement.Application/Commands/CreateTaskCommand.cs)—.
  La capa de aplicación define —y utiliza— abstracciones que emplean el patrón
  de [inyección de
  dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview)
  —ver por ejemplo la interfaz `ITaskRepository` en
  [ITaskRepositorio.cs](./src/TaskManagement.Domain/Interfaces/ITaskRepository.cs).
  Las clases concretas que implementan esa abstracciones están definidas en la
  capa de infraestructura -ver por ejemplo la clase `TaskRepository` en
  [TaskRepository.cs](./src/TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs)-
  y son inyectadas en tiempo de ejecución en la capa de interfaz API -ver la
  llamada a `builder.services… .AddScoped<ITaskRepository, TaskRepository>()` en
  [Program.cs](./src/TaskManagement.API/Program.cs)-. El proyecto
  [`TaskManagement.Domain`](./src/TaskManagement.Domain/TaskManagement.Domain.csproj)
  no referencia ningún otro proyecto, es el centro de los círculos concéntricos.

2. [TaskManagement.Application](./src/TaskManagement.Application/TaskManagement.Application.csproj)
  en otra librería de C# en la que se definen las funcionalidades de la
  aplicación —comandos y consultas—, o dicho de otra forma, donde se implementa
  la lógica de los casos de uso. Esta aplicación usa el patrón
  [CQRS](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_09_.Patrones_de_arquitectura/2_09_CQRS.md)
  donde los comandos están separados de las consultas —ver por ejemplo la clase
  `CreateTaskCommand` en
  [CreateTaskCommand.cs](./src/TaskManagement.Application/Commands/CreateTaskCommand.cs)
  y la clase `GetTaskBuyIdQuery` en
  [GetTaskByIdQuery.cs](./src/TaskManagement.Application/Queries/GetTaskByIdQuery.cs)—.
  La capa de aplicación tamibén define —y utiliza— abstracciones que emplean el
  patrón de [inyección de
  dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview)
  —ver por ejemplo las interfaces `IEmailService` en
  [IEmailService.cs](./src/TaskManagement.Application/Interfaces/IEmailService.cs),
  `ITaskReadRepository` en
  [ITaskReadRepository.cs](./src/TaskManagement.Application/Interfaces/ITaskReadRepository.cs)
  o `IUnitOfWork` en
  [IUnitOfWork.cs](./src/TaskManagement.Application/Interfaces/IUnitOfWork.cs)—.
  Las clases concretas que implementan estas abstracciones están también
  definidas en la capa de infraestructura —ver por ejemplo la clase
  `TaskReadRepository` en
  [TaskReadRepository.cs](./src/TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs)—
  y también son creadas en tiempo de ejecución en la capa de interfaz API —ver
  las llamadas a `builder.services… .AddScoped<IUnitOfWork>(…),
  .AddScoped<ITaskReadRepository, TaskReadRepository>(), y
  .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>()` en
  [Program.cs](./src/TaskManagement.API/Program.cs)—. Los eventos creados en la
  capa de dominio son procesados en la capa de aplicación usando una abstracción
  —ver la interfaz `IDomainEventDispatcher` en
  [IDomainEventDispatcher.cs](./src/TaskManagement.Application/Interfaces/IDomainEventDispatcher.cs)—;
  y esa abstracción también está implementada en una clase definida en la capa
  de infraestructura —ver la clase `MediatRDomainEventDispatcher` en
  [MediatRDomainEventDispatcher.cs](./src/TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs)—.
  El proyecto
  [`TaskManagement.Application`](./src/TaskManagement.Application/TaskManagement.Application.csproj)
  referencia solamente el proyecto
  [`TaskManagement.Domain`](./src/TaskManagement.Domain/TaskManagement.Domain.csproj),
  la dependencia es de un círculo externo al centro de los círculos
  concéntricos.

3. [TaskManagement.Infrastructure](./src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj)
  es otra librería de C# en la que se definen cómo se implementa la
  infraestructura para las abstracciones definidas en las capas de dominio
  —repositorios— y aplicación —despacho de eventos y persistencia—. Las
  abstracciones
  [`ITaskReadRepository`](./src/TaskManagement.Application/Interfaces/ITaskReadRepository.cs)
  de la capa de aplicación y
  [`ITaskRepository`](./src/TaskManagement.Domain/Interfaces/ITaskRepository.cs)
  de la capa de dominio se implementan con las clases
  [`TaskReadRepository`](./src/TaskManagement.Infrastructure/Persistence/Repositories/TaskReadRepository.cs)
  y
  [`TaskRepository`](./src/TaskManagement.Infrastructure/Persistence/Repositories/TaskRepository.cs),
  respectivamente, de esta capa de infraestructura. También en este caso se usa
  injección de dependencias en la capa de interfaz API —ver por ejemplo en
  [`Program`](./src/TaskManagement.API/Program.cs) las llamadas a
  `builder.services...AddScoped<ITaskRepository, TaskRepository>()` y
  `builder.services...AddScoped<ITaskReadRepository, TaskReadRepository>()`—.
  Como ya fue mencionado antes, la abstracción
  [`IDomainEventDispatcher`](./src/TaskManagement.Application/Interfaces/IDomainEventDispatcher.cs)
  definida en la capa de aplicación se implementa con la clase
  [`MediatRDomainEventDispatcher`](./src/TaskManagement.Infrastructure/EventDispatching/MediatRDomainEventDispatcher.cs)
  de esta capa de infraestructura y la instancia se crea en tiempo de ejecución
  también con injección de dependencias en la capa de interfaz API en
  [`Program`](./src/TaskManagement.API/Program.cs) —ver
  `builder.services...AddScoped<IDomainEventDispatcher,
  MediatRDomainEventDispatcher>()`—. El proyecto
  [`TaskManagement.Infrastructure`](./src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj)
  referencia solamente el proyecto
  [`TaskManagement.Application`](./src/TaskManagement.Application/TaskManagement.Application.csproj),
  la dependencia es de un círculo externo a un círculo interno. Esta capa de
  infraestructura tiene también la configuración de los frameworks de acceso a
  datos.

4. [TaskManagement.API](./src/TaskManagement.API/TaskManagement.API.csproj) es
  una aplicación web en .NET en la que se define la interfaz, en esta demo, una
  API REST. El proyecto referencia tanto al proyecto
  [`TaskManagement.Application`](./src/TaskManagement.Application/TaskManagement.Application.csproj)
  como al proyecto
  [`TaskManagement.Infrastructure`](./src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj),
  ambos en círculos internos. Como toda aplicación web en .NET la carpeta
  [`Controllers`](./src/TaskManagement.API/Controllers/) contiene los
  controladores que implementan los *endpoint* de la API REST —ver por ejemplo
  [`TasksController.CreateTaskRequest`](./src/TaskManagement.API/Controllers/TasksController.cs)—.
  El archivo [`TaskManagement.http`](./TaskManagement.http) tiene ejemplos para
  invocar la API REST.

# Frameworks utilizados

La aplicación utiliza los siguientes frameworks.

## FluentValidation

[FluentValidation](https://github.com/FluentValidation/FluentValidation) es una
librería de .NET para constuir reglas de validación fuertemente tipadas.

## MediatR

[MediatR](https://github.com/LuckyPennySoftware/MediatR) es una librería de .NET
para mensajería en proceso —sin persistencia— sin dependencias. Admite
solicitudes y respuestas, comandos, consultas, notificaciones y eventos, tanto
síncronos como asíncronos, con despacho inteligente mediante tipos genéricos en
C#.

## Drapper

[Dapper](https://github.com/DapperLib/Dapper) es una librería de mapeo
objeto-relacional —ORM— de código abierto para aplicaciones .NET. Permite
acceder de forma rápida y sencilla a los datos de las bases de datos sin
necesidad de escribir código complejo.

# Resumen de clases e interfaces definidos en cada capa

## 1. Capa de dominio

La capa de dominio está definida [aquí](./src/TaskManagement.Domain/).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Entidades** | `Entities/TaskItem.cs`, `Entities/TaskStatus.cs`, `Entities/TaskPriority.cs` | Clases de modelo de negocio representando conceptos del dominio |
| **Objetos valor** | `ValueObjects/Email.cs`, `Shared/ValueObject.cs` | Objetos inmutables representando valores del dominio |
| **Eventos de dominio** | `Events/DomainEvent.cs` | Eventos representando ocurrencias importantes del dominio |
| **Interfaces** | `Interfaces/ITaskRepository.cs` | Abstracciones para usadas en el dominio e implementadas en infraestructura |
| **Excepciones** | `Exceptions/DomainException.cs` | Excepciones específicas del dominio |
| **Tipos compartidos** | `Shared/Result.cs`, `Shared/TaskErrors.cs` | Tipos comunes compartidos internamente en el dominio |

**Principio clave:** La capa de dominio **nunca depende** de las capas de
aplicación o infraestructura y contiene solo datos y reglas del negocio.

La entidad `TaskItem` es un [*aggregate
root*](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Aggregate.md)
que define y gestiona sus propios datos y reglas del negocio. Está definida así:

```csharp
public class TaskItem
{
    // Método factory para creación segura de entidad
    public static Result<TaskItem> Create(string title, string description, ...)
    {
        // La validación sucede en el dominio
        var validation = Validate(title, description, dueDate);
        if (validation.IsFailure)
            return Result.Failure<TaskItem>(validation.Errors);

        // La creación de la entidad genera un evento de dominio
        var task = new TaskItem { ... };
        task.AddDomainEvent(new TaskCreatedEvent(...));
        return Result.Success(task);
    }

    // Métodos de negocio que refuerzan invariantes
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

La capa de dominio utiliza el patrón
[Result](https://dev.to/adrianbailador/result-pattern-in-c-fal) para el
resultado de las operaciones. La clase `Result` en
[Result.cs](./src/TaskManagement.Domain/Shared/Result.cs) representa tanto
resultados exitosos —en cuyo caso incluye también el valor del resultado— como
errores —en cuyo caso incluye la lista de errores—. Esto permite que un método
pueda retornar tanto un resultado como un error.

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }
    …
}

// Uso
var result = TaskItem.Create(…);
if (result.IsSuccess)
    // Usar result.Value
else
    // Manejar result.Errors
```

## 2. Capa de aplicación

La capa de aplicación está definida [aquí](./src/TaskManagement.Application/).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Comandos** | `Commands/CreateTaskCommand.cs`, `Commands/CompleteTaskCommand.cs` | Objetos para casos de uso que cambian estado (CQRS) |
| **Consultas** | `Queries/GetTaskByIdQuery.cs` | Objetos para casos de uso de lecturas de datos (CQRS) |
| **DTO** | `Queries/GetTaskByIdQuery.cs` (TaskDto), `Requests/CreateTaskRequest.cs` | Objetos de transferencia de datos para entrada/salida |
| **Interfaces** | `Interfaces/IUnitOfWork.cs`, `Interfaces/ITaskReadRepository.cs`, `Interfaces/IEmailService.cs`, `Interfaces/IDomainEventDispatcher.cs` | Abstracciones para servicios de infraestructura y despacho de eventos |
| **Comportamientos** | `Behaviors/ValidationBehavior.cs` | Comportamientos de pipeline MediatR |
| **Excepciones** | `Exceptions/ValidationException.cs`, `Exceptions/NotFoundException.cs` | Excepciones específicas de la capa de aplicación |
| **Compartido** | `Shared/PagedResult.cs` | DTOs y tipos comunes |

**Principio clave:** La capa de aplicación implementa casos de uso de negocio
pero **delega la lógica de negocio a la capa de dominio**. Básicamente es un
orquestador de objetos del dominio, sin lógica del negocio.

#### Patrón CQRS (Command Query Responsibility Segregation):

El siguiente diagrama muestra una versión simplificada de un comando de la capa
de aplicación y su interacción con la capa de dominio.

![Diagrama de secuencia](https://www.plantuml.com/plantuml/png/bLPRJrf147uduJzCx0icL6dJz86eKK4hYKqDa3xnifwToiPbb-xkiLN3V--SlPmRO5iIm-pktBzl1i-jOyRbAmbpPXz71faJMitmPuugmsxdnBfdYMVT3j1dghM3ro27SwxuI1k51WRLzINB7XzCU9FQmDmmPTVCe7AL4afSqKNT7S2rLYxOl1iCFbzTNDSVBxNXQFO-tmg5UnutGQIla-DKJxUp51pX1vLqhtyNPvOvCZwucS_lO2G59NS0p4Qnsw42dCZ4cb7I1qMpx8YoDLfAD46nFBQKVLVHzrUlLapngExbbkxaAtB1A965wWLnUkZLq_tVVD_GNyeeoZjodpXSUPLqA1eGAeNtPsUrMePmzsqsXsDNLBnGhquuYNtWQ3CZrWLP-h0sGXlXdkawnp7biudz54zqQ3IcahVYV6TIy49_Mh3FEkg0ogM4R8dPOnzOxfOKvSKJz7279ry-326BqYae_4LE_YdZFkGMpOGVjZAF6BM29J26C4F5Up4YsNBSjgtvgq1CKiiTJHIffGgSXcM2i64GWHqM5VQQAPUOHy7h9vI7fNGKYDCx_FS6PTw2Nput0rOCIb8hP8dhVMvfHgq5aqkqAJ18L0zkMGQ2fqAVGeSmqYK7b1TSCo5jcEOgMgCZKxIvT4U5Ocxm96d53LOsfTNsfj6NNF1eKBMzJTVzDdMoU2i_LqFuD1ZGRkIHtjqEIelrucPvbY7ohVeQtPmeuGJPl21XfDT8W-rxEU5zc0SWntKWPuwvt9xEDTVzXRehOHqdM0CH-8KhHO_m6obSS0vZlM92NJrHOHIy_Er0xg-sx7T1vCkrjWuHXdcfJL5zF-q43xRyUIHwnRwQj4Qod8twEEt1jU6juQKQuZVNsQXOMs22jucD_XAdo-DRXFau-90mAJQlhsYpKBSwyuGPngryd9uYfI8b_uUCv1wDqSPkTe5GN-3TpZafq3yCtSuV)

La clase `CreateTaskCommandHandler` implementa un procesador de
[MediatR](https://github.com/LuckyPennySoftware/MediatR) para el comando
`CreateTaskCommand`.

```csharp
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    …

    public async Task<Result<Guid>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        …

        // Delega a la capa de dominio la lógica de negocio
        var createResult = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatedBy);

        // Persiste la entidad de dominio
        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Despacha los eventos de dominio
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);

        …
    }

    …
}
```

En ese método, las variables `_taskRepository` de tipo `ITaskRepository`
—definido en la capa de dominio—, `_unitOfWork` de tipo `IUnitOfWork` e
`_eventDispatcher` de tipo `IDomainEventDispatcher` —definidos en la propia capa
de aplicación— son asignadas mediante injección de dependencias con clases
implementadas en la capa de infraestructura.

## 3. Capa de infraestructura

La capa de infraestructura está definida [aquí](./src/TaskManagement.Infrastructure/).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **DbContext** | `Persistence/TaskDbContext.cs` | Contexto de base de datos de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Repositorios** | `Persistence/Repositories/TaskRepository.cs`, `TaskReadRepository.cs` | Implementaciones de acceso a datos con [Entity Framework](https://learn.microsoft.com/en-us/ef/) y [Dapper](https://github.com/DapperLib/Dapper)  respectivamente |
| **Configuraciones** | `Persistence/Configuration/TaskConfiguration.cs` | Configuraciones de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Despacho de eventos** | `EventDispatching/MediatRDomainEventDispatcher.cs` | Implementación de publicación de eventos de dominio con [MediatR](https://github.com/LuckyPennySoftware/MediatR) |

**Principio clave:** La capa de infraestructura implementa interfaces definidas
en las capas de dominio y aplicación usando [inyección de
dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview).

Esta capa implementa el patrón
[Repository](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Repository.md).
La interfaz `ITaskRepository` definida en la capa de dominio es implementada por
la clase `TaskRepository` definida en esta capa de infraestructura.

```csharp
// Interfaz definida en capa de dominio
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    …
}

// Implementación en la capa de infraestructura
public sealed class TaskRepository : ITaskRepository
{
    // Los detalles de infraestructura como el uso de Entity Framework quedan
    // ocultos del dominio y de la aplicación
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
    …
}
```

## 4. Capa de interfaz

En esta demo la interfaz es una API web. La capa de interfaz está definida
[aquí](./src/TaskManagement.API/).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Controladores** | `Controllers/TasksController.cs` | Endpoints HTTP |
| **Middleware** | `Middleware/ExceptionHandlingMiddleware.cs` | Comportamientos de pipeline transversales |
| **Extensiones** | `Extensions/ClaimsPrincipalExtensions.cs` | Métodos de extensión auxiliares |
| **Solicitudes** | `Requests/CreateTaskRequest.cs` | DTOs de entrada para solicitudes HTTP |
| **Inicio** | `Program.cs` | Raíz de composición de inyección de dependencias |

**Principio clave:** La capa de UI traduce HTTP a comandos o consultas de
aplicación. **No tiene lógica de negocio** y delega todo a la capa de
aplicación.

A continuación un fragmento de la clase `TaskController` con la implementación
del POST de HTTP para crear un cliente —hay ejemplos para probar todos los
endpoints [aquí](TaskManagement.http)—.

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
        // Traduce la solicitud HTTP a un comando de la aplicación
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            userId);

        // Envía el comando a la capa de aplicación
        var result = await _mediator.Send(command, cancellationToken);

        // Traduce resultado a una respuesta HTTP
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

        return BadRequest(CreateProblemDetails(result.Errors));
    }

    …
}
```

En el programa principal en [Program.cs](./src/TaskManagement.API/Program.cs) se
realiza la inyección de dependencias.

```csharp
// Registra servicios para las abstracciones de la capa de dominio
builder.Services
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

// Registra servicios para las abstracciones de la capa de aplicación
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))
    .AddValidatorsFromAssembly(...)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Registra servicios para las abstracciones de la capa de infraestructura
builder.Services
    .AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(connectionString));
```

# Cómo navegar el código

1. Comienza con la capa de dominio [`TaskManagement.Domain/`](./src/TaskManagement.Domain/)
   - Lee `Entities/TaskItem.cs` para entender el modelo de dominio
   - Lee `Shared/Result.cs` para entender el manejo de errores
   - Lee `Events/DomainEvent.cs` para entender la arquitectura dirigida por eventos

2. Muévete a la capa de aplicación** [`TaskManagement.Application/`](./src/TaskManagement.Application/)
   - Lee `Commands/CreateTaskCommand.cs` para ver cómo se implementan los casos de uso
   - Lee `Behaviors/ValidationBehavior.cs` para entender preocupaciones transversales
   - Lee `Queries/GetTaskByIdQuery.cs` para ver el patrón CQRS

3. Explora la capa de infraestructura** [`TaskManagement.Infrastructure/`](./src/TaskManagement.Infrastructure/)
   - Lee `Persistence/Repositories/TaskRepository.cs` para ver cómo se implementan interfaces de dominio
   - Lee `Persistence/TaskDbContext.cs` para ver la configuración de base de datos
   - Lee `EventDispatching/MediatRDomainEventDispatcher.cs` para ver el manejo de eventos

4. Revisa la capa de interfaz API [`TaskManagement.API/`](./src/TaskManagement.API/)
   - Lee `Controllers/TasksController.cs` para ver endpoints HTTP
   - Lee `Program.cs` para ver la configuración de inyección de dependencias
   - Lee `Middleware/ExceptionHandlingMiddleware.cs` para ver el manejo de errores

# Beneficios de Clean Architecture

Esta demo permite entender los siguientes beneficios de Clean Architecture:

* **Facilidad de testeo**: La lógica de dominio sin dependencias externas
  facilita escribir casos de prueba.

*  **Facilidad de mantenimiento**: Separación clara de responsabilidades y
   aspectos.

*  **Flexibilidad**: Fácil de intercambiar implementaciones —base de datos,
   servicio de email, etc.—

*  **Escalabilidad**: Cada capa se puede optimizar de forma independiente.

*  **Reutilización**: La lógica de dominio se puede reutilizar en diferentes UIs
   —web, consola, API, etc.—

*  **Independencia**: La lógica de negocio está aislada de los
   [frameworks](#frameworks-utilizados).
