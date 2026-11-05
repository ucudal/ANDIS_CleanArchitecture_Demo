using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Application.UnitTests;

/// <summary>
/// Pruebas unitarias de las reglas de negocio de la entidad <c>TaskItem</c>. No requieren base de
/// datos, servidor web ni acceso a la red.
/// </summary>
[TestFixture]
public class TaskItemTests
{
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
