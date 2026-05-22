// TaskManagement.Domain/ValueObjects/Email.cs
using System.Text.RegularExpressions;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.ValueObjects;

/// <summary>
/// <c>Email</c> es un Objeto de Valor del Dominio que encapsula la lógica y validación de direcciones de correo electrónico.
///
/// Rol en Clean Architecture:
/// <list type="bullet">
/// <item>Parte del core de la aplicación en la capa de dominio</item>
/// <item>Representa un concepto de dominio inmutable con validación integrada</item>
/// <item>Encapsula reglas de negocio específicas de correo y lógica de validación</item>
/// <item>Se utiliza para manejo de correo seguro de tipo en todo el dominio</item>
/// <item>Previene que direcciones de correo inválidas entren en el sistema</item>
/// </list>
///
/// Características del Objeto de Valor:
/// <list type="bullet">
/// <item>Inmutable: El valor del correo no puede cambiar después de su creación</item>
/// <item>Identidad por Valor: Dos instancias de <see cref="Email"/> son iguales si sus direcciones coinciden</item>
/// <item>Auto-validado: Asegura que solo se creen direcciones de correo válidas</item>
/// <item>Sin preocupaciones directas de persistencia: La infraestructura maneja el almacenamiento</item>
/// </list>
/// </summary>

public sealed class Email : ValueObject
{
    public string Value
    {
        get;
    }
    private Email(string value) => Value = value;
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email is required.");
        // RFC 5322 compliant validation
        var pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        if (!Regex.IsMatch(email, pattern))
            return Result.Failure<Email>("Invalid email format.");
        return Result.Success(new Email(email.ToUpperInvariant()));
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    public override string ToString() => Value;
}
