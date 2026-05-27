using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Requests;

/// <summary>
/// <c>CreateTaskRequest</c> es el DTO de los datos de entrada de la solicitud
/// HTTP para crear una nueva tarea a través de la API REST.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, API Layer</li>
/// <li>DTO de entrada: contiene los datos de la solicitud HTTP que va a la capa de aplicación</li>
/// <li>ASP.NET Core deserializa JSON a este tipo</li>
/// <li>Desacopla contrato de API de capa de aplicación</li>
/// </ul>
///
/// Beneficios de DTOs de Entrada:
/// <ul>
/// <li>Contratos de API separados de modelos de aplicación</li>
/// <li>Puede agregar validación específíca de API (basada en atributos)</li>
/// <li>Puede incluir documentación de API (DataAnnotations)</li>
/// <li>Puede transformar a diferentes estructuras de comando</li>
/// <li>Compatibilidad hacia atrás al cambiar modelos internos</li>
/// </ul>
///
/// Flujo de Datos:
/// <ol>
/// <li>Cliente HTTP envía JSON en cuerpo de solicitud</li>
/// <li>ASP.NET Core deserializa JSON a <see cref="CreateTaskRequest"/></li>
/// <li>Validación de modelo aplicada (anotaciones de datos)</li>
/// <li>Controlador transforma a <see cref="TaskManagement.Application.Commands.CreateTaskCommand"/></li>
/// <li>Comando enviado a capa de aplicación a través de <see cref="MediatR"/></li>
/// <li>Validación de capa de aplicación (FluentValidation, reglas de dominio)</li>
/// <li>Lógica de dominio ejecutada</li>
/// </ol>
///
/// Notas:
/// <ul>
/// <li>Las propiedades deben coincidir con nombres de campo JSON (o usar [JsonPropertyName])</li>
/// <li>Seguridad de tipo asegura que datos inválidos se rechacen temprano</li>
/// <li>Anotaciones de datos documentan campos requeridos y restricciones</li>
/// </ul>
/// </remarks>
public sealed record CreateTaskRequest(
    string Title,
    string Description,
    TaskPriority Priority,
    DateTime? DueDate
);
