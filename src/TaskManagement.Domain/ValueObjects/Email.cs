// TaskManagement.Domain/ValueObjects/Email.cs
using System.Text.RegularExpressions;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.ValueObjects;

/// <summary>
/// <c>Email</c> es un <a
/// href="https://github.com/ucudal/ANDIS_Conceptos/blob/2e0fbbe729ea47ee6029405a8435c918c9e7c4e6/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Value_Object.md">objeto
/// de valor</a> del dominio que encapsula la lógica y validación de direcciones
/// de correo electrónico.
/// </summary>
/// <remarks>
/// Rol en Clean Architecture:
/// <ul>
/// <li>Parte del core de la aplicación en la capa del dominio</li>
/// <li>Representa un concepto del dominio inmutable con validación
/// integrada</li>
/// <li>Encapsula reglas de negocio específicas de correo y lógica de
/// validación</li>
/// <li>Se utiliza para manejo de correo seguro de tipo en todo el dominio</li>
/// <li>Previene que direcciones de correo inválidas entren en el sistema</li>
/// </ul>
///
/// Características del objeto valor:
/// <ul>
/// <li>Inmutable: El valor del correo no puede cambiar después de su
/// creación</li>
/// <li>Identidad por valor: Dos instancias de <c>Email</c> son iguales si sus
/// direcciones coinciden</li>
/// <li>Auto-validado: Asegura que solo se creen direcciones de correo
/// válidas</li>
/// <li>Sin preocupaciones directas de persistencia: La infraestructura maneja
/// el almacenamiento</li>
/// </ul>
/// </remarks>

public sealed class Email : ValueObject
{
    public string Value {
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
