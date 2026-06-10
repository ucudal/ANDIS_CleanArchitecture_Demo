using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Requests;

/// <summary>
/// <c>CreateTaskRequest</c> es el DTO de los datos de entrada de la solicitud
/// HTTP para crear una nueva tarea a través de la %API REST.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de presentación, en este caso, %API Layer</li>
/// <li>DTO de entrada: contiene los datos de la solicitud HTTP que va a la capa de aplicación</li>
/// <li>ASP.NET Core deserializa JSON a este tipo</li>
/// <li>Desacopla contrato de %API de capa de aplicación</li>
/// </ul>
///
/// Flujo de Datos:
/// <ol>
/// <li>Cliente HTTP envía JSON en cuerpo de solicitud</li>
/// <li>ASP.NET Core deserializa JSON a <c>CreateTaskRequest</c></li>
/// <li>Validación de modelo aplicada con
/// <a href="https://learn.microsoft.com/en-us/previous-versions/aspnet/ee256141(v=vs.100)?redirectedfrom=MSDN">
/// anotaciones de datos</a></li>
/// <li>Controlador transforma a <c>CreateTaskCommand</c></li>
/// <li>Comando enviado a capa de aplicación a través de <a href="https://mediatr.io">MediatR</a></li>
/// <li>Validación de capa de aplicación con <a
/// href="https://github.com/FluentValidation/FluentValidation">FluentValidation</a>
/// de las reglas del dominio</li>
/// <li>Lógica del dominio ejecutada</li>
/// </ol>
///
/// Beneficios de DTO de Entrada:
/// <ul>
/// <li>Contratos de %API separados de modelos de aplicación</li>
/// <li>Puede agregar validación específíca de %API basada en atributos</li>
/// <li>Puede incluir documentación de %API DataAnnotations</li>
/// <li>Puede transformar a diferentes estructuras de comando</li>
/// <li>Compatibilidad hacia atrás al cambiar modelos internos</li>
/// </ul>
///
/// Notas:
/// <ul>
/// <li>Las propiedades deben coincidir con nombres de campo JSON o usar <c>JsonPropertyName</c></li>
/// <li>Seguridad de tipo asegura que datos inválidos se rechacen temprano</li>
/// <li>Anotaciones de datos documentan campos requeridos y restricciones</li>
/// </ul>
/// </remarks>
public record struct CreateTaskRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }

    public CreateTaskRequest(string title, string description, TaskPriority priority, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
    }
}
