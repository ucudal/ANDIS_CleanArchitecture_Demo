using Microsoft.Extensions.Logging;
using TaskManagement.Application.Ports;

namespace TaskManagement.Infrastructure.Email;

/// <summary>
/// <c>LoggingEmailService</c> es una implementación de IEmailService que, en
/// lugar de enviar correos reales, registra las notificaciones en el log.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de Infraestructura</li>
/// <li>Implementa la interfaz IEmailService -definida en la capa de
/// aplicación-</li>
/// <li>Puede reemplazarse por una implementación real -SMTP, SendGrid, etc.-
/// sin modificar las capas de aplicación ni del dominio</li>
/// </ul>
/// </remarks>
public sealed class LoggingEmailService : IEmailService
{
    private static readonly Action<ILogger, Guid, Exception?> LogTaskCompletedEmail =
        LoggerMessage.Define<Guid>(
            LogLevel.Information,
            new EventId(2001, nameof(SendTaskCompletedNotificationAsync)),
            "Email sent: task {TaskId} completed");

    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendTaskCompletedNotificationAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        LogTaskCompletedEmail(_logger, taskId, null);
        return Task.CompletedTask;
    }
}
