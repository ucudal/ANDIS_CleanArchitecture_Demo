using TaskManagement.Application.Commands;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.UnitTests;

[TestFixture]
public class ValidatingCreateTaskInteractorTests
{
    private static readonly string[] ExpectedErrors = [
        "Title is required.",
        "Due date cannot be in the past.",
        "CreatedBy is required."
    ];

    private SpyCreateTaskInputBoundary _next = null!;
    private FakeCreateTaskPresenter _presenter = null!;
    private ValidatingCreateTaskInteractor _decorator = null!;

    [SetUp]
    public void SetUp()
    {
        _next = new SpyCreateTaskInputBoundary();
        _presenter = new FakeCreateTaskPresenter();
        _decorator = new ValidatingCreateTaskInteractor(_next, _presenter);
    }

    [Test]
    public async Task ExecuteAsync_WithValidInput_CallsNextInteractor()
    {
        await _decorator.ExecuteAsync(
            new CreateTaskInput("Title", "Description", TaskPriorityLevel.Low, null, Guid.NewGuid()));

        Assert.That(_next.WasCalled, Is.True);
        Assert.That(_presenter.Errors, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithSeveralInvalidFields_PresentsAllErrorsAndStops()
    {
        await _decorator.ExecuteAsync(
            new CreateTaskInput(" ", "Description", TaskPriorityLevel.Low, DateTime.UtcNow.AddDays(-2), Guid.Empty));

        Assert.That(_next.WasCalled, Is.False);
        Assert.That(_presenter.Errors, Is.EquivalentTo(ExpectedErrors));
    }

    [Test]
    public async Task ExecuteAsync_WithUndefinedPriority_PresentsError()
    {
        await _decorator.ExecuteAsync(
            new CreateTaskInput("Title", "Description", (TaskPriorityLevel)99, null, Guid.NewGuid()));

        Assert.That(_next.WasCalled, Is.False);
        Assert.That(_presenter.Errors, Does.Contain("Priority is not valid."));
    }
}
