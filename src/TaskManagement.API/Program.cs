// This file is the composition root and startup configuration for the application.
//
// Role in Clean Architecture:
// - Part of the UI Layer (API layer)
// - Composition Root: Wires up all dependencies through Dependency Injection
// - Configuration: Sets up middleware pipeline, authentication, and services
// - Entry point: ASP.NET Core executes this file on application startup
//
// Responsibilities:
// - Register services from all layers into the DI container
// - Configure authentication and authorization
// - Add data access and persistence services
// - Configure middleware pipeline (order matters!)
// - Initialize database if needed
//
// Dependency Injection Pattern:
// - Registers interfaces from Application and Domain layers
// - Implementations from Infrastructure layer
// - Controllers use registered services (constructor injection)
// - MediatR handlers use registered services
//
// Architecture Layer Registration:
// - Domain Layer: IUnitOfWork, ITaskRepository, IDomainEventDispatcher
// - Application Layer: MediatR, FluentValidation, ValidationBehavior
// - Infrastructure Layer: DbContext, Repositories, Event Dispatcher
// - UI Layer: Controllers, Middleware
//
// Composition Root Guidelines:
// - UI layer can reference Infrastructure for DI registration
// - Infrastructure implements interfaces from other layers
// - Domain and Application layers remain independent
// - This file is the only place Infrastructure is directly referenced
//
// Middleware Pipeline:
// - Order is critical: earlier middleware processes requests first
// - ExceptionHandlingMiddleware: Usually first to catch all exceptions
// - Authentication, Authorization: Security-related middleware
// - MapControllers: Routes requests to controllers

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

// Load user-secrets explicitly so dotnet user-jwts tokens work even when
// the process is started without ASPNETCORE_ENVIRONMENT=Development.
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);

const string connectionString = "Data Source=file:taskmanagement?mode=memory&cache=shared";
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:DefaultConnection"] = connectionString
});

builder.Services.AddSingleton<SqliteConnection>(_ =>
{
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
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
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

// Add services to container
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TaskManagement.Application.Commands.CreateTask.CreateTaskCommand).Assembly))
    .AddValidatorsFromAssembly(typeof(TaskManagement.Application.Commands.CreateTask.CreateTaskCommand).Assembly)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
    .AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<ITaskReadRepository, TaskReadRepository>()
    .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure middleware pipeline


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
