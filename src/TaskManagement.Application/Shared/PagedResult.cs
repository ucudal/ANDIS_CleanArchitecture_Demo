namespace TaskManagement.Application.Common;

#pragma warning disable CS1570 // XML comment has badly formed XML
/// <summary>
/// <c>PagedResult</c> es un DTO para devolver resultados de consultas paginadas.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte de la capa de aplicación</li>
/// <li>DTO de salida: Transfiere datos paginados desde aplicación a capa de API/UI</li>
/// <li>Modelo de lectura: Optimizado para consultas que devuelven múltiples elementos</li>
/// <li>Soporta paginación: Habilita manejo eficiente de conjuntos de resultados grandes</li>
/// </ul>
///
/// Beneficios de Paginación:
/// <ul>
/// <li>Reduce uso de memoria: Devuelve subconjunto de resultados en lugar de todos</li>
/// <li>Mejora rendimiento: Ancho de banda de red para cargas más pequeñas</li>
/// <li>Mejor UX: Los clientes pueden cargar datos incrementalmente</li>
/// <li>Escalabilidad: Maneja bases de datos con millones de registros</li>
/// </ul>
///
/// Propiedades:
/// <ul>
/// <li>Elementos: Datos reales para página actual</li>
/// <li>ConteoTotal: Número total de elementos en todas las páginas</li>
/// <li>Página: Número de página actual -basado en 1-</li>
/// <li>TamanyoPagina: Número de elementos por página</li>
/// <li>Calculados: PáginasTotal, TienePaginaSiguiente, TienePaginaAnterior</li>
/// </ul>
///
/// Patrón de Uso:
/// <ul>
/// <li>El manejador de consulta consulta repositorio para conteo total y elementos paginados</li>
/// <li>Devuelve PagedResult< T > a controlador</li>
/// <li>Controlador serializa a JSON con metadatos de paginación</li>
/// <li>Cliente utiliza metadatos de paginación para solicitar página siguiente</li>
/// </ul>
///
/// Diseño:
/// <ul>
/// <li>Inmutable: Datos establecidos en constructor, no pueden cambiar</li>
/// <li>Genérico: PagedResult< T > funciona con cualquier tipo de elemento</li>
/// <li>Sellado: Previene herencia accidental</li>
/// </ul>
/// </remarks>
#pragma warning restore CS1570 // XML comment has badly formed XML
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
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
