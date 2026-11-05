using TaskManagement.Application.Queries;
using TaskManagement.Domain.Common;

namespace TaskManagement.Application.UnitTests;

[TestFixture]
public class GetTaskByIdInteractorTests
{
    [Test]
    public async Task ExecuteAsync_WhenTaskExists_PresentsTask()
    {
        var task = new TaskDto { Title = "Title" };
        var presenter = new FakeGetTaskByIdPresenter();
        var interactor = new GetTaskByIdInteractor(new FakeTaskReadRepository { Task = task }, presenter);

        await interactor.ExecuteAsync(new GetTaskByIdInput(Guid.NewGuid()));

        Assert.That(presenter.Task, Is.SameAs(task));
        Assert.That(presenter.NotFoundMessage, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WhenTaskDoesNotExist_PresentsNotFound()
    {
        var id = Guid.NewGuid();
        var presenter = new FakeGetTaskByIdPresenter();
        var interactor = new GetTaskByIdInteractor(new FakeTaskReadRepository(), presenter);

        await interactor.ExecuteAsync(new GetTaskByIdInput(id));

        Assert.That(presenter.NotFoundMessage, Is.EqualTo(TaskErrors.NotFound(id)));
        Assert.That(presenter.Task, Is.Null);
    }
}
