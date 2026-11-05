using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Domain.UnitTests;

/// <summary>
/// Pruebas unitarias de las reglas de negocio de la entidad <c>TaskItem</c>. No requieren base de
/// datos, servidor web ni acceso a la red.
/// </summary>
[TestFixture]
public class TaskItemTests
{
    private static readonly string[] ExpectedErrors = [
        "Title is required.",
        "Due date cannot be in the past.",
        "CreatedBy is required."
    ];

    private static TaskItem CreateValidTask() =>
        TaskItem.Create("Title", "Description", TaskPriority.Medium, null, Guid.NewGuid()).Value!;

    [Test]
    public void Create_WithValidData_ReturnsTaskInTodoStatusAndRaisesEvent()
    {
        var result = TaskItem.Create(" Title ", " Description ", TaskPriority.High, null, Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value!.Title, Is.EqualTo("Title"));
        Assert.That(result.Value.Status, Is.EqualTo(TaskStatus.Todo));
        Assert.That(result.Value.DomainEvents.Single(), Is.TypeOf<TaskCreatedEvent>());
    }

    [Test]
    public void Create_WithoutTitle_ReturnsFailure()
    {
        var result = TaskItem.Create(" ", "Description", TaskPriority.Low, null, Guid.NewGuid());

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Errors, Does.Contain("Title is required."));
    }

    [Test]
    public void Create_WithoutCreator_ReturnsFailure()
    {
        var result = TaskItem.Create("Title", "Description", TaskPriority.Low, null, Guid.Empty);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Errors, Does.Contain("CreatedBy is required."));
    }

    [Test]
    public void Create_WithSeveralInvalidFields_ReturnsAllErrors()
    {
        var result = TaskItem.Create(" ", "Description", TaskPriority.Low, DateTime.UtcNow.AddDays(-2), Guid.Empty);

        Assert.That(result.Errors, Is.EquivalentTo(ExpectedErrors));
    }

    [Test]
    public void Complete_WhenNotCompleted_CompletesTaskAndRaisesEvent()
    {
        var task = CreateValidTask();
        task.ClearDomainEvents();

        var result = task.Complete();

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(task.Status, Is.EqualTo(TaskStatus.Completed));
        Assert.That(task.DomainEvents.Single(), Is.TypeOf<TaskCompletedEvent>());
    }

    [Test]
    public void Complete_WhenAlreadyCompleted_ReturnsFailure()
    {
        var task = CreateValidTask();
        task.Complete();

        var result = task.Complete();

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Errors, Does.Contain(TaskErrors.AlreadyCompleted));
    }
}
