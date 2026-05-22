using System.Security.Claims;

namespace TaskManagement.API.Extensions;

/// <summary>
/// ClaimsPrincipalExtensions proporciona métodos de extensión para extraer información de usuario de reclamaciones.
///
/// Rol en Clean Architecture:
/// - Parte de la capa de presentación (Capa de API)
/// - Métodos de extensión: Proporciona ayudantes convenientes para extracción de reclamaciones
/// - Encapsula lógica de navegación de reclamaciones
/// - Reduce repetición en controladores y servicios
///
/// Autenticación Basada en Reclamaciones:
/// - ClaimsPrincipal: Representa usuario autenticado con sus reclamaciones
/// - Reclamaciones: Piezas individuales de información sobre el usuario
/// - Los tokens JWT contienen reclamaciones: ID de usuario, correo, roles, etc.
/// - Las extensiones simplifican la extracción de reclamaciones específicas
///
/// Beneficios de Diseño:
/// - Fuente única de verdad para lógica de extracción de reclamaciones
/// - Manejo consistente de reclamaciones en toda la aplicación
/// - Acceso seguro de tipo a reclamaciones
/// - Fácil modificar estructura de reclamaciones
/// - Encapsula lógica relacionada con seguridad
///
/// Extensiones Comunes:
/// - GetUserId(): Extraer ID de usuario de reclamaciones
/// - GetEmail(): Extraer correo de reclamaciones
/// - GetRoles(): Extraer roles de usuario de reclamaciones
///
/// Consideraciones de Seguridad:
/// - Solo extraer reclamaciones ya presentes en token
/// - Sin modificaciones de reclamaciones (solo lectura)
/// - Validar token en entrada de solicitud (hecho por middleware de autenticación)
/// - Las extensiones asumen que el usuario ya está autenticado
/// </summary>

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // Soportar nombres comunes de reclamación JWT/user-jwts para ID de usuario.
        var candidateValues = user.Claims
            .Where(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "sub" ||
                c.Type == "nameid" ||
                c.Type == "oid" ||
                c.Type == "uid" ||
                c.Type == "userId")
            .Select(c => c.Value);

        foreach (var candidate in candidateValues)
        {
            if (Guid.TryParse(candidate, out var parsedUserId))
            {
                return parsedUserId;
            }
        }

        throw new InvalidOperationException("The current user does not have a valid GUID user id claim.");
    }
}
