namespace TaskManagement.Application.Queries.SearchTasks;

/// <summary>
/// <c>TaskSearchRequest</c> es el DTO de entrada para operaciones avanzadas de búsqueda de tareas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Aplicación)</li>
/// <li>DTO de entrada: Transfiere datos de filtro de búsqueda a manejadores de consulta</li>
/// <li>Patrón de especificación: Encapsula lógica de filtrado y clasificación</li>
/// <li>Soporta consultas complejas con múltiples opciones de filtro</li>
/// </ul>
///
/// Capacidades de Filtrado:
/// <ul>
/// <li>Título: Filtrar tareas por coincidencia parcial de título</li>
/// <li>Estado: Filtrar tareas por estado actual</li>
/// <li>Prioridad: Filtrar tareas por nivel de prioridad</li>
/// <li>AsignadoA: Filtrar tareas asignadas a usuario específicos</li>
/// </ul>
///
/// Soporte de Paginación:
/// <ul>
/// <li>Página: Número de página actual (basado en 1)</li>
/// <li>TamanyoPagina: Número de elementos por página (por defecto 20)</li>
/// <li>Habilita manejo eficiente de conjuntos de resultados grandes</li>
/// <li>Previene carga de toda base de datos a la vez</li>
/// </ul>
///
/// Soporte de Clasificación:
/// <ul>
/// <li>OrdenarPor: Nombre de columna por el que ordenar (por defecto "CreatedAt")</li>
/// <li>DirecciónOrdenamiento: "ASC" o "DESC" (por defecto "DESC")</li>
/// <li>Permite ordenamiento flexible de resultados</li>
/// <li>El cliente controla el orden de clasificación</li>
/// </ul>
///
/// Semántica de Filtrado:
/// <ul>
/// <li>Valores nulos: Filtros opcionales, solo aplicados si se proporcionan</li>
/// <li>Valores por defecto: Pagina=1, TamanyoPagina=20, OrdenarPor="CreatedAt", DireccionOrdenamiento="DESC"</li>
/// <li>Resultados vacíos: Respuesta válida si ninguna tarea coincide con filtros</li>
/// </ul>
///
/// Patrón de Diseño - Patrón de Especificación:
/// <ul>
/// <li>Encapsula criterios de consulta en un solo tipo</li>
/// <li>Seguro de tipo: Sin análisis o manipulación de cadenas</li>
/// <li>Reutilizable: Puede ser utilizado por múltiples manejadores de consulta</li>
/// <li>Testeable: Puede probar lógica de búsqueda en aislamiento</li>
/// </ul>
///
/// Flujo de Uso:
/// <ol>
/// <li>Cliente envía HTTP GET con parámetros de búsqueda</li>
/// <li>Controlador deserializa a TaskSearchRequest</li>
/// <li>Manejador de consulta utiliza criterios para construir consulta de base de datos</li>
/// <li>Repositorio ejecuta consulta filtrada</li>
/// <li>Devuelve PagedResult&lt;TaskDto&gt; con resultados filtrados</li>
/// </ol>
/// </remarks>
public sealed record TaskSearchRequest(
    string? Title = null,
    string? Status = null,
    string? Priority = null,
    Guid? AssignedTo = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "CreatedAt",
    string SortDirection = "DESC"
);
