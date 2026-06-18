using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Ucu.Andis.ArchitectureMetrics;

namespace TaskManagment.Tests;

[TestFixture]
public class StabilityMetricsTests
{
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(TaskManagement.Domain.Entities.TaskItem).Assembly,
            typeof(TaskManagement.Application.Commands.CreateTaskCommand).Assembly,
            typeof(TaskManagement.Infrastructure.Persistence.Repositories.TaskRepository).Assembly,
            typeof(TaskManagement.API.Controllers.TasksController).Assembly
        )
        .Build();

    /// <summary>
    /// Muestra todas las métricas de estabilidad de los componentes de la arquitectura.
    /// </summary>
    /// <remarks>
    /// Este test es un test de demostración que imprime una tabla formateada con todas las métricas
    /// de cada capa de la arquitectura. No valida umbrales específicos, solo muestra el estado actual.
    ///
    /// La tabla incluye:
    /// - Component: Nombre de la capa
    /// - Ca: Acoplamiento aferente (quién depende de mí)
    /// - Ce: Acoplamiento eferente (de quién dependo)
    /// - Abstractness: Proporción de tipos abstractos (0-1)
    /// - Instability: Qué tan inestable es (0=estable, 1=inestable)
    /// - Distance: Distancia de la secuencia principal (0 o 1 es ideal)
    /// </remarks>
    [Test]
    public void DisplayAllComponentMetrics()
    {
        var components =
            MetricsCalculator.BuildAssemblyComponents(
                Architecture,
                "TaskManagement.Domain",
                "TaskManagement.Application",
                "TaskManagement.Infrastructure",
                "TaskManagement.API");

        var metrics =
            MetricsCalculator.CalculateMetrics(components);

        TestContext.Out.WriteLine("\n=== Métricas de componentes ===");
        TestContext.Out.WriteLine(
            "{0,-40} {1,6} {2,6} {3,11} {4,12} {5,10}",
            "Componente", "FanIn", "FanOut", "Abstracción", "Inestabilidad", "Distancia");

        TestContext.Out.WriteLine(new string('-', 95));

        foreach (var metric in metrics.OrderBy(m => m.Name))
        {
            TestContext.Out.WriteLine(
                "{0,-40} {1,6} {2,6} {3,11:F4} {4,12:F4} {5,10:F4}",
                metric.Name,
                metric.FanIn,
                metric.FanOut,
                metric.Abstractness,
                metric.Instability,
                metric.Distance);
        }
    }

    /// <summary>
    /// Valida que la capa Domain mantenga características de estabilidad esperadas.
    /// </summary>
    /// <remarks>
    /// La capa Domain es el núcleo de la arquitectura y debe ser:
    /// - Estable (Instabilidad &lt; 0.3): Pocos cambios externos la afectan.
    /// - Cercana a la secuencia principal (Distance &lt; 0.8): Bien diseñada y equilibrada.
    ///
    /// Umbrales usados:
    /// - I &lt; 0.3: El Domain debe ser poco dependiente de otras capas.
    /// - D &lt; 0.8: Distancia aceptable considerando que es principalmente concreto (entidades).
    /// </remarks>
    [Test]
    public void Domain_Should_Remain_Stable()
    {
        var components =
            MetricsCalculator.BuildAssemblyComponents(
                Architecture,
                "TaskManagement.Domain",
                "TaskManagement.Application",
                "TaskManagement.Infrastructure",
                "TaskManagement.API");

        var metrics =
            MetricsCalculator.CalculateMetrics(components);

        var domain =
            metrics.Single(
                m => m.Name == "TaskManagement.Domain");

        Assert.That(domain.Instability,
            Is.LessThan(0.3));

        Assert.That(domain.Distance,
            Is.LessThan(0.8));
    }

    /// <summary>
    /// Valida que la capa Application tenga características equilibradas.
    /// </summary>
    /// <remarks>
    /// La capa Application actúa como intermediaria entre Domain e Infrastructure.
    /// Debe ser moderadamente equilibrada:
    /// - Instabilidad moderada (I &lt; 0.6): Depende de Domain y puede ser usada por Infrastructure.
    /// - Cercana a la secuencia principal (D &lt; 0.7): Bien diseñada con abstracción razonable.
    ///
    /// Umbrales usados:
    /// - I &lt; 0.6: Permite algunas dependencias externas pero mantiene independencia relativa.
    /// - D &lt; 0.7: Distancia aceptable para una capa de aplicación equilibrada.
    /// </remarks>
    [Test]
    public void Application_Should_Be_Balanced()
    {
        var components =
            MetricsCalculator.BuildAssemblyComponents(
                Architecture,
                "TaskManagement.Domain",
                "TaskManagement.Application",
                "TaskManagement.Infrastructure",
                "TaskManagement.API");

        var metrics =
            MetricsCalculator.CalculateMetrics(components);

        var application =
            metrics.Single(
                m => m.Name == "TaskManagement.Application");

        // Application layer should have moderate dependencies
        Assert.That(application.Instability,
            Is.LessThan(0.6),
            "Application should be relatively stable (I < 0.6)");

        // Application layer distance should be acceptable
        Assert.That(application.Distance,
            Is.LessThan(0.7),
            "Application should be close to main sequence (D < 0.7)");
    }

    /// <summary>
    /// Valida que la capa Infrastructure cumpla el rol de capa dependiente.
    /// </summary>
    /// <remarks>
    /// La capa Infrastructure es una capa "hoja" que implementa detalles técnicos.
    /// Debe ser altamente dependiente y estar en la secuencia principal:
    /// - Altamente dependiente (I &gt; 0.8): Depende de múltiples componentes (es una capa hoja).
    /// - En la secuencia principal (D &lt; 0.2): Bajo acoplamiento o completamente concreto.
    ///
    /// Umbrales usados:
    /// - I &gt; 0.8: Esperamos que sea un componente consumidor, no productor.
    /// - D &lt; 0.2: Debe estar en una esquina de la secuencia principal (cercana a (0,1)).
    /// </remarks>
    [Test]
    public void Infrastructure_Should_Be_Dependent()
    {
        var components =
            MetricsCalculator.BuildAssemblyComponents(
                Architecture,
                "TaskManagement.Domain",
                "TaskManagement.Application",
                "TaskManagement.Infrastructure",
                "TaskManagement.API");

        var metrics =
            MetricsCalculator.CalculateMetrics(components);

        var infrastructure =
            metrics.Single(
                m => m.Name == "TaskManagement.Infrastructure");

        // Infrastructure should be highly dependent (leaf layer)
        Assert.That(infrastructure.Instability,
            Is.GreaterThan(0.8),
            "Infrastructure should be highly dependent (I > 0.8)");

        // Infrastructure should be on the main sequence
        Assert.That(infrastructure.Distance,
            Is.LessThan(0.2),
            "Infrastructure should be on main sequence (D < 0.2)");
    }

    /// <summary>
    /// Valida que la capa API cumpla el rol de capa de presentación dependiente.
    /// </summary>
    /// <remarks>
    /// La capa API/Presentación es la capa más externa ("hoja") que expone la aplicación.
    /// Debe ser altamente dependiente y estar en la secuencia principal:
    /// - Altamente dependiente (I &gt; 0.8): Depende de Application e Infrastructure.
    /// - En la secuencia principal (D &lt; 0.2): Baja abstracción (es principalmente concreto).
    ///
    /// Umbrales usados:
    /// - I &gt; 0.8: Esperamos que sea un consumidor de servicios, no un proveedor.
    /// - D &lt; 0.2: Debe estar cercana al punto (0,1) de la secuencia principal.
    /// </remarks>
    [Test]
    public void API_Should_Be_Dependent()
    {
        var components =
            MetricsCalculator.BuildAssemblyComponents(
                Architecture,
                "TaskManagement.Domain",
                "TaskManagement.Application",
                "TaskManagement.Infrastructure",
                "TaskManagement.API");

        var metrics =
            MetricsCalculator.CalculateMetrics(components);

        var api =
            metrics.Single(
                m => m.Name == "TaskManagement.API");

        // API/Presentation should be highly dependent (leaf layer)
        Assert.That(api.Instability,
            Is.GreaterThan(0.8),
            "API should be highly dependent (I > 0.8)");

        // API should be on the main sequence
        Assert.That(api.Distance,
            Is.LessThan(0.2),
            "API should be on main sequence (D < 0.2)");
    }
}
