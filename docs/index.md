# Introducción

A continuación describimos la arquitectura de esta demo de [Clean
Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
utilizando [modelos
C4](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_02_.Arquitectura/2_02_01_Modelo_C4.md).

Recuerda que en Clean Architecture el código se organiza en círculos
concéntricos, donde cada círculo representa diferentes áreas del software. Los
elementos de código de un círculo no pueden tener referencias a elementos en un
círculo exterior, o dicho de otra forma, las dependencias pueden ir sólo de
afuera hacia adentro.

![Clean
Architecture](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

## Diagrama C4 de sistema de software

El primer diagrama C4 es el del [sistema de
software](https://c4model.com/abstractions/software-system). Muestra los
[sistemas
adyacentes](https://github.com/ucudal/ANDIS_Conceptos/blob/main/4_Conceptos/4_Sistema_adyacente.md)
que en este caso son el `Usuario` que gestiona las tareas y un `Email Service`
que envía correos electrónicos para notificar cambios en las tareas.

\note En esta demo no se utiliza un servidor de correo externo pero está
    diseñada de forma tal que podría agregarse uno.

![Task Management System
Software](https://www.plantuml.com/plantuml/png/NP11QiD034NtFeNgbGk5RB8i8M0CtGHCxAo3cAX1TAn9PkIGzavTvGYvM6KxbDQxB_Jr_fzP9yMeVUUp9nRd-mF1QxbSbCjz6KJfggkiffY257sY-0BvBlKOEUGcfmN2kISuKb8U1iCHflmvQpwJKbSefez9saLh0snGy4WTYSAq65WD3mhXX14dsFqcO6rcl7rrrSACx7-XrJ11G_72ZaRMA3c0rQSeOO9AK7vdXxOGId0A4Lo8aSpcoMcqF7R1WhRa7u_zrtsdxFaBXu2tjgqNswff7-w_lbatkTo_vzbZnqrRs_MQv62V_mC0)

## Diagrama C4 de contenedores

El segundo diagrama C4 es el de
[contenedores](https://c4model.com/diagrams/container). Muestra la aplicación
[ASP .NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) que expone una
API REST `TaskManagement.API` y las librerías `TaskManagement.Domain`,
`TaskManagement.Application` y `TaskManagement.Infrastructure` que implementan
las capas del dominio, aplicación e infraestructura, respectivamente, de acuerdo
a los círculos de Clean Architecture mostrados más arriba. También se muestra la
base de datos utilizada por la capa de infraestructura para almacenamiento de
datos.

Traduciendo la terminología C4 a .NET, los contenedores son los
[ensamblados](https://learn.microsoft.com/en-us/dotnet/standard/assembly/) que
contienen el código de las
[librería](https://learn.microsoft.com/en-us/dotnet/standard/class-libraries)
-cada ensamblado se genera a partir de un proyecto `.csproj` con el mismo
nombre; las propiedades de la librería se especifican en ese proyecto-.

![Task Management
Containers](https://www.plantuml.com/plantuml/png/bLJ1RjD04BtxAuPmIGLgBZqXX9hQXWQ5D91KFKRZzJHQkjuri-jI2_59FVKJycFCEiOKKY7mQJtxPjwRDsEV-e0ScieapxHLfYa9NgSda_HaapeRK5lYDycIs3ixRZpnInXT-WPPkv4SKoQ45hRaWtOMGMeH-j5Hicfze6fpvXfR8hTZttg1hz7Vm0UqkAMAR80zQdG4tnAGPs0SOwrx-2_qyNGvYvpJrV9uaQy5pGHNL40Vp-zgPmd8bhNJDdWuNw-NK2CZ6DnBt3rOa0O7bN-Ira-GrhNH2cDxaMIk2oRUtMEianhqv25z7czcLMsw50I5tla8QBnx1SfLQ4i9j7AqlZ7X2UvIGDeUqcTT_E_C6Lthhl6WIombB3HWTWzRqHt1bhPEQIVyhlXCGPJSed5ye47RQyP32RCOze4R5HgcVpQXbftHCbwhD0A1J8IsZAB32fhk0h4zz24hncKXTSwoM7gzdrDy5uuOoJ3ANtsSwz0fcPeA5GsB9osM7OB9T5GelWl7z-GJcNvzzwjrwJ7cnnMPl5zTB0GK8TtbVWzczjQftsVv6FJJB2a4M8WzgFhUzE11AOhmIxEx7nAby33fc4a6AbUISP51RhJLxZ6uP_a_cZwfCt9EDCczObrqVkMo8w65jumLVNLyyo9ZuQ6u2wlpYwlDF7-NBxBnKN8gdr7yAVm0)

Puedes ver claramente en el diagrama que las dependencias van "de arriba hacia
abajo", lo que equivale a "afuera hacia adentro" cuando las capas se muestran en
círculos.

## Diagrama C4 de componentes

El siguiente diagrama C4 es el de
[componentes](https://c4model.com/diagrams/component). Muestra las
funcionalidades de la aplicación encapsuladas detrás de interfaces -tipos- bien
definidos.

Traduciendo la terminología de C4 a .NET, los componentes son las clases -las
clases definen tipos-.

![Task Management
Components](https://www.plantuml.com/plantuml/png/XLRDRXiv3Bu7o3kmFhM0sxtqk5WqiTskWNZZs6bx3EWPUg9MbgQIngsns4VQKnyXBrPIy--Pe04IQ2Zo8_Nf8vLtFg0BnK73xEt3p1voQyY4Xxd2pE7XykA5CeakKe8_Ps__wBgyoJdehykBcJK1bI7tS6qBaw8xlSHSJM5oZ_xh2WrcT13tg_LoyWh-lRm0_cbGNYOSxgpMvAeGc3MM2Pkkjkit_opksUheR6lnGp20J9fRPOA7pM9x3pawX8nyK4y_3N3XV3v2FtbrdlcWqbJJTzwDM0dbGLa3VwD9jJ8Ph9hzGHtzhHdceU2a9pYWeIy89609Wv67JDiTQehv_tk6hNo4hJpNAa6fQ8or0wKAWr2sYgjDhEDkiuNosqQ2RjstWeb02UJIKiybtXNa58tGaLG-0bGj9-swf8LSQMD20qd5rUuqucZg6OSflfMv6Q6kebgUUl0VA0ZCzMcPniqfR0bTyYX67or3w2DgbSgv1VszBjZsgJG9Wspxr3YqNqaj4KUHg15MTlI8HrN2hbLEh0o2wzfOac6zVvqK8OXe-7uJSkOyoHHkR9QnmnGMpbdNgEnvMQJsm9QXCkRHFYOA_bMXv7THBQMCCW1MDYKT3R7BeiS9kFr0F_tCM6j2XA7Cid17-IteTlU5ak3BenK4rVTeaWtbZZo7ef4aqbtMSmhzV0uwyb_VWYtYT-nqMS6ws6db7uMrwXIb4DEuFOLAUr67fMsOW-S4kJqcaIMkqNh5cZZ1ih4Bzjgw07FQAwD4Fz6s5FWsOWhBZxnxk_zitTVdRr6PlSFX9Ix5x8Ch4fuSD7QPhiwa-gNMbuhSrbS9rriM9wSMv5RJP_KEgIYQyXb0pW9s3_4qKgz3up694x6jk-nEBrQ4KBfKS-u4ERUlye5Ced2OBTq9E_GZIF7rEWwfpKgwPgyodg5rjrRgVCzF47tdaignsyQDQsUkV8uXUOnlILT4qEwqSgf6JhnlKHPtG4I9CWLkFxvzUtwpynsVX2EsTpSgS4Ly3JlqyHaeJV6jq0TM4bCa0yVttw0DwTxBqtl_FdgiVRhsTjelpF7fV-GW6R1MM6S_N-ZG0Q-TEmDzQOusmO57Eu_x3fsXrvkh2sxtWdLtR0TRMswTh3E5U1gelMfnctmTdnkYUhkrDfEWkfXXbbuVj-gl7VlTqzLUX-A-qsyGkzrorfipQrsgJECwq69x76SPlt7ir1L5jQuwYFaHbtVydmt_ZtLvyGi0)

Puedes ver claramente como las clases en los contenedores mantienen las
referencias del diagrama de contenedores, es decir, "de arriba hacia abajo".

## Diagrama C4 de código

El último diagrama C4 es el de [código](https://c4model.com/diagrams/code).
Muestra cómo los componentes son implementados en código.

En este caso usamos un diagrama de clases -parcial- mostrando algunas de las
clases involucradas en la creación de una tarea.

![Task Management
Components](https://www.plantuml.com/plantuml/png/rLPjJzim4FxkNs7roxgDyWDGL2WjOf7GYg28ZvFd760rZJFRgLP1__lEBscSDC0gJKYJcZQzEzyzpxtOFjE6ALEjSa9AnfVi4UXW9bTC5FIARK0DADCq29u9fJndMjF1WkbbQc0re4a2XH5cCnuxFQKZccRqX5vM8Wl_5yBaW29jb2WUWt06cYjH6Y6BhkfQ2QdG9Sf5OMhx2cRCM3VsPo5MS1eqKmKeoIOcqhLLvviOq5VwZAIDuDkW7neBi0PnHBC6MYCi0oY72Mqyd0ZD4ULrD1_iCmQjynG2-qHUh35b6hNf3R1iNkIRgSnpu3PW4fcCmwf9baNI8TZlazYNZRIMEthJ79YAGmqZWorfgXZPk4AweBwEiPab4JBPQ-jgR8d-JTrxIhB51p_TC7JqCevP4iJKDEcaB7F1cUkVfbDZQTIktgqB85ShLcIM0lUlCOaFQTGszJT0ViCdjyIIvCc_HbX_63IyWL9gOQIoBjD5IrBRLOKmyuTxgPRMvcxthzQxIiw4BfdXJw2iKPJqHX6QouCQAlXLWSOst4EDSy0A3ddkgh2GIy0fcdP5zN0aDX7ZfWamZw0U6CSAT_ZODAIrTfS8DnqJlIduq2QacRZ3O0HVbs3EDsdm9p9VfWC9rG1EFIlI8TMRszUurHOUaAkKVd_RE9ZwjWkowPBNaDsoDKonAnz1Ut07vKZupMtR7kcqU50C1wpYfb8GJNL7qJc7sYqYzi1YZsS_fnBblzqHKR__7_tZfY9YkuF_AbdfzYeTKvrxqR-jNUyvqkbfqODFWreB3jTAhaNcDvpRjkYZkn-vn0V7Q5IVEwJJULOJMHBoYTvSN4qMwVpxBTdR2UdnyNZNJnZFtW_8lbdOvYdwxcnvkARIxEqLT_7s_gvrRwNUNOWhzycTu9L8Si25f0ixUHdv6q5q6hLw5rs-jB2JVc3UhFyu8hlCVi5eaGrjtOqq8MTGPFQ6-mS0)

<!-- markdownlint-disable-next-line MD025 -->
# Implementación de la arquitectura

La arquitectura en este demo está organizada en proyectos de C#
independientes, donde cada proyecto corresponde a un círculo:

\note En la [documentación de Microsoft sobre Clean
    Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
    el círculo central es `Application Core` e incluye lo que en la
    documentación original de [Clean
    Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
    son los círculos `Entities` y `Use Cases`. A pesar de ser una demo en .NET,
    usamos la terminología original.

1. [TaskManagement.Domain](./namespaceTaskManagement_1_1Domain.html) es una
  librería de C# en la que se define el
  [dominio](https://github.com/ucudal/ANDIS_Conceptos/blob/main/4_Conceptos/4_Dominio.md)
  de la aplicación:
  [entidades](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Entity.md)
  y [objetos
  valor](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Value_Object.md),
  eventos y excepciones. El domino utiliza
  [eventos](./classTaskManagement_1_1Domain_1_1Events_1_1DomainEvent.html)
  para informar cuando se crea, se completa, o se asigna una tarea, o cuando
  cambia su prioridad -ver por ejemplo la propiedad `TaskItem.DomainEvents` y el
  método `TaskItem.Create` en
  [TaskItem](./classTaskManagement_1_1Domain_1_1Entities_1_1TaskItem.html)-;
  por esto, esta aplicación también utiliza una [arquitectura dirigida por
  eventos](https://github.com/ucudal/ANDIS_Conceptos/blob/main/3_Plantillas/3_13_Event_Driven_Architecture.md).
  La capa del dominio tiene la responsabilidad de generar eventos, pero es la
  capa de aplicación la que tienen la responsabilidad de procesarlos -ver por
  ejemplo `CreateTaskCommand.Handle` en
  [CreateTaskCommand](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTask_1_1CreateTaskCommandHandler.html)-.
  El proyecto
  [`TaskManagement.Domain`](./namespaceTaskManagement_1_1Domain.html) no
  referencia ningún otro proyecto, es el centro de los círculos concéntricos.

2. [TaskManagement.Application](./namespaceTaskManagement_1_1Application.html)
  es otra librería de C# en la que se definen las funcionalidades de la
  aplicación -comandos y consultas-, o dicho de otra forma, donde se implementa
  la lógica de los casos de uso. Esta aplicación usa el patrón
  [CQRS](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_09_.Patrones_de_arquitectura/2_09_CQRS.md)
  donde los comandos están separados de las consultas -ver por ejemplo la clase
  [CreateTaskCommand](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTask_1_1CreateTaskCommandHandler.html)
  y la clase
  [GetTaskByIdQuery](./classTaskManagement_1_1Application_1_1Queries_1_1GetTaskById_1_1GetTaskByIdQueryHandler.html)-.
  La capa de aplicación define -y utiliza- abstracciones que emplean el patrón
  de [inyección de
  dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview)
  -ver por ejemplo las interfaces
  [IEmailService](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1IEmailService.html),
  [ITaskRepository](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1ITaskRepository.html),
  [ITaskReadRepository](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1ITaskReadRepository.html)
  o
  [IUnitOfWork](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1IUnitOfWork.html)-[^3].
  Las clases concretas que implementan estas abstracciones están definidas en la
  capa de infraestructura -ver por ejemplo las clases
  [TaskRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskRepository.html)
  y
  [TaskReadRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskReadRepository.html),
  respectivamente- y también son creadas en tiempo de ejecución en la capa de
  interfaz API -ver las llamadas a `builder.services…
  .AddScoped<IUnitOfWork>(…), .AddScoped<ITaskReadRepository,
  TaskReadRepository>(), y .AddScoped<IDomainEventDispatcher,
  MediatRDomainEventDispatcher>()` en
  [Program](./classProgram.html)-. Los eventos
  creados en la capa del dominio son procesados en la capa de aplicación usando
  una abstracción -ver la interfaz
  [IDomainEventDispatcher](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1IDomainEventDispatcher.html)-;
  y esa abstracción también está implementada en una clase definida en la capa
  de infraestructura -ver la clase
  [MediatRDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1MediatRDomainEventDispatcher.html)-.
  El proyecto
  [`TaskManagement.Application`](./namespaceTaskManagement_1_1Application.html)
  referencia solamente el proyecto
  [`TaskManagement.Domain`](./namespaceTaskManagement_1_1Domain.html), la
  dependencia es de un círculo externo al centro de los círculos concéntricos.

3. [TaskManagement.Infrastructure](./namespaceTaskManagement_1_1Infrastructure.html)
  es otra librería de C# en la que se definen cómo se implementa la
  infraestructura para las abstracciones definidas en la capa de aplicación de
  -repositorios, despacho de eventos y persistencia-. Las abstracciones
  [ITaskRepository](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1ITaskRepository.html)
  y
  [ITaskReadRepository](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1ITaskReadRepository.html)
  de la capa de aplicación se implementan con las clases
  [TaskRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskRepository.html)
  y
  [TaskReadRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskReadRepository.html),
  respectivamente, de esta capa de infraestructura. También en este caso se usa
  injección de dependencias en la capa de interfaz API -ver por ejemplo en
  [Program](./classProgram.html) las llamadas a
  `builder.services...AddScoped<ITaskRepository, TaskRepository>()` y
  `builder.services...AddScoped<ITaskReadRepository, TaskReadRepository>()`-.
  Como ya fue mencionado antes, la abstracción
  [IDomainEventDispatcher](./interfaceTaskManagement_1_1Application_1_1Interfaces_1_1IDomainEventDispatcher.html)
  definida en la capa de aplicación se implementa con la clase
  [MediatRDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1MediatRDomainEventDispatcher.html)
  de esta capa de infraestructura y la instancia se crea en tiempo de ejecución
  también con injección de dependencias en la capa de interfaz API en
  [Program](./classProgram.html) -ver
  `builder.services...AddScoped<IDomainEventDispatcher,
  MediatRDomainEventDispatcher>()`-. El proyecto
  [`TaskManagement.Infrastructure`](./namespaceTaskManagement_1_1Infrastructure.html)
  referencia solamente el proyecto
  [`TaskManagement.Application`](./namespaceTaskManagement_1_1Application.html),
  la dependencia es de un círculo externo a un círculo interno. Esta capa de
  infraestructura tiene también la configuración de los frameworks de acceso a
  datos.

4. [TaskManagement.API](./namespaceTaskManagement_1_1API.html) es una
  aplicación web en .NET en la que se define la interfaz, en esta demo, una API
  REST. El proyecto referencia tanto al proyecto
  [`TaskManagement.Application`](./namespaceTaskManagement_1_1Application.html)
  como al proyecto
  [`TaskManagement.Infrastructure`](./namespaceTaskManagement_1_1Infrastructure.html),
  ambos en círculos internos. Como toda aplicación web en .NET la carpeta
  contiene los controladores que implementan los *endpoint* de la API REST -ver
  por ejemplo
  [TasksController](./classTaskManagement_1_1API_1_1Controllers_1_1TasksController.html)-.
  El archivo [`TaskManagement.http`](../TaskManagement.http) tiene ejemplos para
  invocar la API REST.

[^3]: Algunos autores definen las abstracciones relacionadas con el dominio en
    la capa del dominio, aunque no sean utilizadas en esa capa. Siguiendo esos
    autores, interfaces como `ITaskRepository` se definirían en la capa de
    dominio. En esta demo, interfaces como esa son definidas en la capa de
    aplicación, porque es allí donde se usan.

<!-- markdownlint-disable-next-line MD025 -->
# Frameworks utilizados

La aplicación utiliza los siguientes frameworks.

## FluentValidation

[FluentValidation](https://github.com/FluentValidation/FluentValidation) es una
librería de .NET para constuir reglas de validación fuertemente tipadas.

## MediatR

[MediatR](https://github.com/LuckyPennySoftware/MediatR) es una librería de .NET
para mensajería en proceso -sin persistencia- sin dependencias. Admite
solicitudes y respuestas, comandos, consultas, notificaciones y eventos, tanto
síncronos como asíncronos, con despacho inteligente mediante tipos genéricos en
C#.

## Drapper

[Dapper](https://github.com/DapperLib/Dapper) es una librería de mapeo
objeto-relacional -ORM- de código abierto para aplicaciones .NET. Permite
acceder de forma rápida y sencilla a los datos de las bases de datos sin
necesidad de escribir código complejo.

<!-- markdownlint-disable-next-line MD025 -->
# Resumen de clases e interfaces definidos en cada capa

## 1. Capa del dominio

La capa del dominio está definida
[aquí](./namespaceTaskManagement_1_1Domain.html).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Entidades** | `Entities/TaskItem.cs`, `Entities/TaskStatus.cs`, `Entities/TaskPriority.cs` | Clases de modelo de negocio representando conceptos del dominio |
| **Objetos valor** | `ValueObjects/Email.cs`, `Shared/ValueObject.cs` | Objetos inmutables representando valores del dominio |
| **Eventos del dominio** | `Events/DomainEvent.cs` | Eventos representando ocurrencias importantes del dominio |
| **Interfaces** | Ver nota [^3] | Ver nota [^3] |
| **Excepciones** | `Exceptions/DomainException.cs` | Excepciones específicas del dominio |
| **Tipos compartidos** | `Shared/Result.cs`, `Shared/TaskErrors.cs` | Tipos comunes compartidos internamente en el dominio |

**Principio clave:** La capa del dominio **nunca depende** de las capas de
aplicación o infraestructura y contiene solo datos y reglas del negocio.

La entidad `TaskItem` es un [*aggregate
root*](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Aggregate.md)
que define y gestiona sus propios datos y reglas del negocio. Está definida así:

```csharp
public class TaskItem
{
    // Método factory para creación segura de entidad
    public static Result<TaskItem> Create(string title, string description, ...)
    {
        // La validación sucede en el dominio
        var validation = Validate(title, description, dueDate);
        if (validation.IsFailure)
            return Result.Failure<TaskItem>(validation.Errors);

        // La creación de la entidad genera un evento del dominio
        var task = new TaskItem { ... };
        task.AddDomainEvent(new TaskCreatedEvent(...));
        return Result.Success(task);
    }

    // Métodos de negocio que refuerzan invariantes
    public Result Complete()
    {
        if (Status == TaskStatus.Completed)
            return Result.Failure(TaskErrors.AlreadyCompleted);
        Status = TaskStatus.Completed;
        AddDomainEvent(new TaskCompletedEvent(...));
        return Result.Success();
    }
}
```

La capa del dominio utiliza el patrón
[Result](https://dev.to/adrianbailador/result-pattern-in-c-fal) para el
resultado de las operaciones. La clase
[Result](./classTaskManagement_1_1Domain_1_1Common_1_1Result.html)
representa tanto resultados exitosos -en cuyo caso incluye también el valor del
resultado- como errores -en cuyo caso incluye la lista de errores-. Esto permite
que un método pueda retornar tanto un resultado como un error.

```csharp
public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }
    …
}

// Uso
var result = TaskItem.Create(…);
if (result.IsSuccess)
    // Usar result.Value
else
    // Manejar result.Errors
```

## 2. Capa de aplicación

La capa de aplicación está definida
[aquí](./namespaceTaskManagement_1_1Application.html).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Comandos** | `Commands/CreateTaskCommand.cs`, `Commands/CompleteTaskCommand.cs` | Objetos para casos de uso que cambian estado (CQRS) |
| **Consultas** | `Queries/GetTaskByIdQuery.cs` | Objetos para casos de uso de lecturas de datos (CQRS) |
| **DTO** | `Queries/GetTaskByIdQuery.cs` (TaskDto), `Requests/CreateTaskRequest.cs` | Objetos de transferencia de datos para entrada/salida |
| **Interfaces** | `Interfaces/IUnitOfWork.cs`, `Interfaces/ITaskRepository.cs`, `Interfaces/ITaskReadRepository.cs`, `Interfaces/IEmailService.cs`, `Interfaces/IDomainEventDispatcher.cs` | Abstracciones para servicios de infraestructura y despacho de eventos |
| **Comportamientos** | `Behaviors/ValidationBehavior.cs` | Comportamientos de pipeline MediatR |
| **Excepciones** | `Exceptions/ValidationException.cs`, `Exceptions/NotFoundException.cs` | Excepciones específicas de la capa de aplicación |
| **Compartido** | `Shared/PagedResult.cs` | DTOs y tipos comunes |

**Principio clave:** La capa de aplicación implementa casos de uso de negocio
pero **delega la lógica de negocio a la capa del dominio**. Básicamente es un
orquestador de objetos del dominio, sin lógica del negocio.

El siguiente diagrama muestra una versión simplificada de un comando de la capa
de aplicación y su interacción con la capa del dominio.

![Diagrama de
secuencia](https://www.plantuml.com/plantuml/png/bLPRJrf147uduJzCx0icL6dJz86eKK4hYKqDa3xnifwToiPbb-xkiLN3V--SlPmRO5iIm-pktBzl1i-jOyRbAmbpPXz71faJMitmPuugmsxdnBfdYMVT3j1dghM3ro27SwxuI1k51WRLzINB7XzCU9FQmDmmPTVCe7AL4afSqKNT7S2rLYxOl1iCFbzTNDSVBxNXQFO-tmg5UnutGQIla-DKJxUp51pX1vLqhtyNPvOvCZwucS_lO2G59NS0p4Qnsw42dCZ4cb7I1qMpx8YoDLfAD46nFBQKVLVHzrUlLapngExbbkxaAtB1A965wWLnUkZLq_tVVD_GNyeeoZjodpXSUPLqA1eGAeNtPsUrMePmzsqsXsDNLBnGhquuYNtWQ3CZrWLP-h0sGXlXdkawnp7biudz54zqQ3IcahVYV6TIy49_Mh3FEkg0ogM4R8dPOnzOxfOKvSKJz7279ry-326BqYae_4LE_YdZFkGMpOGVjZAF6BM29J26C4F5Up4YsNBSjgtvgq1CKiiTJHIffGgSXcM2i64GWHqM5VQQAPUOHy7h9vI7fNGKYDCx_FS6PTw2Nput0rOCIb8hP8dhVMvfHgq5aqkqAJ18L0zkMGQ2fqAVGeSmqYK7b1TSCo5jcEOgMgCZKxIvT4U5Ocxm96d53LOsfTNsfj6NNF1eKBMzJTVzDdMoU2i_LqFuD1ZGRkIHtjqEIelrucPvbY7ohVeQtPmeuGJPl21XfDT8W-rxEU5zc0SWntKWPuwvt9xEDTVzXRehOHqdM0CH-8KhHO_m6obSS0vZlM92NJrHOHIy_Er0xg-sx7T1vCkrjWuHXdcfJL5zF-q43xRyUIHwnRwQj4Qod8twEEt1jU6juQKQuZVNsQXOMs22jucD_XAdo-DRXFau-90mAJQlhsYpKBSwyuGPngryd9uYfI8b_uUCv1wDqSPkTe5GN-3TpZafq3yCtSuV)

La clase `CreateTaskCommandHandler` implementa un procesador de
[MediatR](https://github.com/LuckyPennySoftware/MediatR) para el comando
`CreateTaskCommand`.

```csharp
public sealed class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    …

    public async Task<Result<Guid>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        …

        // Delega a la capa del dominio la lógica de negocio
        var createResult = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatedBy);

        // Persiste la entidad del dominio
        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Despacha los eventos del dominio
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);

        …
    }

    …
}
```

En ese método, las variables `_taskRepository` de tipo `ITaskRepository`
-definido en la capa del dominio-, `_unitOfWork` de tipo `IUnitOfWork` e
`_eventDispatcher` de tipo `IDomainEventDispatcher` -definidos en la propia capa
de aplicación- son asignadas mediante injección de dependencias con clases
implementadas en la capa de infraestructura.

## 3. Capa de infraestructura

La capa de infraestructura está definida
[aquí](./namespaceTaskManagement_1_1Infrastructure.html).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **DbContext** | `Persistence/TaskDbContext.cs` | Contexto de base de datos de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Repositorios** | `Persistence/Repositories/TaskRepository.cs`, `TaskReadRepository.cs` | Implementaciones de acceso a datos con [Entity Framework](https://learn.microsoft.com/en-us/ef/) y [Dapper](https://github.com/DapperLib/Dapper)  respectivamente |
| **Configuraciones** | `Persistence/Configuration/TaskConfiguration.cs` | Configuraciones de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Despacho de eventos** | `EventDispatching/MediatRDomainEventDispatcher.cs` | Implementación de publicación de eventos del dominio con [MediatR](https://github.com/LuckyPennySoftware/MediatR) |

**Principio clave:** La capa de infraestructura implementa interfaces definidas
en las capas del dominio y aplicación usando [inyección de
dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview).

Esta capa implementa el patrón
[Repository](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_08_.Patrones_de_diseno/2_08_Repository.md).
La interfaz `ITaskRepository` definida en la capa de aplicación es implementada
por la clase `TaskRepository` definida en esta capa de infraestructura.

```csharp
// Interfaz definida en capa de aplicación
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    …
}

// Implementación en la capa de infraestructura
public sealed class TaskRepository : ITaskRepository
{
    // Los detalles de infraestructura como el uso de Entity Framework quedan
    // ocultos del dominio y de la aplicación
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
    …
}
```

## 4. Capa de interfaz

En esta demo la interfaz es una API web. La capa de interfaz está definida
[aquí](../src/TaskManagement.API/).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **Controladores** | `Controllers/TasksController.cs` | Endpoints HTTP |
| **Middleware** | `Middleware/ExceptionHandlingMiddleware.cs` | Comportamientos de pipeline transversales |
| **Extensiones** | `Extensions/ClaimsPrincipalExtensions.cs` | Métodos de extensión auxiliares |
| **Solicitudes** | `Requests/CreateTaskRequest.cs` | DTOs de entrada para solicitudes HTTP |
| **Inicio** | `Program.cs` | Raíz de composición de inyección de dependencias |

**Principio clave:** La capa de UI traduce HTTP a comandos o consultas de
aplicación. **No tiene lógica de negocio** y delega todo a la capa de
aplicación.

A continuación un fragmento de la clase `TaskController` con la implementación
del POST de HTTP para crear un cliente -hay ejemplos para probar todos los
endpoints [aquí](../TaskManagement.http)-.

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        // Traduce la solicitud HTTP a un comando de la aplicación
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            userId);

        // Envía el comando a la capa de aplicación
        var result = await _mediator.Send(command, cancellationToken);

        // Traduce resultado a una respuesta HTTP
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);

        return BadRequest(CreateProblemDetails(result.Errors));
    }

    …
}
```

En el programa principal en
[Program](./classProgram.html) se realiza la
inyección de dependencias.

```csharp
// Registra servicios para las abstracciones de la capa del dominio
builder.Services
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

// Registra servicios para las abstracciones de la capa de aplicación
builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))
    .AddValidatorsFromAssembly(...)
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Registra servicios para las abstracciones de la capa de infraestructura
builder.Services
    .AddDbContext<TaskDbContext>(options =>
        options.UseSqlite(connectionString));
```

<!-- markdownlint-disable-next-line MD025 -->
# Cómo navegar el código

1. Comienza con la capa del dominio
   [TaskManagement.Domain](./namespaceTaskManagement_1_1Domain.html)
   * Lee
     [TaskItem](./classTaskManagement_1_1Domain_1_1Entities_1_1TaskItem.html)
     para entender el modelo del dominio
   * Lee [Result](./classTaskManagement_1_1Domain_1_1Common_1_1Result.html)
     para entender el manejo de errores
   * Lee
     [DomainEvent](./classTaskManagement_1_1Domain_1_1Events_1_1DomainEvent.html)
     para entender la arquitectura dirigida por eventos

2. Muévete a la capa de aplicación
   [TaskManagement.Application](./namespaceTaskManagement_1_1Application.html)
   * Lee
     [CreateTaskCommand](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTask_1_1CreateTaskCommandHandler.html)
     para ver cómo se implementa un caso de uso
   * Lee
     [ValidationBehavior](./classTaskManagement_1_1Application_1_1Behaviors_1_1ValidationBehavior-2-g.html)
     para entender preocupaciones transversales
   * Lee
     [GetTaskByIdQuery](./classTaskManagement_1_1Application_1_1Queries_1_1GetTaskById_1_1GetTaskByIdQueryHandler.html)
     para ver otro caso de uso pero usando el patrón CQRS

3. Explora la capa de infraestructura
   [TaskManagement.Infrastructure](./namespaceTaskManagement_1_1Infrastructure.html)
   * Lee
     [TaskRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskRepository.html)
     para ver cómo se implementan interfaces del dominio
   * Lee
     [TaskDbContext](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1TaskDbContext.html)
     para ver la configuración de base de datos
   * Lee
     [MediatRDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1MediatRDomainEventDispatcher.html)
     para ver el manejo de eventos

4. Revisa la capa de interfaz API
   [TaskManagement.API](./namespaceTaskManagement_1_1API.html)
   * Lee
     [TasksController](./classTaskManagement_1_1API_1_1Controllers_1_1TasksController.html)
     para ver endpoints HTTP
   * Lee [Program](./classProgram.html) para ver
     la configuración de inyección de dependencias
   * Lee
     [ExceptionHandlingMiddleware](./ExceptionHandlingMiddleware_8cs.html)
     para ver el manejo de errores

<!-- markdownlint-disable-next-line MD025 -->
# Beneficios de Clean Architecture

Esta demo permite entender los siguientes beneficios de Clean Architecture:

* **Facilidad de testeo**: La lógica del dominio sin dependencias externas
  facilita escribir casos de prueba.

* **Facilidad de mantenimiento**: Separación clara de responsabilidades y
   aspectos.

* **Flexibilidad**: Fácil de intercambiar implementaciones -base de datos,
   servicio de email, etc.-

* **Escalabilidad**: Cada capa se puede optimizar de forma independiente.

* **Reutilización**: La lógica del dominio se puede reutilizar en diferentes UIs
   -web, consola, API, etc.-

* **Independencia**: La lógica de negocio está aislada de los
   [frameworks](#frameworks-utilizados).

<!-- markdownlint-disable-next-line MD025 -->
# Acerca de las pruebas de estabilidad

## Principio de dependencias establas o SDP

El concepto de estabilidad fue introducido por Robert C. Martin en el principio
de dependencias estables o
[**SDP**](https://objectmentor.com/resources/articles/stability.pdf) por *stable
dependencies principle* en inglés.

El principio dice:

\note Las dependencias entre componentes en un diseño deben dirigirse en el
sentido de la estabilidad de los paquetes. Un componente solo debe depender de
componentes que sean más estables que él.

En esta definición, ≪componente≫ significa ≪unidad de despliegue≫, por ejemplo,
una DLL, un JAR, etc.

En otras palabras, si un componente cambia con frecuencia -es decir, si es
inestable- puede depender de otro que cambia poco -es decir, que es más
estable-, pero no al revés. De lo contrario, cambios en un componente inestable
podrían obligar a modificar componentes que deberían permanecer estables.

Vean el siguiente diagrama. El componente `X` es un componente estable. Los
componentes `A`, `B` y `C` dependen de `X`, por lo que `X` tiene tres razones
para no cambiar, decimos que `X` es ≪responsable≫ por `A`, `B` y `C`. Por otro
lado, `X` no depende de ningún otro componente que pueda obligarlo a cambiar,
decimos que es ≪independiente≫.

![](https://www.plantuml.com/plantuml/png/TP2n3i8W48Ptdk9Izzh9gMaAn4vEng4hQOanKguvw62Cxow2RHYKuVBVztDtkMkTL-INoJ1ME_ymkc4GtXdHmpFAb2vsb4XM4rGR5MguNobP1WakfAoR5Mdhjp_Qw2daMI-03grF5RkgEXnvbta2QTJqpptThFxjlLYAzVwx1Hjmcj5-PXUy0EM0vqX4978a8v8vwYFg6OUoR9eWn2haAbYhxHZkzmO0)

\note En este contexto, que un componente sea **estable**, significa que es
resistente a los cambios -o que tiene razones para no cambiar- debido a las
dependencias que otros componentes tienen sobre él.

Vean ahora este otro diagrama a continuación. El componente `Y` es un componente
muy inestable. Como ningún otro componente depende de `Y`, decimos que es
≪irresponsable≫. Pero `Y` depende de los componentes `D`, `E` y `F`, por lo que
`Y` tiene tres posibles orígenes externos de cambios: cambios en cualquiera de
estos componetes podrían hacer que `Y` cambie. Decimos que `Y` es ≪dependiente≫.

![](https://www.plantuml.com/plantuml/png/TP0n3u8m48Nt_eeBxiZaH0WKON8o3ZWsS356k99h6OpnlrifaYPgXykxxxrhSysDLModeHSYDVRUOoR0dQFM5XwpzEr4NQcPa25PgLJjxSBUwZN3YHMPKJ6INJFOu-3wu2w0MfFr4viasLL57NXISuVypprS2NzxJ-5hkbss4AqUHatljwDp1Cf04wBzZ8eGLJEItwbID137MURYYv0VKGgH8tNk_ru0)

\note En este contexto, que un componente sea **inestable**, quiere decir que es
libre para cambiar -o que no hay razones para impedir cambios- porque ningún
otro componente depende de él.

Una forma de medir la estabilidad de un componente es contar las referencias que
entran y que salen del componente:

* **Fan-in**: dependencias entrantes. Es el número de tipos -clases, interfaces,
  etc.- fuera del componente que dependen de tipos dentro del componente.

* **Fan-out**: dependencias salientes. Es el número de tipos dentro del
  componente que dependen del tipos fuera del componente.

* **Instability**: $I = \frac{\text{Fan-out}}{\text{Fan-in} + \text{Fan-out}}$.
  Varía entre 0 y 1. La inestabilidad es mínima -$I = 0$-, es decir, el
  componente tiene muchos motivos para no cambiar, cuando otros componentes
  dependen de él -$Fan-in > 0$-, pero él no depende de ningún otro -$Fan-out =
  0$-; en ese caso componente es independiente. La inestabilidad es máxima —$I =
  1$—, es decir, el componente no tiene razones para que no pueda cambiar,
  cuando ningún otro componente depende de él -$Fan-in = 0$-, pero él depende de
  otros componentes -$Fan-out > 0$-; en este caso el componente es irresponsable
  y dependiente.

El principio de dependencias estables dice que la métrica $I$ de un componente
debe ser mayor que la de los componentes de los que depende; dicho de otra
forma, la métrica $I$ decrece en la dirección de la dependencia.

![](https://www.plantuml.com/plantuml/png/XP0z3u9038Rt-nKDxdWkJaZW32GaJiR1mUM4nXYF7Yd7O8p_tHCK6JHsQDxswKVQf7MQNLSR8QBMNGt60hnjhNLmw-KGTbIjBHCoo36hT7avyO2CPTYnfhRHZEJ66ZHO-tWo2o2tv8QEcPJBBCqdJ5cka7_HpPdmExkBmKVprnMYj7LZwRaxA0ZzcKU31ofMYm1q2-aVhgOy-r3PqxqGAOHX77kbta8aI9N_v0C0)

El diagrama anterior muestra una situación ideal: los componentes inestables
dependen de un componente estable. Ahora bien, supongan que un nuevo componente
`Flexible` ha sido diseñado para ser fácil de modificar, es decir, para que sea
inestable. Por alguna razón, en el componente `Estable` se agrega una referencia
a `Flexible`. Esto viola el principio de dependencias estables porque
necesariamente la métrica de inestabilidad de `Flexible` es mayor que la de
`Estable` que es cero; como consecuencia, `Flexible` ya no sería tan fácil de
modificar.

![](https://www.plantuml.com/plantuml/png/XT2z2i8m403WFKznsBtMnKcBhHWeE8c3e0xH7YAcbt99GH7VtTZuqq4nGxZBTvTmcLYds3ML5c9atKLZAi2lXJ4EhcqOqeRAgM52XkHzcglzwS21j65OiY9RAqPoxnhKMFcySnU0DEH2HvGamracEvORBf5_wVn8-Drk8koZ_dCAiJTLRUZnTgyWz6FkD6we60zwe2naVppl-koZIQkhhagDvwFdLgGG6SHnclgQ_1Mi85v7ZENsaVgA2LBfl_mE)

Para resolver el problema, se utiliza el principio de inversión de dependencias.
Supongan que la dependencia de `Estable` a `Flexible` se genera por una
dependencia entre una clase `Consumer` en `Estable` y una clase `Provider` en
`Flexible`, tal como se muestra en el diagrama a continuación.

![](https://www.plantuml.com/plantuml/png/TP71IWCn48RlynJ3zgvxybINsgKBWWSH2u9wIBD33IOpooIhBT9tDpQbhA0vXCp_vvlCJ1On6KrZy03pcFQU6i2y7aGIVXsF9HovRqMP5EkpkZJsVQl5ygBug8RZO9GudU-Gfv3ZIIx4JZWLObRNLwkRRk8jHNlI_zmxn_ItTu2opozFNO3nNZw3spdUmflh2IaCQO-1meOq5iawjPu0h8H1-AVEQfK7il7qmhVNTOKcujEfTkjDZ5Yrmd4Cf4Tl3OSfstdQkG8t1Uv-m_Sg7_dT0hSPXZLUFC-K-jUcoG8iYFly6Ty0)

El principio de inversión de depdencias dice que `Consumer` -que es concreta- no
puede depender de `Provider` que también es concreta; debe depender de una
abstracción. Definimos entonces una interfaz `IProvider` que tenga todos los
atributos y métodos que `Consumer` necesita en un nuevo componente
`Abstractions`. Luego la clase `Consumer` referencia esa interfaz y la clase
`Provider` la implementa.

![](https://www.plantuml.com/plantuml/png/TL7DIyCm5B_dhtZK--p9KotxO8M2Xo8E5BoawNC6avVoakc6xdytNSfOnUKGydvV_9BCVP0STjO8KVXmC5WAYElHkG3VfsCQTzIj71Cov6TqATN7bXDb7CCJI_AzPAHmrg116-UBB83AKKYEcpo_Mo-g2sxfk4E-nZveml-veqXxSJlUGaXZt9VLUqrRyQux1BHzE815soBx12dDog0GojdUqT0pMyS7QGs-KdsVPo0zF8_TbP7UGxPov7SM-SHjn75YNhG-i5H1Hr5AYAANCK1JG7wJ2Y6h6tQViK1AgPi_CPN1lKubobIYcfQOs5U3UmEpiXoIwWQAdzaiecAErCMV_GK0)

Ahora, con ese cambio, tanto `Consumer` como `Flexible` dependen de
`Abstractions` que es estable -$I=0$-. Noten que luego de este cambio la
inestabilidad decrece desde `Flexible` -$I=1$- hacia `Abstractions` -$I=0$-,
cumpliendo el principio de dependencias estables.

## El principio de abstracciones estables

Algunas piezas del software no van a variar frecuentemente; estas piezas
representan decisiones de alto nivel sobre arquitectura y políticas. No es
deseable que estas decisiones de arquitectura y del negocio sean volátiles, por
lo que deberían estar en [componentes estables](link a SDP) -con $I = 0$-. Los
componentes inestables -con $I = 1$- deberían contener piezas de software que
sean volátiles, es decir, que se puedan cambiar fácil y rápidamente.

Pero si estas piezas de alto nivel estan en componentes estables, van a ser
difíciles de cambiar. Esto puede resultar en una arquitectura inflexible.

¿Cómo puede un componente que es máximamente estable -con $I = 0$- ser lo
suficientemente flexible como para soportar cambios? La respuesta está en el
[principio
abierto-cerrado](https://github.com/ucudal/PII_Guias/blob/main/OCP.md). Este
principio dice que es posible y deseable que las clases sean lo suficientemente
flexibles como para ser extendidas sin que tengan que ser modificadas. Las
clases abstractas -o las interfaces, e incluso los tipos genéricos- cumplen con
este principio.

El principio de abstracciones estables, o
[**SAP**](https://objectmentor.com/resources/articles/stability.pdf) por *stable
abstractions principle* en inglés, fue introducido por Robert C. Martin. El
principio dice:

> Los componentes que sean máximamente estables deben ser máximamente
> abstractos. Los componentes inestables deben ser concretos. La abstracción de
> un componente debe ser proporcional a su estabilidad.

El principio establece una relación entre la estabilidad y el nivel de
abstracción de un componente. Por un lado, plantea que un componente estable
debería ser también abstracto, de modo que su estabilidad no se convierta en un
obstáculo para su extensión. Por otro lado, sostiene que un componente inestable
debería ser concreto, ya que precisamente su inestabilidad facilita la
modificación del código específico que contiene.

Combinando el *SAP* y el [*SDP*](link to sdp) llegamos al
[*DIP*](https://github.com/ucudal/PII_Guias/blob/main/DIP.md) para componentes.
El *SDP* nos dice que las dependencias deben ir en dirección de la estabilidad y
el *SAP* nos dice que la estabilidad implica abstracción, por lo que las
dependencias deben ir en dirección de la abtracción.

El *DIP* aplica a tipos que bien son abstractos o bien son concretos
-en este contexto, son tipos abstractos las clases abstractas, las
interfaces y los tipos genéricos-. En cambio, tanto el *SDP* como el *SAP*
aplican a componentes, que pueden ser parcialmente abstractos o parcialmente
concretos.

¿Cómo medimos la abstracción? La métrica $A$ de abstracción de un componente se
define como $A = N_a / N_c$, donde $N_a$ es el número de tipos abstractos y
$N_c$ es el total de tipos.

```mermaid
xychart-beta
    x-axis "I" [0, 0.5, 1]
    y-axis "A" 0 --> 1
```
