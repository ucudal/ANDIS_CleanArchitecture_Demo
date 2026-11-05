using TaskManagement.Application.Commands;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Application.UnitTests;

[TestFixture]
public class CompleteTaskInteractorTests
{
    private FakeTaskRepository _repository = null!;
    private FakeUnitOfWork _unitOfWork = null!;
    private FakeEventDispatcher _dispatcher = null!;
    private FakeCompleteTaskPresenter _presenter = null!;
    private CompleteTaskInteractor _interactor = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new FakeTaskRepository();
        _unitOfWork = new FakeUnitOfWork();
        _dispatcher = new FakeEventDispatcher();
        _presenter = new FakeCompleteTaskPresenter();
        _interactor = new CompleteTaskInteractor(_repository, _unitOfWork, _dispatcher, _presenter);
    }

    private TaskItem AddTask()
    {
        var task = TaskItem.Create("Title", "Description", TaskPriority.Low, null, Guid.NewGuid()).Value!;
        task.ClearDomainEvents();
        _repository.Tasks[task.Id] = task;
        return task;
    }

    [Test]
    public async Task ExecuteAsync_WhenTaskExists_CompletesSavesPublishesAndPresentsSuccess()
    {
        var task = AddTask();

        await _interactor.ExecuteAsync(new CompleteTaskInput(task.Id));

        Assert.That(task.Status, Is.EqualTo(TaskStatus.Completed));
        Assert.That(_repository.UpdateCount, Is.EqualTo(1));
        Assert.That(_unitOfWork.SaveCount, Is.EqualTo(1));
        Assert.That(_dispatcher.Events.Single(), Is.TypeOf<TaskCompletedEvent>());
        Assert.That(_presenter.Succeeded, Is.True);
    }

    [Test]
    public async Task ExecuteAsync_WhenTaskDoesNotExist_PresentsNotFound()
    {
        var id = Guid.NewGuid();

        await _interactor.ExecuteAsync(new CompleteTaskInput(id));

        Assert.That(_presenter.NotFoundMessage, Is.EqualTo(TaskErrors.NotFound(id)));
        Assert.That(_presenter.Succeeded, Is.False);
        Assert.That(_unitOfWork.SaveCount, Is.Zero);
    }

    [Test]
    public async Task ExecuteAsync_WhenAlreadyCompleted_PresentsRuleViolationAndSavesNothing()
    {
        var task = AddTask();
        task.Complete();
        task.ClearDomainEvents();

        await _interactor.ExecuteAsync(new CompleteTaskInput(task.Id));

        Assert.That(_presenter.RuleViolations, Does.Contain(TaskErrors.AlreadyCompleted));
        Assert.That(_presenter.Succeeded, Is.False);
        Assert.That(_unitOfWork.SaveCount, Is.Zero);
        Assert.That(_dispatcher.Events, Is.Empty);
    }
}
