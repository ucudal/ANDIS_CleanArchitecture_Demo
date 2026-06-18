// Este archivo es la raíz de composición y configuración de inicio de la aplicación.
//
// Rol en Clean Architecture:
// - Parte de la capa de presentación, en este caso, API REST
// - Raíz de Composición: Conecta todas las dependencias a través de Inyección de Dependencias
// - Configuración: Configura pipeline de middleware, autenticación y servicios
// - Punto de entrada: ASP.NET Core ejecuta este archivo en el inicio de la aplicación
//
// Responsabilidades:
// - Registrar servicios de todas las capas en el contenedor de DI
// - Configurar autenticación y autorización
// - Agregar servicios de acceso a datos y persistencia
// - Configurar pipeline de middleware (¡el orden importa!)
// - Inicializar base de datos si es necesario
//
// Patrón de Inyección de Dependencias:
// - Registra interfaces de capas de Aplicación y Dominio
// - Implementaciones de capa de Infraestructura
// - Los controladores usan servicios registrados (inyección de constructor)
// - Los manejadores de MediatR usan servicios registrados
//
// Registro de Capa de Arquitectura:
// - Capa del dominio: IUnitOfWork, ITaskRepository, IDomainEventDispatcher
// - Capa de aplicación: MediatR, FluentValidation, ValidationBehavior
// - Capa de infraestructura: DbContext, Repositorios, Distribuidor de Eventos
// - Capa de UI: Controladores, Middleware
//
// Directrices de Raíz de Composición:
// - La capa de UI puede hacer referencia a Infraestructura para registro de DI
// - Infraestructura implementa interfaces de otras capas
// - Las capas del dominio y Aplicación permanecen independientes
// - Este archivo es el único lugar donde se hace referencia directa a Infraestructura
//
// pipeline de Middleware:
// - El orden es crítico: el middleware anterior procesa solicitudes primero
// - ExceptionHandlingMiddleware: Usualmente primero para capturar todas las excepciones
// - Autenticación, Autorización: Middleware relacionado con seguridad
// - MapControllers: Enruta solicitudes a controladores

using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Middleware;
using TaskManagement.Application.Behaviors;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.EventDispatching;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Cargar secretos de usuario explícitamente para que los tokens dotnet user-jwts funcionen incluso cuando
// el proceso se inicia sin ASPNETCORE_ENVIRONMENT=Development.
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);

const string connectionString = "Data Source=file:taskmanagement?mode=memory&cache=shared";
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?> {
    ["ConnectionStrings:DefaultConnection"] = connectionString
});

builder.Services.AddSingleton<SqliteConnection>(_ => {
    var connection = new SqliteConnection(connectionString);
    connection.Open();
    return connection;
});

var bearerSection = builder.Configuration.GetSection("Authentication:Schemes:Bearer");
var validIssuer = bearerSection["ValidIssuer"];
var validAudiences = bearerSection
    .GetSection("ValidAudiences")
    .Get<string[]>() ?? [];
var signingKeys = bearerSection
    .GetSection("SigningKeys")
    .GetChildren()
    .Select(x => x["Value"])
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => new SymmetricSecurityKey(Convert.FromBase64String(x!)))
    .Cast<SecurityKey>()
    .ToArray();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = !string.IsNullOrWhiteSpace(validIssuer),
            ValidIssuer = validIssuer,
            ValidateAudience = validAudiences.Length > 0,
            ValidAudiences = validAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidateLifetime = true,
            TryAllIssuerSigningKeys = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();

// Agregar servicios al contenedor
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TaskManagement.Application.Commands.CreateTaskCommand).Assembly))
    .AddValidatorsFromAssembly(typeof(TaskManagement.Application.Commands.CreateTaskCommand).Assembly)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
    .AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<ITaskReadRepository, TaskReadRepository>()
    .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>()
    .AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configurar pipeline de middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Partial Program class made public for testing purposes.
/// Allows WebApplicationFactory to access the Program configuration in integration tests.
/// </summary>
public partial class Program
{
    // Intencionalmente en blanco.
}
