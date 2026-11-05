using TaskManagement.Application.Commands;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;

namespace TaskManagement.Application.UnitTests;

[TestFixture]
public class CreateTaskInteractorTests
{
    private FakeTaskRepository _repository = null!;
    private FakeUnitOfWork _unitOfWork = null!;
    private FakeEventDispatcher _dispatcher = null!;
    private FakeCreateTaskPresenter _presenter = null!;
    private CreateTaskInteractor _interactor = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new FakeTaskRepository();
        _unitOfWork = new FakeUnitOfWork();
        _dispatcher = new FakeEventDispatcher();
        _presenter = new FakeCreateTaskPresenter();
        _interactor = new CreateTaskInteractor(_repository, _unitOfWork, _dispatcher, _presenter);
    }

    [Test]
    public async Task ExecuteAsync_WithValidInput_SavesTaskPublishesEventAndPresentsId()
    {
        var input = new CreateTaskInput("Title", "Description", TaskPriorityLevel.High, null, Guid.NewGuid());

        await _interactor.ExecuteAsync(input);

        var task = _repository.Tasks.Values.Single();
        Assert.That(task.Priority, Is.EqualTo(TaskPriority.High));
        Assert.That(_unitOfWork.SaveCount, Is.EqualTo(1));
        Assert.That(_dispatcher.Events.Single(), Is.TypeOf<TaskCreatedEvent>());
        Assert.That(_presenter.Output!.TaskId, Is.EqualTo(task.Id));
        Assert.That(_presenter.Errors, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WhenEntityRejectsInput_PresentsErrorsAndSavesNothing()
    {
        var input = new CreateTaskInput(" ", "Description", TaskPriorityLevel.Low, null, Guid.NewGuid());

        await _interactor.ExecuteAsync(input);

        Assert.That(_presenter.Errors, Does.Contain("Title is required."));
        Assert.That(_presenter.Output, Is.Null);
        Assert.That(_repository.Tasks, Is.Empty);
        Assert.That(_unitOfWork.SaveCount, Is.Zero);
        Assert.That(_dispatcher.Events, Is.Empty);
    }
}
