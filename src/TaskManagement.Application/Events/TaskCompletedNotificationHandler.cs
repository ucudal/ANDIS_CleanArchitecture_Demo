using TaskManagement.Application.Ports;
using TaskManagement.Domain.Events;

namespace TaskManagement.Application.Events;

/// <summary>
/// <c>TaskCompletedNotificationHandler</c> envía una notificación cuando se completa una tarea.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación: notificar al completar una tarea es una política de la
/// aplicación, no un detalle técnico</li>
/// <li>Usa el puerto de salida IEmailService; no sabe cómo se envía la notificación</li>
/// </ul>
/// </remarks>
public sealed class TaskCompletedNotificationHandler : IDomainEventHandler<TaskCompletedEvent>
{
    private readonly IEmailService _emailService;

    public TaskCompletedNotificationHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task HandleAsync(TaskCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        return _emailService.SendTaskCompletedNotificationAsync(domainEvent.TaskId, cancellationToken);
    }
}
