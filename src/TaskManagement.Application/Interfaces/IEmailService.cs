namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>IEmailService</c> es la abstracción para envío de notificaciones por correo electrónico.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Abstracción de infraestructura: Interfaz en Núcleo de Aplicación, implementación en Infraestructura
/// - Inversión de dependencia: El código de aplicación no sabe detalles del proveedor de correo
/// - Habilita acoplamiento suelto a servicios de correo externos
///
/// Responsabilidades:
/// - Enviar correos para eventos de dominio (tarea creada, completada, asignada, etc.)
/// - Soportar futuras notificaciones de eventos de dominio sin cambios de capa de dominio
/// - Manejar entrega de correo de forma asíncróna
///
/// Beneficios de Abstracción de Implementación:
/// - Puede intercambiar proveedores de correo (SMTP, SendGrid, AWS SES, etc.)
/// - Puede implementar patrones de reintentos y resiliencia
/// - Puede agregar plantillas y formateo de correo
/// - Puede registrar resultados de entrega de correo
/// - Las pruebas unitarias pueden simular sin envío real de correo
///
/// Preocupación Transversal:
/// - Las notificaciones por correo son efectos secundarios de eventos de dominio
/// - Se manejan a través de manejadores de eventos que dependen de IEmailService
/// - Desacopla la lógica de negocio de la infraestructura de envío de correos
/// </summary>

public interface IEmailService
{
    Task SendTaskCompletedNotificationAsync(Guid taskId, CancellationToken cancellationToken = default);
}
