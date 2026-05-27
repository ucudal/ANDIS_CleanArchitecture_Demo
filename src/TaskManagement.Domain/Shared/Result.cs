// TaskManagement.Domain/Shared/Result.cs
namespace TaskManagement.Domain.Common;

/// <summary>
/// <c>Result</c> es un tipo de unión discriminada que implementa el patrón <c>Result</c> para manejo de errores.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación (Capa de Dominio - Shared/Common)</li>
/// <li>Reemplaza excepciones con retornos explícitos de éxito/fracaso</li>
/// <li>Habilita manejo de errores funcional en toda la aplicación</li>
/// <li>Previene que errores ocultos se lancen y capturen inesperadamente</li>
/// <li>Proporciona una forma consistente de manejar operaciones que pueden fallar</li>
/// </ul>
///
/// Beneficios del Patrón:
/// <ul>
/// <li>Manejo de errores explícito: Los llamadores deben verificar éxito o fracaso</li>
/// <li>Seguridad de tipo: Generic <see cref="Result{T}"/> preserva tanto el valor de éxito como información de error</li>
/// <li>Funcional: Soporta composición y encadenamiento de operaciones</li>
/// <li>Evita verificaciones de nulo: Siempre devuelve un objeto Result válido</li>
/// <li>Rendimiento: Evita la sobrecarga de desenrollado de pila de excepciones</li>
/// </ul>
///
/// Uso:
/// <ul>
/// <li>Utilizar <see cref="Result"/> para operaciones que no deben lanzar excepciones</li>
/// <li>Utilizar <see cref="Result{T}"/> cuando la operación devuelve un valor en caso de éxito</li>
/// <li>Coincidencia de patrones con <see cref="Result.IsSuccess"/> y <see cref="Result.Errors"/> para manejar ambos casos</li>
/// <li>Utilizar método <see cref="Result{T}.Match"/> para manejo de errores de estilo funcional</li>
/// </ul>
/// </remarks>
public class Result
{
    public bool IsSuccess
    {
        get;
    }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors
    {
        get;
    }
    protected Result(bool isSuccess, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    }
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, new[] { error });
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result<T> Success<T>(T value) => new(value);
    public static Result<T> Failure<T>(string error) => new(new[] { error });
    public static Result<T> Failure<T>(IEnumerable<string> errors) => new(errors);
}

/// <summary>
/// <c>Result{T}</c> es un tipo de unión discriminada genérico para operaciones que devuelven un valor en caso de éxito.
/// </summary>
/// <remarks>
/// Hereda de <see cref="Result"/> y agrega una propiedad <see cref="Result{T}.Value"/> para el caso de éxito.
/// </remarks>
public class Result<T> : Result
{
    public T? Value
    {
        get;
    }
    internal Result(T value) : base(true) => Value = value;
    internal Result(IEnumerable<string> errors) : base(false, errors) => Value = default;
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<IReadOnlyList<string>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        return IsSuccess ? onSuccess(Value!) : onFailure(Errors);
    }
}
