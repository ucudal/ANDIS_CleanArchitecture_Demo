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
Containers](https://www.plantuml.com/plantuml/png/ZLJ1RjD04BtxAuPmIGLgBZqXX9hQXWQ5D91KFKPZUs8jNU-QsNMfXVWaJls4_1YpjWaDZIfyMi-yVM_cpTZdpYFxfjBH2sLotHG4R-EpINosYQtng0pnksX9xAmPDuxuDOokNOEix4YECHD22oKvhwn142v2Dpg9LgtpL6qkR6CAv7RiMYVmDRfR-8G6Iwh8UEXHenFu4O4yUyKnrcg0_qMVJfUpe3bTBKyNwLhGJ7136GpnzBwsXe1CKLjbl8FBzNe9DJA2nhx4tOC1QN1VvQ6eCbl6fxAp47QUczmtJ45fhZ8ctlt2G1PhTEGWVXaoiwhMtIs4MZnJqfx95GA1L4GbOous2PhkFR4vKWFMzP76wrgh78F5_osVexCk01fdtq1kApI515ev6jTeVqIx490oJ-MJBlwySa9RrPdlLO65X4K6zEwXbBe3s51ffNVHjzbNybB97MhB_zQGPECY4xgB95nQztCAxo8L8Wc6gwlFS-MxIKnrXJdj5q_eYmx1f3khKNeMZP_H5zB3-lQhTUqmt0yhClcukbe8A42wP3-3cRcp-Q6FOOoF1jmG-pxdK4lRuLGpkP3Dmwuz9lspNDBgLfMDB2KP80t8ZEPRGmDsW0qbF2EsvnRWVd3ILU2iR47Q1gnab95FzO2yStXED4bTpYhhZ4nbSBw5zumLVRTy-oeHPWhpMLrUtMpcwOTqaOnFedFvlCBFuZS)

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
Components](https://www.plantuml.com/plantuml/png/ZHRBRXityEwlcEgK08QQGtffYoAsfJO2ubZn8pqQezsHmeHBRackRA7eL-KJycET8VUfNRKnO8dpVi_eZG_eGbLek7hzU6MBqXeouN6XSEUmEFj1cKnNES6lLwz_x7FCIvRvxUpAce3Aa7kyj9N9qHrUOAdEONQF_iir6jnH8TmNwzNi9Vnz1lpNwdXHg3pNz8IEM6Bvd54Pb3Nm5askbTd1TKkVCSF5tNh-VddFyewEIEp6Nf4B15lK6ZsmIxuaXpc_oK1bW9op3dvwzMhsyiYBZ6DmLcjojTyUhbhKoF2Gj7mM9MmYBwqomSFjykuUIdG8E_91VVjgWBF7EIRq8ykb8y_Vv3ohMhU0ABwfGba5QB8A5pcM3OrTyC5L1mwCPSgA3N6OR-_lrt00piCJ53OdBVR-cQvGEL6XijGgGod0HAKsILQHU7hP0kBDshgG7XLNm7g9bqKaz-mE0nwruZU_Ci5bwAq0bRSJfIWABhwFGQQdQ5v9MZ0BjewVanb8_Ey8vr6BfcFyoCHVx6KAuqDwJHZuWu9ekJoiyl_Gj8znIS9Ov6CBY885PTQX2FL8PZVb-4WjxVapZa3gYwN0q21YFtQbWhNbvs5MrxF9GVxU1hMjUMhHSVT9uVe5h7kkXLUYyfPAwnKxUZY7rODHuMRxftLV65WMgFGTkRtAU7xdy_cfViijSvfnooqYVgBR-5-5LAHb_OnPZVnmdPgxrhSAL02FCEXlNtSSiRIMePtDb9sgM4Ktcy-KnS0_2WWrF6c0WqzJYKPio8AA3QvFceZLyvskMBseAzLnNMsqyf-u6TgQnQR88_aS1bMCk5DPLMRhS9pKbQ1bFMIXukKuJgvh2feIVDi3Omum85na6VdK0-9AaJ9mr0z03GRp3ZlAY9DPo9Ef-8gjTdlTJtnI8zIVGIElomr_fbMyLxpTW7iLEEiiDjAVRoGvz1p4-URTs61mgtQLQoSpzjC1VkVZHa_Snt6f_mAg3RFVvDFLNIXVOiW-nNlHBpDqbAxW3QwkEDjtj4FGt4TH6tyZ-Ce3RplwZ3cxCpuKD1YrMPmygE5qXdYT61pWLhsG-gCwxAV5XXF8t7SVtYa-drBz3Vge8A7Ymj45PcHujh1UyEr1koKzEEE3Oxeq-uhqdcg-FcrmK1uyLtXqFIQa1WVp7P4qIti2Q_u-py1ABo4zO-awrRBzbJwGJGjTVaSaqIatczDrokY1Jrx3tLwXxs7kLFSMuyC4kTde3sCxOzBHdVbVXdQpyRndlFgu7LhErgsMQLb8xoJoSECqXhfWQxRU89_a6GtWQSx1o7o_mkFpkp9xc-5a9Drcwtv_fhKbHsbIfB-ikapti74OrsoQrvwq67EyuL-N_6Fxx5y)

Puedes ver claramente como las clases en los contenedores mantienen las
referencias del diagrama de contenedores, es decir, "de arriba hacia abajo".

## Diagrama C4 de código

El último diagrama C4 es el de [código](https://c4model.com/diagrams/code).
Muestra cómo los componentes son implementados en código.

En este caso usamos un diagrama de clases -parcial- mostrando algunas de las
clases involucradas en la creación de una tarea.

![Task Management
Components](https://www.plantuml.com/plantuml/png/nLVRSjem47ttLsZoKj86Fg33a12W6S-aXI6qVLRa3L4naYl9D4oIVwyk5iZ6DccQJfyOhSxkdhsmArz8XONArsaKPPYiy1BGoPYlCMNe6cz1d20iKI5uZ10YAPOIdIomNCKArYUetmUcgDeE1lOUeGyeJj17T9NJf3Wlg4f1YwGIb2qBuHWa4JHJbBFgrKnGBhH9BJT-_D4hvZ36ojWpFmkwXlFYPYH0IvAX2kxALvVRCZCNs6IZGv3w9fu3JgOitOvucW8ntFe1P51eFUgSAKgU2wiTPU8xGya-eZEK5QIrsF4zGyHpEDMkvY3pLFLz7htfKGfOXAmw1hpXbl5pj2z0WB4riFnS2KAOiL_4NekrvJ4SNbAocYHQNrKdgFiq25CRFGVDDSoob19iwxxli50Qj9cpQWcoBBVinujfoFrjMkjQvoHzQNyTn7z0TfghUhQLJ5jjoXI8EqnqRk9Al9SyPmaMMsE96W7Ak0XBDta0aYiOoYqZ7OlGROOPWJIrkLpm5R3J8jXsXuvpw95RITNbJ83KDRt92G4fEmuKD6c0-OPJcbWU4o6ua1smFmqT7SQk2MAYk721-zFV1buuyhJOiibb0eGB_0OUmrGR-HmoBgast9BWOP8u9qehjFimy2jGbzkuKA59Yy9ks9mFwlW5VzWnbHbMv1v4CqsFSOxjJ95_c96lZAhftNSkLirkRl06HlUOBK4wJytXKJUXdqlZ9cPt0kiHa1EL2mWcJkMYijlAXLhgBlc5j_-fI2LQkZ4JGsaw_upuxG-zf09MkyNqIV3rnAvbIz0RT-f-6F42uM-DrngVuziHrqLxKFMPU4s1zWvYDXFS39B6xZlq-9hEAssWMHnk7dsg55-QW8Lv0bW6P7SAI1IhNy3F7AJootrXko5pumz4MvtZeNrVk7V0e74MphmjmsHdE23Y39Z7IsZmtmnhJV4Tca-kXuju-kKcgkGAzNe3rBJ2j9bSubgLYeP6ccGUWUxoek4fGDIuoL3_gTThKAJhB8MrFk4eQYHUqTzlYhA1TltsdLEsT0JRxRPcsUskAAhLjHKgpxUYCCtm_PG-2gtz4n_LQgpS4SMgdc-SHWK_v3mmggR2bkeWFM6HWsJimBi1MS7MPyEgjO-zifqA7n-8c-Kc5Cus58I2ADe5-BxR3H_WvYqRV5D4qGMmn7pp_GO)

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
  y eventos. El domino utiliza
  [eventos](./classTaskManagement_1_1Domain_1_1Events_1_1DomainEvent.html)
  para informar cuando se crea, se completa, o se asigna una tarea, o cuando
  cambia su prioridad -ver por ejemplo la propiedad `TaskItem.DomainEvents` y el
  método `TaskItem.Create` en
  [TaskItem](./classTaskManagement_1_1Domain_1_1Entities_1_1TaskItem.html)-;
  por esto, esta aplicación también utiliza una [arquitectura dirigida por
  eventos](https://github.com/ucudal/ANDIS_Conceptos/blob/main/3_Plantillas/3_13_Event_Driven_Architecture.md).
  La capa del dominio tiene la responsabilidad de generar eventos, pero es la
  capa de aplicación la que tienen la responsabilidad de procesarlos -ver por
  ejemplo `CreateTaskInteractor.ExecuteAsync` en
  [CreateTaskInteractor](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTaskInteractor.html)-.
  El proyecto
  [`TaskManagement.Domain`](./namespaceTaskManagement_1_1Domain.html) no
  referencia ningún otro proyecto, es el centro de los círculos concéntricos.

2. [TaskManagement.Application](./namespaceTaskManagement_1_1Application.html)
  es otra librería de C# en la que se definen las funcionalidades de la
  aplicación -comandos y consultas-, o dicho de otra forma, donde se implementa
  la lógica de los casos de uso. Esta aplicación usa el patrón
  [CQRS](https://github.com/ucudal/ANDIS_Conceptos/blob/main/2_Tecnicas_y_herramientas/2_09_.Patrones_de_arquitectura/2_09_CQRS.md)
  donde los comandos están separados de las consultas -ver por ejemplo la clase
  [CreateTaskInteractor](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTaskInteractor.html)
  y la clase
  [GetTaskByIdInteractor](./classTaskManagement_1_1Application_1_1Queries_1_1GetTaskByIdInteractor.html)-.
  La capa de aplicación define -y utiliza- abstracciones que emplean el patrón
  de [inyección de
  dependencias](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview)
  -ver por ejemplo las interfaces
  [IEmailService](./interfaceTaskManagement_1_1Application_1_1Ports_1_1IEmailService.html),
  [ITaskRepository](./interfaceTaskManagement_1_1Application_1_1Ports_1_1ITaskRepository.html),
  [ITaskReadRepository](./interfaceTaskManagement_1_1Application_1_1Ports_1_1ITaskReadRepository.html)
  o
  [IUnitOfWork](./interfaceTaskManagement_1_1Application_1_1Ports_1_1IUnitOfWork.html)-[^3].
  Cada caso de uso tiene además un puerto de entrada, que usa el controlador, y un
  puerto de salida, que implementa un presenter en la capa de interfaz API -ver
  por ejemplo las interfaces
  [ICreateTaskInputBoundary](./interfaceTaskManagement_1_1Application_1_1Commands_1_1ICreateTaskInputBoundary.html)
  y
  [ICreateTaskOutputBoundary](./interfaceTaskManagement_1_1Application_1_1Commands_1_1ICreateTaskOutputBoundary.html)-.
  Las clases concretas que implementan estas abstracciones están definidas en la
  capa de infraestructura -ver por ejemplo las clases
  [TaskRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskRepository.html)
  y
  [TaskReadRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskReadRepository.html),
  respectivamente- y también son creadas en tiempo de ejecución en la capa de
  interfaz API -ver las llamadas a `builder.services…
  .AddScoped<IUnitOfWork>(…), .AddScoped<ITaskReadRepository,
  TaskReadRepository>(), y .AddScoped<IDomainEventDispatcher,
  InProcessDomainEventDispatcher>()` en
  [Program](./classProgram.html)-. Los eventos
  creados en la capa del dominio son procesados en la capa de aplicación usando
  una abstracción -ver la interfaz
  [IDomainEventDispatcher](./interfaceTaskManagement_1_1Application_1_1Ports_1_1IDomainEventDispatcher.html)-;
  y esa abstracción también está implementada en una clase definida en la capa
  de infraestructura -ver la clase
  [InProcessDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1InProcessDomainEventDispatcher.html)-.
  El proyecto
  [`TaskManagement.Application`](./namespaceTaskManagement_1_1Application.html)
  referencia solamente el proyecto
  [`TaskManagement.Domain`](./namespaceTaskManagement_1_1Domain.html), la
  dependencia es de un círculo externo al centro de los círculos concéntricos.

3. [TaskManagement.Infrastructure](./namespaceTaskManagement_1_1Infrastructure.html)
  es otra librería de C# en la que se definen cómo se implementa la
  infraestructura para las abstracciones definidas en la capa de aplicación de
  -repositorios, despacho de eventos y persistencia-. Las abstracciones
  [ITaskRepository](./interfaceTaskManagement_1_1Application_1_1Ports_1_1ITaskRepository.html)
  y
  [ITaskReadRepository](./interfaceTaskManagement_1_1Application_1_1Ports_1_1ITaskReadRepository.html)
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
  [IDomainEventDispatcher](./interfaceTaskManagement_1_1Application_1_1Ports_1_1IDomainEventDispatcher.html)
  definida en la capa de aplicación se implementa con la clase
  [InProcessDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1InProcessDomainEventDispatcher.html)
  de esta capa de infraestructura y la instancia se crea en tiempo de ejecución
  también con injección de dependencias en la capa de interfaz API en
  [Program](./classProgram.html) -ver
  `builder.services...AddScoped<IDomainEventDispatcher,
  InProcessDomainEventDispatcher>()`-. El proyecto
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
  También contiene los presenters que convierten el resultado de cada caso de uso
  en una respuesta HTTP -ver por ejemplo
  [CreateTaskPresenter](./classTaskManagement_1_1API_1_1Presenters_1_1CreateTaskPresenter.html)-.
  Solo [Program](./classProgram.html), la raíz de composición, usa la capa de
  infraestructura.
  El archivo [`TaskManagement.http`](../TaskManagement.http) tiene ejemplos para
  invocar la API REST.

[^3]: Algunos autores definen las abstracciones relacionadas con el dominio en
    la capa del dominio, aunque no sean utilizadas en esa capa. Siguiendo esos
    autores, interfaces como `ITaskRepository` se definirían en la capa de
    dominio. En esta demo, interfaces como esa son definidas en la capa de
    aplicación, porque es allí donde se usan.

<!-- markdownlint-disable-next-line MD025 -->
# Conceptos de Clean Architecture en esta demo

La siguiente tabla muestra dónde está cada concepto, con el caso de uso de crear
una tarea como ejemplo.

| Concepto | Círculo | Elemento en esta demo |
| -------- | ------- | --------------------- |
| Entidad | Entidades | `TaskItem` |
| Objeto valor | Entidades | `Email` |
| Evento del dominio | Entidades | `TaskCreatedEvent`, `TaskCompletedEvent` |
| *Interactor* | Casos de uso | `CreateTaskInteractor` |
| DTO de entrada y de salida | Casos de uso | `CreateTaskInput`, `CreateTaskOutput` |
| Puerto de entrada -*input boundary*- | Casos de uso | `ICreateTaskInputBoundary` |
| Puerto de salida -*output boundary*- | Casos de uso | `ICreateTaskOutputBoundary` |
| Puerto de salida -repositorios y servicios externos- | Casos de uso | `ITaskRepository`, `IUnitOfWork`, `IEmailService` |
| Adaptador conductor -*controller*- | Adaptadores de interfaz | `TasksController` |
| *Presenter* | Adaptadores de interfaz | `CreateTaskPresenter` |
| *View model* | Adaptadores de interfaz | `TaskViewModel` |
| Adaptador conducido -*gateway*- | Adaptadores de interfaz | `TaskRepository`, `LoggingEmailService` |
| Objeto del ORM | Frameworks & Drivers | `TaskRecord` |
| Configuración de la inyección de dependencias | Frameworks & Drivers | `Program.cs` |

Los cuatro proyectos no coinciden exactamente con los cuatro círculos:
`TaskManagement.API` y `TaskManagement.Infrastructure` contienen tanto adaptadores
de interfaz como código que depende de los frameworks.

La regla de dependencias se verifica con pruebas de arquitectura en
[`ArchitectureTests.cs`](../test/TaskManagment.Tests/ArchitectureTests.cs), que se
ejecutan en el *pipeline* de CI. Las reglas de negocio y los casos de uso se
prueban sin base de datos ni servidor web en
[`TaskManagement.Application.UnitTests`](../test/TaskManagement.Application.UnitTests).

<!-- markdownlint-disable-next-line MD025 -->
# Frameworks utilizados

La aplicación utiliza los siguientes frameworks.

## Entity Framework Core

[Entity Framework
Core](https://learn.microsoft.com/en-us/ef/) es el ORM de .NET. En esta demo se
usa solo en la capa de infraestructura, para las escrituras.

## Dapper

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
| **Comandos** | `Commands/CreateTaskInteractor.cs`, `Commands/CompleteTaskInteractor.cs` | *Interactors* de casos de uso que cambian estado (CQRS) |
| **Consultas** | `Queries/GetTaskByIdInteractor.cs` | *Interactors* de casos de uso de lecturas de datos (CQRS) |
| **Puertos de entrada y salida** | `Commands/ICreateTaskInputBoundary.cs`, `Commands/ICreateTaskOutputBoundary.cs`, etc. | Contratos de cada caso de uso con los controladores y los presenters |
| **DTO** | `Commands/CreateTaskInput.cs`, `Commands/CreateTaskOutput.cs`, `Queries/TaskDto.cs`, etc. | Objetos de transferencia de datos para entrada/salida |
| **Decoradores** | `Commands/ValidatingCreateTaskInteractor.cs` | Validación de la entrada antes de ejecutar el caso de uso |
| **Puertos** | `Ports/IUnitOfWork.cs`, `Ports/ITaskRepository.cs`, `Ports/ITaskReadRepository.cs`, `Ports/IEmailService.cs`, `Ports/IDomainEventDispatcher.cs` | Abstracciones para servicios de infraestructura y despacho de eventos |
| **Manejadores de eventos** | `Events/IDomainEventHandler.cs`, `Events/TaskCompletedNotificationHandler.cs` | Reacciones de la aplicación a los eventos del dominio |
| **Compartido** | `Shared/PagedResult.cs`, `Shared/TaskPriorityLevel.cs` | DTOs y tipos comunes |

**Principio clave:** La capa de aplicación implementa casos de uso de negocio
pero **delega la lógica de negocio a la capa del dominio**. Básicamente es un
orquestador de objetos del dominio, sin lógica del negocio.

El siguiente diagrama muestra una versión simplificada de un comando de la capa
de aplicación y su interacción con la capa del dominio.

![Diagrama de
secuencia](https://www.plantuml.com/plantuml/png/bLLDRnen4BtxLupI2qWfeLNL0q8eX10LgL8Ge9ouEEj9OiNOrFQYe2Z_lMFluZKRegOv4C-7dz-yiMSldUVMbni5A-wU9nQvbqOlyK-9ka0sT7wlSCI0_XR6U7YDoyeijPWOgz52FtclUF7yQAkGCXPMbcktvHQrJpceS4DsXWLmOxIlTdpfztzEnpTPxCfOWVQZw6-fyNJiZLM_mukw2lOa1S83rN5lNXZZXQTTPxzMgpbCb2HAPy1TlMH4sij2RZanFGlYk0bnj4Ofj5LUCezpxxYIWcJKZr5E37jdsgEDvwrrPvRxjwM_2d9nk-z6s7gxQQ6UmehvZVcUyipZ9YOTZ8ymvnOT1g1ZUhUbVySlfJ9MAGLVHwDCZ07CRvShkE1RUU63R6lzwgLNU0u2NM7bDjomSzXQQQpqUtANU4q7liMEo1qjSssRPJmeoJE0wGiMfSUns-kYqzAqcu4rYdBbIQzTi466j7X3KZmUaEeR79Q7XXezjTPOry7uhyjGkHhzGxIcE__a6pOdpbbNWxyn8-gXdOEqwbLudLwlruPAlG_UWz62gKaZt9UEtfDpO4k5haOYrHPamauQHKaYlR06hLth0bsfVE-6IrLQ71uIHsbhgwB_KBqQ8QHyuEJ0KOvxa2TP7TKu5WAkpOPBFTqHFjtqDF9GH5VtyuKjow8WcKuK9l2qyYUApd7dQ9rq7iAJ0Qx5MWj9WzCNJo0YQS10qNrUm9fYf_sSQmLr9LX6Dxjp8Pf5cXVKVexPiPKoB7agxVB1C1iNePzrOeuKC-Cq6S3t_ZUekm9XNlxez-cp8XRXc-HyJYkR4koIADDdZFq5)

La clase `CreateTaskInteractor` implementa el caso de uso. El resultado no se
devuelve al controlador: sale por el puerto de salida
`ICreateTaskOutputBoundary`, de modo que el caso de uso no conoce a quién le
entrega el resultado ni en qué formato se muestra.

```csharp
public sealed class CreateTaskInteractor : ICreateTaskInputBoundary
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ICreateTaskOutputBoundary _output;

    …

    public async Task ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default)
    {
        …

        // Delega a la capa del dominio la lógica de negocio
        var createResult = TaskItem.Create(
            input.Title,
            input.Description,
            ToDomainPriority(input.Priority),
            input.DueDate,
            input.CreatedBy);
        if (createResult.IsFailure)
        {
            _output.PresentValidationErrors(createResult.Errors);
            return;
        }

        // Persiste la entidad del dominio
        var task = createResult.Value!;
        await _taskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Despacha los eventos del dominio
        await _eventDispatcher.DispatchAsync(task.DomainEvents, cancellationToken);

        …

        // Entrega el resultado por el puerto de salida
        _output.PresentSuccess(new CreateTaskOutput(task.Id));
    }

    …
}
```

En ese método, las variables `_taskRepository`, `_unitOfWork` y
`_eventDispatcher` -de tipos definidos en esta capa de aplicación- son asignadas
mediante inyección de dependencias con clases implementadas en la capa de
infraestructura. La variable `_output` es asignada con un presenter de la capa de
interfaz.

## 3. Capa de infraestructura

La capa de infraestructura está definida
[aquí](./namespaceTaskManagement_1_1Infrastructure.html).

| Componente | Archivos | Propósito |
| ---------- | -------- | --------- |
| **DbContext** | `Persistence/TaskDbContext.cs` | Contexto de base de datos de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Repositorios** | `Persistence/Repositories/TaskRepository.cs`, `TaskReadRepository.cs` | Implementaciones de acceso a datos con [Entity Framework](https://learn.microsoft.com/en-us/ef/) y [Dapper](https://github.com/DapperLib/Dapper)  respectivamente |
| **Configuraciones** | `Persistence/Configuration/TaskConfiguration.cs` | Configuraciones de [Entity Framework](https://learn.microsoft.com/en-us/ef/) |
| **Objetos del ORM** | `Persistence/Records/TaskRecord.cs` | Representación de una fila para [Entity Framework](https://learn.microsoft.com/en-us/ef/); no sale de esta capa |
| **Despacho de eventos** | `EventDispatching/InProcessDomainEventDispatcher.cs` | Entrega los eventos del dominio a sus manejadores, en el mismo proceso |

**Principio clave:** La capa de infraestructura implementa interfaces definidas
en la capa de aplicación usando [inyección de
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
        var record = await _dbContext.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        // Los objetos del ORM no salen de esta capa
        return record?.ToEntity();
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
| **Presenters** | `Presenters/CreateTaskPresenter.cs`, `Presenters/CompleteTaskPresenter.cs`, `Presenters/GetTaskByIdPresenter.cs` | Convierten el resultado de un caso de uso en una respuesta HTTP |
| **View models** | `ViewModels/TaskViewModel.cs` | Datos tal como se muestran en la respuesta HTTP |
| **Middleware** | `Middleware/ExceptionHandlingMiddleware.cs` | Respuesta para fallas inesperadas |
| **Solicitudes** | `Requests/CreateTaskRequest.cs` | DTOs de entrada para solicitudes HTTP |
| **Inicio** | `Program.cs` | Raíz de composición de inyección de dependencias |

**Principio clave:** La capa de UI traduce HTTP a los DTO de entrada de los casos
de uso, y sus resultados a respuestas HTTP. **No tiene lógica de negocio** y
delega todo a la capa de aplicación.

A continuación un fragmento de la clase `TaskController` con la implementación
del POST de HTTP para crear un cliente -hay ejemplos para probar todos los
endpoints [aquí](../TaskManagement.http)-.

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        [FromServices] ICreateTaskInputBoundary createTask,
        [FromServices] CreateTaskPresenter presenter,
        CancellationToken cancellationToken)
    {
        // Traduce la solicitud HTTP al DTO de entrada del caso de uso
        var input = new CreateTaskInput(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            GetUserId(User));

        // Ejecuta el caso de uso a través de su puerto de entrada
        await createTask.ExecuteAsync(input, cancellationToken);

        // Devuelve la respuesta HTTP que construyó el presenter
        return presenter.Result;
    }

    …
}
```

El presenter implementa el puerto de salida del caso de uso. Decide los códigos
de estado y el formato de la respuesta. Hay una instancia por solicitud: el caso
de uso escribe en el mismo objeto que lee el controlador.

```csharp
public sealed class CreateTaskPresenter : ICreateTaskOutputBoundary
{
    public IActionResult Result { get; private set; } = …

    public void PresentSuccess(CreateTaskOutput output)
    {
        Result = new CreatedAtActionResult(
            nameof(TasksController.GetById),
            "Tasks",
            new { id = output.TaskId },
            output.TaskId);
    }

    public void PresentValidationErrors(IReadOnlyList<string> errors) =>
        Result = ErrorResponses.ValidationFailed(errors);
}
```

En el programa principal en
[Program](./classProgram.html) se realiza la
inyección de dependencias.

```csharp
// Registra los adaptadores de la capa de infraestructura para los puertos de salida
builder.Services
    .AddDbContext<TaskDbContext>(options => options.UseSqlite(…))
    .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TaskDbContext>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IDomainEventDispatcher, InProcessDomainEventDispatcher>()
    .AddScoped<IEmailService, LoggingEmailService>();

// Registra el presenter, el caso de uso y su decorador de validación
builder.Services
    .AddScoped<CreateTaskPresenter>()
    .AddScoped<ICreateTaskOutputBoundary>(sp => sp.GetRequiredService<CreateTaskPresenter>())
    .AddScoped<CreateTaskInteractor>()
    .AddScoped<ICreateTaskInputBoundary>(sp => new ValidatingCreateTaskInteractor(
        sp.GetRequiredService<CreateTaskInteractor>(),
        sp.GetRequiredService<ICreateTaskOutputBoundary>()));
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
     [CreateTaskInteractor](./classTaskManagement_1_1Application_1_1Commands_1_1CreateTaskInteractor.html)
     para ver cómo se implementa un caso de uso
   * Lee
     [ValidatingCreateTaskInteractor](./classTaskManagement_1_1Application_1_1Commands_1_1ValidatingCreateTaskInteractor.html)
     para ver cómo se agrega validación sin cambiar el caso de uso
   * Lee
     [GetTaskByIdInteractor](./classTaskManagement_1_1Application_1_1Queries_1_1GetTaskByIdInteractor.html)
     para ver otro caso de uso pero usando el patrón CQRS

3. Explora la capa de infraestructura
   [TaskManagement.Infrastructure](./namespaceTaskManagement_1_1Infrastructure.html)
   * Lee
     [TaskRepository](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1Repositories_1_1TaskRepository.html)
     para ver cómo se implementan los puertos de salida
   * Lee
     [TaskDbContext](./classTaskManagement_1_1Infrastructure_1_1Persistence_1_1TaskDbContext.html)
     para ver la configuración de base de datos
   * Lee
     [InProcessDomainEventDispatcher](./classTaskManagement_1_1Infrastructure_1_1EventDispatching_1_1InProcessDomainEventDispatcher.html)
     para ver el manejo de eventos

4. Revisa la capa de interfaz API
   [TaskManagement.API](./namespaceTaskManagement_1_1API.html)
   * Lee
     [TasksController](./classTaskManagement_1_1API_1_1Controllers_1_1TasksController.html)
     para ver endpoints HTTP
   * Lee
     [CreateTaskPresenter](./classTaskManagement_1_1API_1_1Presenters_1_1CreateTaskPresenter.html)
     para ver cómo se presenta el resultado de un caso de uso
   * Lee [Program](./classProgram.html) para ver
     la configuración de inyección de dependencias
   * Lee
     [ExceptionHandlingMiddleware](./ExceptionHandlingMiddleware_8cs.html)
     para ver cómo se responde a fallas inesperadas

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



## El principio de abstracciones estables

