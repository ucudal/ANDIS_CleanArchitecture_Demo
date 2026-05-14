// TaskManagement.API/Program.cs
using TaskManagement.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.EventDispatching;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Persistence.Repositories;
using MediatR;
using FluentValidation;
using TaskManagement.Application.Behaviors;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Data Source=file:taskmanagement?mode=memory&cache=shared";
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:DefaultConnection"] = connectionString
});

var keepAliveConnection = new SqliteConnection(connectionString);
keepAliveConnection.Open();

builder.Services.AddSingleton(keepAliveConnection);

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
app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();