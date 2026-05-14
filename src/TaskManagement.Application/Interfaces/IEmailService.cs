namespace TaskManagement.Application.Interfaces;

public interface IEmailService
{
    Task SendTaskCompletedNotificationAsync(Guid taskId, CancellationToken cancellationToken = default);
}
