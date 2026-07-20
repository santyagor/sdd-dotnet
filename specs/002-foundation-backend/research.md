# Research: Backend Foundation

## Decision

- Usar .NET 11 con ASP.NET Core Minimal APIs y arquitectura Vertical Slice para el backend.
- Implementar descubrimiento de slices mediante un contrato `ISlice` y scanning de assemblies.
- Registrar handlers marcados con `IHandler` por assembly scanning como infraestructura base.
- Usar FluentValidation para validación automática de request DTOs con una fábrica de filtros basada en convención.
- Mapear errores esperados a `ProblemDetails` de forma centralizada y usar `ILogger` para logging estructurado.
- Migrar el endpoint `/health` a una implementación de `ISlice` como sonda de infraestructura.

## Rationale

- El prompt pide una base de infraestructura que no dependa de cambios en `Program.cs` para cada endpoint nuevo, por lo que la detección por scanning es la mejor opción.
- `ISlice` es el patrón preferido en Minimal API Vertical Slice para encapsular el mapeo de endpoints por feature sin controllers.
- FluentValidation es parte del stack obligatorio de la constitución y se adapta bien a la validación de payloads de Minimal APIs.
- La ausencia de validator debe ser pass-through para no forzar implementaciones prematuras.
- Un único endpoint `/health` sirve como sonda concreta sin introducir lógica de negocio ni dependencia de dominio.

## Alternatives considered

- Registro manual de slices en `Program.cs`.
  - Rechazada porque violaría el requerimiento de adicionar endpoints sin modificar `Program.cs`.
- Uso de controllers en lugar de Minimal APIs.
  - Rechazada por la constitución y el prompt, que exigen Minimal APIs sin controllers.
- Validación explícita en cada handler.
  - Rechazada porque el requisito pide validación automática por convención, no registros individuales.
- Implementar un endpoint de negocio simple como prueba.
  - Rechazada porque la iniciativa debe evitar casos de uso de producto y solo usar `/health` como sonda.
