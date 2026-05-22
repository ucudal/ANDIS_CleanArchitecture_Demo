namespace TaskManagement.Application.Interfaces;

/// <summary>
/// IUnitOfWork es la abstracción para coordinar la persistencia de cambios de entidades.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - Define contrato para guardar cambios en almacén de persistencia
/// - Abstracción de implementación: Interfaz en Núcleo de Aplicación, implementación en Infraestructura
/// - Inversión de dependencia: La aplicación depende de interfaz, no de tecnología de persistencia concreta
///
/// Beneficios del Patrón Unit of Work:
/// - Coordina cambios de múltiples repositorios en una sola transacción atómica
/// - Asegura persistencia de todo o nada: todos los cambios tienen éxito o todos se revierten
/// - Mantiene consistencia en múltiples agregados de dominio
/// - Abstrae gestión de transacción de base de datos del código de aplicación
///
/// Uso Típico:
/// - Después de modificar entidades de dominio a través de repositorios
/// - Antes de ejecutar eventos de dominio que dependen de persistencia exitosa
/// - Los servicios de aplicación llaman a SaveChangesAsync después de coordinar operaciones de dominio
///
/// En esta implementación:
/// - <c>TaskDbContext</c> implementa tanto <c>DbContext</c> como <see cref="IUnitOfWork"/>
/// - <see cref="IUnitOfWork.SaveChangesAsync"/> envuelve Entity Framework.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
