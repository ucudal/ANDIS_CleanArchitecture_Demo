using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Requests;

/// <summary>
/// CreateTaskRequest es el DTO de entrada para crear una nueva tarea a través de API HTTP.
///
/// Rol en Clean Architecture:
/// - Parte de la capa de presentación (UI Layer)
/// - DTO de entrada: Transfiere datos de solicitud HTTP a capa de aplicación
/// - Vinculación de solicitud: ASP.NET Core deserializa JSON a este tipo
/// - Desacopla contrato de API de capa de aplicación
///
/// DTOs de Solicitud vs Comandos:
/// - CreateTaskRequest: Formato de solicitud HTTP, estructura específíca de API
/// - CreateTaskCommand: Comando de aplicación, estructura consciente del dominio
/// - El controlador transforma solicitud a comando
/// - Separación permite evolución independiente de API y aplicación
///
/// Beneficios de DTOs de Entrada:
/// - Contratos de API separados de modelos de aplicación
/// - Puede agregar validación específíca de API (basada en atributos)
/// - Puede incluir documentación de API (DataAnnotations)
/// - Puede transformar a diferentes estructuras de comando
/// - Compatibilidad hacia atrás al cambiar modelos internos
///
/// Flujo de Datos:
/// 1. Cliente HTTP envía JSON en cuerpo de solicitud
/// 2. ASP.NET Core deserializa JSON a <see cref="CreateTaskRequest"/>
/// 3. Validación de modelo aplicada (anotaciones de datos)
/// 4. Controlador transforma a <see cref="TaskManagement.Application.Commands.CreateTask.CreateTaskCommand"/>
/// 5. Comando enviado a capa de aplicación a través de <see cref="MediatR"/>
/// 6. Validación de capa de aplicación (FluentValidation, reglas de dominio)
/// 7. Lógica de dominio ejecutada
///
/// Notas:
/// - Las propiedades deben coincidir con nombres de campo JSON (o usar [JsonPropertyName])
/// - Seguridad de tipo asegura que datos inválidos se rechacen temprano
/// - Anotaciones de datos documentan campos requeridos y restricciones
/// </summary>

internal sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate
);
