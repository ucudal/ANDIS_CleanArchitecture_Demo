namespace TaskManagement.Application.Queries.SearchTasks;

/// <summary>
/// <c>TaskSearchRequest</c> es el DTO de entrada para operaciones avanzadas de búsqueda de tareas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - DTO de entrada: Transfiere datos de filtro de búsqueda a manejadores de consulta
/// - Patrón de especificación: Encapsula lógica de filtrado y clasificación
/// - Soporta consultas complejas con múltiples opciones de filtro
///
/// Capacidades de Filtrado:
/// - Título: Filtrar tareas por coincidencia parcial de título
/// - Estado: Filtrar tareas por estado actual
/// - Prioridad: Filtrar tareas por nivel de prioridad
/// - AsignadoA: Filtrar tareas asignadas a usuario específicos
///
/// Soporte de Paginación:
/// - Página: Número de página actual (basado en 1)
/// - TamanyoPagina: Número de elementos por página (por defecto 20)
/// - Habilita manejo eficiente de conjuntos de resultados grandes
/// - Previene carga de toda base de datos a la vez
///
/// Soporte de Clasificación:
/// - OrdenarPor: Nombre de columna por el que ordenar (por defecto "CreatedAt")
/// - DirecciónOrdenamiento: "ASC" o "DESC" (por defecto "DESC")
/// - Permite ordenamiento flexible de resultados
/// - El cliente controla el orden de clasificación
///
/// Semántica de Filtrado:
/// - Valores nulos: Filtros opcionales, solo aplicados si se proporcionan
/// - Valores por defecto: Pagina=1, TamanyoPagina=20, OrdenarPor="CreatedAt", DireccionOrdenamiento="DESC"
/// - Resultados vacíos: Respuesta válida si ninguna tarea coincide con filtros
///
/// Patrón de Diseño - Patrón de Especificación:
/// - Encapsula criterios de consulta en un solo tipo
/// - Seguro de tipo: Sin análisis o manipulación de cadenas
/// - Reutilizable: Puede ser utilizado por múltiples manejadores de consulta
/// - Testeable: Puede probar lógica de búsqueda en aislamiento
///
/// Flujo de Uso:
/// 1. Cliente envía HTTP GET con parámetros de búsqueda
/// 2. Controlador deserializa a TaskSearchRequest
/// 3. Manejador de consulta utiliza criterios para construir consulta de base de datos
/// 4. Repositorio ejecuta consulta filtrada
/// 5. Devuelve PagedResult&lt;TaskDto&gt; con resultados filtrados
/// </summary>
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
