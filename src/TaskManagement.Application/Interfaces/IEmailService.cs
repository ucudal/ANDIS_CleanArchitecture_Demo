namespace TaskManagement.Application.Interfaces;

/// <summary>
/// IEmailService is the abstraction for sending email notifications.
///
/// Role in Clean Architecture:
/// - Part of the Application Core (Application Layer)
/// - Infrastructure abstraction: Interface in Application Core, implementation in Infrastructure
/// - Dependency inversion: Application code doesn't know about email provider details
/// - Enables loose coupling to external email services
///
/// Responsibilities:
/// - Send emails for domain events (task created, completed, assigned, etc.)
/// - Support future domain event notifications without domain layer changes
/// - Handle email delivery asynchronously
///
/// Implementation Abstraction Benefits:
/// - Can swap email providers (SMTP, SendGrid, AWS SES, etc.)
/// - Can implement retries and resilience patterns
/// - Can add email templating and formatting
/// - Can log email delivery results
/// - Unit tests can mock without actual email sending
///
/// Cross-Cutting Concern:
/// - Email notifications are side effects of domain events
/// - Handled through event handlers that depend on IEmailService
/// - Supports eventual consistency: emails sent asynchronously after persistence
/// </summary>

public interface IEmailService
{
    Task SendTaskCompletedNotificationAsync(Guid taskId, CancellationToken cancellationToken = default);
}
