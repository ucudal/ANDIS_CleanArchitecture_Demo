using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TaskManagment.Tests;

/// <summary>
/// Tests de arquitectura para asegurar la aplicación de los principios de Clean
/// Architecture usando NUnit y ArchUnitNET. Basado en <a
/// href="https://mareks-082.medium.com/architectural-tests-in-net-1bd5d19b0ba8">Architectural
/// Tests in .NET</a>.
/// </summary>
[TestFixture]
public class ArchitectureTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(TaskManagement.Domain.Entities.TaskItem).Assembly,
            typeof(TaskManagement.Application.Commands.CreateTaskInteractor).Assembly,
            typeof(TaskManagement.Infrastructure.Persistence.Repositories.TaskRepository).Assembly,
            typeof(TaskManagement.API.Controllers.TasksController).Assembly
        )
        .Build();

    // Domain Layer Rules

    /// <summary>
    /// Verifica que la capa del dominio no dependa de ninguna otra capa
    /// -aplicación, infraestructura o API-.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnAnyOtherLayer()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.Application*")
                    .Or()
                    .ResideInNamespaceMatching("TaskManagement.Infrastructure*")
                    .Or()
                    .ResideInNamespaceMatching("TaskManagement.API*")
            );

        rule.Check(Architecture);
    }

    // Application Layer Rules

    /// <summary>
    /// Verifica que la capa de aplicación no dependa de la capa API ni de la
    /// capa de infraestructura.
    /// </summary>
    [Test]
    public void ApplicationLayer_ShouldNotDependOnApiOrInfrastructureLayer()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Application*")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.API*")
                    .Or()
                    .ResideInNamespaceMatching("TaskManagement.Infrastructure*")
            );

        rule.Check(Architecture);
    }

    // Infrastructure Layer Rules

    /// <summary>
    /// Verifica que la capa de infraestructura no dependa de la capa API.
    /// </summary>
    [Test]
    public void InfrastructureLayer_ShouldNotDependOnApiLayer()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Infrastructure*")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.API*")
            );

        rule.Check(Architecture);
    }

    // API Layer Rules

    /// <summary>
    /// Verifica que ninguna capa interna -dominio, aplicación e
    /// infraestructura- dependa de la capa API.
    /// </summary>
    [Test]
    public void ApiLayer_ShouldNotHaveDependenciesFromOtherLayers()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Or()
            .ResideInNamespaceMatching("TaskManagement.Application*")
            .Or()
            .ResideInNamespaceMatching("TaskManagement.Infrastructure*")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.API*")
            );

        rule.Check(Architecture);
    }

    // Interface Adapter Rules

    /// <summary>
    /// Verifica que la capa de aplicación no dependa de frameworks de mensajería, validación, web
    /// ni acceso a datos: los casos de uso solo conocen sus puertos.
    /// </summary>
    [Test]
    public void ApplicationLayer_ShouldNotDependOnFrameworks()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Application*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("MediatR*")
                    .Or()
                    .ResideInNamespaceMatching("FluentValidation*")
                    .Or()
                    .ResideInNamespaceMatching("Microsoft.AspNetCore*")
                    .Or()
                    .ResideInNamespaceMatching("Dapper*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que controllers, presenters, view models y requests no dependan de la capa de
    /// infraestructura. Solo Program.cs, la raíz de composición, conoce esa capa.
    /// </summary>
    [Test]
    public void InterfaceAdapters_ShouldNotDependOnInfrastructure()
    {
        var rule = Types()
            .That()
            .ResideInNamespace("TaskManagement.API.Controllers")
            .Or()
            .ResideInNamespace("TaskManagement.API.Presenters")
            .Or()
            .ResideInNamespace("TaskManagement.API.ViewModels")
            .Or()
            .ResideInNamespace("TaskManagement.API.Requests")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.Infrastructure*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que las entidades del dominio no lleguen a la interfaz de usuario: controllers,
    /// presenters, view models y requests solo conocen los DTO de la capa de aplicación.
    /// </summary>
    [Test]
    public void InterfaceAdapters_ShouldNotDependOnDomain()
    {
        var rule = Types()
            .That()
            .ResideInNamespace("TaskManagement.API.Controllers")
            .Or()
            .ResideInNamespace("TaskManagement.API.Presenters")
            .Or()
            .ResideInNamespace("TaskManagement.API.ViewModels")
            .Or()
            .ResideInNamespace("TaskManagement.API.Requests")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching("TaskManagement.Domain*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que los presenters residan en <c>TaskManagement.API.Presenters</c>.
    /// </summary>
    [Test]
    public void Presenters_ShouldBeInApiPresenters()
    {
        var rule = Classes()
            .That()
            .HaveNameEndingWith("Presenter")
            .Should()
            .ResideInNamespace("TaskManagement.API.Presenters");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que los objetos del ORM no salgan de la capa de infraestructura.
    /// </summary>
    [Test]
    public void OrmRecords_ShouldNotLeaveInfrastructure()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Application*")
            .Or()
            .ResideInNamespaceMatching("TaskManagement.API*")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespace("TaskManagement.Infrastructure.Persistence.Records")
            );

        rule.Check(Architecture);
    }

    // Cross-Layer Naming Convention Rules

    /// <summary>
    /// Verifica que todos los controladores residan en el namespace
    /// <c>TaskManagement.API.Controllers</c> y que el namespace de
    /// controladores solo contenga clases que terminen con "Controller".
    /// </summary>
    [Test]
    public void Controllers_ShouldBeInApiControllers()
    {
        // TaskManagement.API.Controllers solo tiene *Controllers
        var rule = Types()
            .That()
            .ResideInNamespace("TaskManagement.API.Controllers")
            .Should()
            .HaveNameEndingWith("Controller");

        rule.Check(Architecture);

        // Los *Controllers deben residir en TaskManagement.API.Controllers
        rule = Types()
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .ResideInNamespace("TaskManagement.API.Controllers");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que el namespace <c>TaskManagement.Application.Commands</c> contenga solo
    /// interactores, sus puertos de entrada y de salida, y sus DTO.
    /// </summary>
    [Test]
    public void Commands_ShouldContainOnlyUseCaseElements()
    {
        var rule = Types()
            .That()
            .ResideInNamespace("TaskManagement.Application.Commands")
            .Should()
            .HaveNameEndingWith("Interactor")
            .OrShould()
            .HaveNameEndingWith("Boundary")
            .OrShould()
            .HaveNameEndingWith("Input")
            .OrShould()
            .HaveNameEndingWith("Output");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que el namespace <c>TaskManagement.Application.Queries</c> contenga solo
    /// interactores, sus puertos de entrada y de salida, y sus DTO.
    /// </summary>
    [Test]
    public void Queries_ShouldContainOnlyUseCaseElements()
    {
        var rule = Types()
            .That()
            .ResideInNamespace("TaskManagement.Application.Queries")
            .Should()
            .HaveNameEndingWith("Interactor")
            .OrShould()
            .HaveNameEndingWith("Boundary")
            .OrShould()
            .HaveNameEndingWith("Input")
            .OrShould()
            .HaveNameEndingWith("Dto");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que los interactores residan en los namespaces de comandos o de consultas.
    /// </summary>
    [Test]
    public void Interactors_ShouldBeInCommandsOrQueries()
    {
        var rule = Classes()
            .That()
            .HaveNameEndingWith("Interactor")
            .Should()
            .ResideInNamespace("TaskManagement.Application.Commands")
            .OrShould()
            .ResideInNamespace("TaskManagement.Application.Queries");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de frameworks de mapeo de
    /// objetos relacionales -ORM- como Entity Framework o Dapper -que son los
    /// que se usan en este proyecto; podrían incluirse otros como NHibernate
    /// por ejemplo-.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnOrmFrameworks()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.EntityFrameworkCore*")
                    .Or()
                    .ResideInNamespaceMatching("Dapper*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de frameworks de mensajería
    /// como MediatR -que es el que se usa en este proyecto; podrían incluirse
    /// otros como MassTransit o NServiceBus-.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnMessagingFrameworks()
    {
        // Verifica que el dominio no dependa de MediatR.
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("MediatR*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de frameworks web como
    /// ASP.NET Core o System.Web.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnWebFrameworks()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.AspNetCore*")
                    .Or()
                    .ResideInNamespaceMatching("System.Web*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de proveedores específicos de
    /// bases de datos como Microsoft.Data (SQL Server) o Sqlite -que son los
    /// que se usan en este proyecto, podrían incluirse otros como MySQL o
    /// PostgreSQL-.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnDataProviders()
    {
        // Verifica que el dominio no dependa de proveedores de base de datos: SqlClient, Sqlite, MySql, PostgreSQL
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.Data*")
                    .Or()
                    .ResideInNamespaceMatching("Sqlite*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de servicios de
    /// Microsoft.Extensions como Logging, Configuration, DependencyInjection o
    /// Caching.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnMicrosoftExtensions()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.Extensions.Logging*")
                    .Or()
                    .ResideInNamespaceMatching("Microsoft.Extensions.Configuration*")
                    .Or()
                    .ResideInNamespaceMatching("Microsoft.Extensions.DependencyInjection*")
                    .Or()
                    .ResideInNamespaceMatching("Microsoft.Extensions.Caching*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa del dominio no dependa de frameworks de
    /// autenticación e identidad como Microsoft.IdentityModel,
    /// System.Security.Principal o IdentityModel.
    /// </summary>
    [Test]
    public void DomainLayer_ShouldNotDependOnIdentityFrameworks()
    {
        // Verifica que el dominio no dependa de frameworks de autenticación/identidad
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Domain*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.IdentityModel*")
                    .Or()
                    .ResideInNamespaceMatching("System.Security.Principal*")
                    .Or()
                    .ResideInNamespaceMatching("IdentityModel*")
            );

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que todas las clases de repositorio -y no las interfaces-
    /// residan en el namespace
    /// <c>TaskManagement.Infrastructure.Persistence.Repositories</c>.
    /// </summary>
    [Test]
    public void RepositoriesShouldResideInPersistenceLayer()
    {
        var rule = Classes()
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .ResideInNamespace("TaskManagement.Infrastructure.Persistence.Repositories");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Verifica que la capa de aplicación no dependa directamente de Entity
    /// Framework Core -donde se define <c>DbContext</c>-. La comunicación con
    /// la base de datos debe realizarse a través de repositorios o
    /// abstracciones, nunca directamente desde la capa de aplicación.
    /// </summary>
    [Test]
    public void ApplicationLayer_ShouldNotDirectlyUseDbContext()
    {
        var rule = Types()
            .That()
            .ResideInNamespaceMatching("TaskManagement.Application*")
            .Should()
            .NotDependOnAny(
                Types(true)
                    .That()
                    .ResideInNamespaceMatching("Microsoft.EntityFrameworkCore*")
            );

        rule.Check(Architecture);
    }
}
