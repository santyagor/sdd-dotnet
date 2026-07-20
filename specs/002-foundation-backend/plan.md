# Implementation Plan: Backend Foundation

**Branch**: `002-foundation-backend` | **Date**: 2026-07-20 | **Spec**: [specs/002-foundation-backend/spec.md](spec.md)

**Input**: Feature specification from `/specs/002-foundation-backend/spec.md`

**Note**: Este plan define la infraestructura transversal del backend RealtorApi y los artefactos de diseño necesarios para implementar la base de la solución.

## Summary

Establecer la infraestructura base del backend `RealtorApi` con arquitectura Vertical Slice y Minimal APIs. Se implementará un mecanismo de descubrimiento y mapeo centralizado de slices mediante `ISlice`, un pipeline de validación automática con FluentValidation, un contrato de registro de handlers `IHandler` y un mapeo uniforme de errores esperados a `ProblemDetails`. El único endpoint efectivo migrado como sonda es `/health`.

## Technical Context

**Language/Version**: C# / .NET 11 (definido por `global.json` en la raíz del repositorio)

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- FluentValidation
- Microsoft.Extensions.Logging
- FluentAssertions, xUnit, Microsoft.AspNetCore.Mvc.Testing para pruebas backend

**Storage**: N/A (no se crea base de datos ni EF Core en esta fase)

**Testing**: xUnit + FluentAssertions + Microsoft.AspNetCore.Mvc.Testing para pruebas de API y validación.

**Target Platform**: ASP.NET Core backend ejecutable en .NET 11 sobre Windows, Linux y macOS.

**Project Type**: Backend web service minimal API con infraestructura transversal de API y validación.

**Performance Goals**: `dotnet build app/` debe compilar sin errores; la infraestructura debe permitir extender slices sin modificar `Program.cs`.

**Constraints**:
- No generar dominios, entidades, `AppDbContext`, migraciones ni conexión a base de datos.
- No usar controllers.
- `Program.cs` solo configura servicios, middleware y delega a mapeo centralizado.
- El endpoint `/health` es únicamente una sonda de infraestructura.
- Clases de prueba `ISlice` y validators deben vivir en el proyecto de tests.

**Scale/Scope**: Definir la base del backend y la infraestructura transversal sin implementar lógica de negocio de producto.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Todas las puertas de gobernanza de la constitución aplicables a esta fase pasan. No se identifican violaciones de arquitectura, stack o scope.

## Project Structure

### Documentation (this feature)

```text
specs/002-foundation-backend/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-minimal-api.md
└── tasks.md
```

### Source Code (repository root)

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Program.cs
│   │       ├── RealtorApi.csproj
│   │       ├── Features/
│   │       │   └── .gitkeep
│   │       ├── appsettings.json
│   │       └── appsettings.Development.json
│   └── tests/
│       └── RealtorApiTests/
│           ├── RealtorApiTests.csproj
│           ├── UnitTest1.cs
│           ├── Usings.cs
│           └── UnitTests/
└── frontend/
    ├── src/
    │   └── RealtorWeb/
    └── test/
        └── RealtorWeb/
```

**Structure Decision**: Seleccionar la estructura `backend/src` y `backend/tests` para el backend, manteniendo el frontend existente en `app/frontend`. El backend se organiza por slices y depende de la detección de `ISlice` para mapear endpoints automáticamente.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
