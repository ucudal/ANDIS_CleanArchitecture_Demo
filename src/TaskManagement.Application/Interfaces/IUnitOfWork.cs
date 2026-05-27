namespace TaskManagement.Application.Interfaces;

/// <summary>
/// <c>IUnitOfWork</c> es la abstracción para coordinar la persistencia de cambios de entidades.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>Define contrato para guardar cambios en almacén de persistencia</li>
/// <li>Abstracción de implementación: Interfaz en Núcleo de Aplicación, implementación en Infraestructura</li>
/// <li>Inversión de dependencia: La aplicación depende de interfaz, no de tecnología de persistencia concreta</li>
/// </ul>
///
/// Beneficios del Patrón Unit of Work:
/// <ul>
/// <li>Coordina cambios de múltiples repositorios en una sola transacción atómica</li>
/// <li>Asegura persistencia de todo o nada: todos los cambios tienen éxito o todos se revierten</li>
/// <li>Mantiene consistencia en múltiples agregados de dominio</li>
/// <li>Abstrae gestión de transacción de base de datos del código de aplicación</li>
/// </ul>
///
/// Uso Típico:
/// <ul>
/// <li>Después de modificar entidades de dominio a través de repositorios</li>
/// <li>Antes de ejecutar eventos de dominio que dependen de persistencia exitosa</li>
/// <li>Los servicios de aplicación llaman a SaveChangesAsync después de coordinar operaciones de dominio</li>
/// </ul>
///
/// En esta implementación:
/// <ul>
/// <li><c>TaskDbContext</c> implementa tanto <c>DbContext</c> como <see cref="IUnitOfWork"/></li>
/// <li><see cref="IUnitOfWork.SaveChangesAsync"/> envuelve Entity Framework.</li>
/// </ul>
/// </remarks>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
