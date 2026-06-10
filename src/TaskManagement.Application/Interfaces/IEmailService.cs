namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>IEmailService</c> es la abstracción para envío de notificaciones por correo electrónico.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>Abstracción de infraestructura: Interfaz en Núcleo de Aplicación, implementación en Infraestructura</li>
/// <li>Inversión de dependencia: El código de aplicación no sabe detalles del proveedor de correo</li>
/// <li>Habilita acoplamiento suelto a servicios de correo externos</li>
/// </ul>
///
/// Responsabilidades:
/// <ul>
/// <li>Enviar correos para eventos del dominio -tarea creada, completada, asignada, etc.-</li>
/// <li>Soportar futuras notificaciones de eventos del dominio sin cambios de capa del dominio</li>
/// <li>Manejar entrega de correo de forma asíncróna</li>
/// </ul>
///
/// Beneficios de Abstracción de Implementación:
/// <ul>
/// <li>Puede intercambiar proveedores de correo -SMTP, SendGrid, AWS SES, etc.-</li>
/// <li>Puede implementar patrones de reintentos y resiliencia</li>
/// <li>Puede agregar plantillas y formateo de correo</li>
/// <li>Puede registrar resultados de entrega de correo</li>
/// <li>Las pruebas unitarias pueden simular sin envío real de correo</li>
/// </ul>
///
/// Preocupación Transversal:
/// <ul>
/// <li>Las notificaciones por correo son efectos secundarios de eventos del dominio</li>
/// <li>Se manejan a través de manejadores de eventos que dependen de IEmailService</li>
/// <li>Desacopla la lógica de negocio de la infraestructura de envío de correos</li>
/// </ul>
/// </remarks>
public interface IEmailService
{
    Task SendTaskCompletedNotificationAsync(Guid taskId, CancellationToken cancellationToken = default);
}
