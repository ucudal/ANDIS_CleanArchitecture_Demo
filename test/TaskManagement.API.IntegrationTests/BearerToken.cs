namespace TaskManagement.API.IntegrationTests;

/// <summary>
/// <c>BearerToken</c> obtiene el token JWT usado por las pruebas de
/// integración, sin necesidad de incluirlo en el código.
/// </summary>
/// <remarks>
/// El token se busca en este orden:
/// <ol>
/// <li>La variable de entorno <c>BEARER_TOKEN</c> -útil en CI-</li>
/// <li>La entrada <c>BEARER_TOKEN</c> del archivo .env en la raíz del
/// proyecto -el mismo que usa TaskManagement.http-</li>
/// </ol>
/// </remarks>
internal static class BearerToken
{
    private const string VariableName = "BEARER_TOKEN";

    private static readonly Lazy<string?> Token = new(Load);

    public static string Value => Token.Value
        ?? throw new InvalidOperationException(
            $"No se encontró el token. Definir la variable de entorno {VariableName} o crear el archivo .env " +
            "en la raíz del proyecto a partir de .env.example.");

    private static string? Load()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(VariableName);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment.Trim();

        var envFile = FindEnvFile();
        if (envFile is null)
            return null;

        return File.ReadLines(envFile)
            .Select(line => line.Trim())
            .Where(line => line.StartsWith(VariableName + "=", StringComparison.Ordinal))
            .Select(line => line[(VariableName.Length + 1)..].Trim().Trim('"'))
            .FirstOrDefault(value => value.Length > 0);
    }

    // Sube desde el directorio de salida de las pruebas hasta encontrar un archivo .env.
    private static string? FindEnvFile()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, ".env");
            if (File.Exists(candidate))
                return candidate;
        }
        return null;
    }
}
