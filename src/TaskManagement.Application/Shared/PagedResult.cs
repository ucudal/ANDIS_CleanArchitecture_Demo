namespace TaskManagement.Application.Common;

/// <summary>
/// <c>PagedResult</c> es un DTO para devolver resultados de consultas paginadas.
///
/// Rol en Clean Architecture:
/// - Parte del core de la aplicación (Capa de Aplicación)
/// - DTO de salida: Transfiere datos paginados desde aplicación a capa de API/UI
/// - Modelo de lectura: Optimizado para consultas que devuelven múltiples elementos
/// - Soporta paginación: Habilita manejo eficiente de conjuntos de resultados grandes
///
/// Beneficios de Paginación:
/// - Reduce uso de memoria: Devuelve subconjunto de resultados en lugar de todos
/// - Mejora rendimiento: Ancho de banda de red para cargas más pequeñas
/// - Mejor UX: Los clientes pueden cargar datos incrementalmente
/// - Escalabilidad: Maneja bases de datos con millones de registros
///
/// Propiedades:
/// - Elementos: Datos reales para página actual
/// - ConteoTotal: Número total de elementos en todas las páginas
/// - Página: Número de página actual (basado en 1)
/// - TamanyoPagina: Número de elementos por página
/// - Calculados: PáginasTotal, TienePaginaSiguiente, TienePaginaAnterior
///
/// Patrón de Uso:
/// - El manejador de consulta consulta repositorio para conteo total y elementos paginados
/// - Devuelve <see cref="PagedResult{T}"/> a controlador
/// - Controlador serializa a JSON con metadatos de paginación
/// - Cliente utiliza metadatos de paginación para solicitar página siguiente
///
/// Diseño:
/// - Inmutable: Datos establecidos en constructor, no pueden cambiar
/// - Genérico: <see cref="PagedResult{T}"/> funciona con cualquier tipo de elemento
/// - Sellado: Previene herencia accidental
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items
    {
        get;
    }
    public int TotalCount
    {
        get;
    }
    public int Page
    {
        get;
    }
    public int PageSize
    {
        get;
    }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
