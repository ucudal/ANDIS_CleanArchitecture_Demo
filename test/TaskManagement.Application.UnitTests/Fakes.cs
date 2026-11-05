using TaskManagement.Application.Commands;
using TaskManagement.Application.Ports;
using TaskManagement.Application.Queries;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;

namespace TaskManagement.Application.UnitTests;

// Dobles de prueba de los puertos de salida. Reemplazan a los adaptadores de Infrastructure y API.

internal sealed class FakeTaskRepository : ITaskRepository
{
    public Dictionary<Guid, TaskItem> Tasks { get; } = [];

    public int UpdateCount { get; private set; }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Tasks.GetValueOrDefault(id));

    public Task<IReadOnlyList<TaskItem>> GetByAssigneeAsync(Guid userId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<TaskItem>> GetOverdueAsync(CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        Tasks[task.Id] = task;
        return Task.CompletedTask;
    }

    public void Update(TaskItem task) => UpdateCount++;

    public void Delete(TaskItem task) => Tasks.Remove(task.Id);
}

internal sealed class FakeTaskReadRepository : ITaskReadRepository
{
    public TaskDto? Task { get; set; }

    public Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        System.Threading.Tasks.Task.FromResult(Task);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FakeEventDispatcher : IDomainEventDispatcher
{
    public List<DomainEvent> Events { get; } = [];

    public Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken cancellationToken = default)
    {
        Events.AddRange(events);
        return Task.CompletedTask;
    }
}

internal sealed class FakeCreateTaskPresenter : ICreateTaskOutputBoundary
{
    public CreateTaskOutput? Output { get; private set; }

    public IReadOnlyList<string>? Errors { get; private set; }

    public void PresentSuccess(CreateTaskOutput output) => Output = output;

    public void PresentValidationErrors(IReadOnlyList<string> errors) => Errors = errors;
}

internal sealed class FakeCompleteTaskPresenter : ICompleteTaskOutputBoundary
{
    public bool Succeeded { get; private set; }

    public string? NotFoundMessage { get; private set; }

    public IReadOnlyList<string>? RuleViolations { get; private set; }

    public void PresentSuccess() => Succeeded = true;

    public void PresentNotFound(string message) => NotFoundMessage = message;

    public void PresentRuleViolation(IReadOnlyList<string> errors) => RuleViolations = errors;
}

internal sealed class FakeGetTaskByIdPresenter : IGetTaskByIdOutputBoundary
{
    public TaskDto? Task { get; private set; }

    public string? NotFoundMessage { get; private set; }

    public void PresentSuccess(TaskDto task) => Task = task;

    public void PresentNotFound(string message) => NotFoundMessage = message;
}

internal sealed class SpyCreateTaskInputBoundary : ICreateTaskInputBoundary
{
    public bool WasCalled { get; private set; }

    public Task ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        return Task.CompletedTask;
    }
}
