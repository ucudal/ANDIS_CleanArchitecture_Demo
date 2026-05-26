# Universidad Católica del Uruguay

## Facultad de Ingeniería y Tecnologías

### Análisis y diseño de aplicaciones II

<!-- markdownlint-disable-next-line MD025 -->
# Demo de Clean Architecture

[![Deploy to Pages](https://github.com/ucudal/ANDIS_CleanArchitecture_Demo/actions/workflows/pages.yaml/badge.svg)](https://github.com/ucudal/ANDIS_CleanArchitecture_Demo/actions/workflows/pages.yaml)

En [Clean
Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html),
el código se organiza en círculos concéntricos, donde cada círculo representa
diferentes áreas del software[^1]. Los elementos de código de un círculo no
pueden tener referencias a elementos en un círculo exterior, o dicho de otra
forma, las dependencias pueden ir sólo de afuera hacia adentro.

[^1]: Por lo tanto, esta arquitectura hace énfasis en la **separación de
    aspectos** o, en inglés, [*separation of
    concerns*](https://en.wikipedia.org/wiki/Separation_of_concerns).

![](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg)

Este repositorio contiene una simple aplicación para gestión de tareas
desarrollada usando Clean Architecture y es una versión extendida del ejemplo de
[^2].

[^2]: Maurice, M. (2026, February 1). Clean architecture in .NET: A complete
    production-ready guide.
    [Medium](https://medium.com/@michaelmaurice410/clean-architecture-in-net-a-complete-production-ready-guide-49dcbdb22166).

> [!TIP]
> Todas las clases en esta demo están documentadas para explicar su rol en la
> arquitectura. Comienza consultando 📖 [esta
> documentación](https://ucudal.github.io/ANDIS_CleanArchitecture_Demo)
> que te ayudará a entender la arquitectura navegando por el código.
