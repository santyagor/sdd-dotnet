# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]

**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command; its definition describes the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: [e.g., Python 3.11, Swift 5.9, Rust 1.75 or NEEDS CLARIFICATION]

**Primary Dependencies**: [e.g., FastAPI, UIKit, LLVM or NEEDS CLARIFICATION]

**Storage**: [if applicable, e.g., PostgreSQL, CoreData, files or N/A]

**Testing**: [e.g., pytest, XCTest, cargo test or NEEDS CLARIFICATION]

**Target Platform**: [e.g., Linux server, iOS 15+, WASM or NEEDS CLARIFICATION]

**Project Type**: [e.g., library/cli/web-service/mobile-app/compiler/desktop-app or NEEDS CLARIFICATION]

**Performance Goals**: [domain-specific, e.g., 1000 req/s, 10k lines/sec, 60 fps or NEEDS CLARIFICATION]

**Constraints**: [domain-specific, e.g., <200ms p95, <100MB memory, offline-capable or NEEDS CLARIFICATION]

**Scale/Scope**: [domain-specific, e.g., 10k users, 1M LOC, 50 screens or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

[Gates determined based on constitution file]

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: Single project (DEFAULT)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
# Plan de implementación: Paginación de listado de propiedades

**Branch**: `004-properties-list-pagination` | **Date**: 2026-07-20 | **Spec**: [specs/004-properties-list-pagination/spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/004-properties-list-pagination/spec.md`

## Resumen

Implementar un endpoint Minimal API basado en `ISlice` para listar propiedades de forma paginada en `/api/properties`, ordenadas por `title` ascendente, con validación automática de `page` y `pageSize`, y con una respuesta que proyecte `imageUrl` como URL pública absoluta bajo `/assets/properties/{fileName}`. La lógica vivirá en un slice de feature dedicado, utilizando proyección EF Core, `AsNoTracking`, y contratos explícitos de request/response sin exponer entidades de persistencia.

## Contexto técnico

**Language/Version**: C# / .NET 11 (definido por `global.json`).

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- Patrón `ISlice` con auto-descubrimiento
- FluentValidation
- `ValidationFilterFactory` + endpoint filters
- EF Core + Npgsql + PostgreSQL
- `ProblemDetails` / `ValidationProblemDetails`
- xUnit + FluentAssertions para pruebas unitarias

**Storage**: PostgreSQL vía EF Core; lectura desde el modelo persistente de `Property` ya existente.

**Testing**: Pruebas unitarias de slice, handler, mapeo, validador y contrato de URL pública; validación manual mediante archivo `.http`.

**Target Platform**: Backend ASP.NET Core ejecutable en Windows, Linux y macOS.

**Project Type**: Backend web service con Minimal APIs y arquitectura Vertical Slice.

**Performance Goals**: Responder el listado paginado con una sola consulta de conteo y una proyección paginada eficiente; mantener el endpoint delgado y sin sobrecarga innecesaria.

**Constraints**:
- No usar controllers.
- No devolver entidades EF directamente.
- No introducir validación manual en el endpoint.
- No romper el descubrimiento centralizado de slices.
- Construir `imageUrl` absoluta con el host de la solicitud actual.
- Mantener el alcance limitado al listado paginado.
- Usar solo pruebas unitarias; no introducir integración pesada sin justificación explícita.

**Scale/Scope**: Listado paginado de propiedades con metadatos de navegación y proyección de una URL pública por elemento.

## Constitution Check

*GATE: Debe pasar antes de la investigación de Phase 0. Revalidar después del diseño de Phase 1.*

- La funcionalidad vive dentro de la solución única compartida y no crea proyectos paralelos.
- La implementación se modela como Vertical Slice en `Features/Properties/ListProperties`.
- No se usan controllers.
- El backend sigue el stack obligatorio: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- La lógica de dominio no se mueve a UI ni a `DbContext`.
- La documentación nueva permanece en español.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Estructura del proyecto

### Documentación de esta funcionalidad

```text
specs/004-properties-list-pagination/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── spec.md
```

### Código fuente relevante

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Features/
│   │       │   └── Properties/
│   │       │       └── ListProperties/
│   │       │           ├── ListPropertiesSlice.cs
│   │       │           ├── ListPropertiesHandler.cs
│   │       │           ├── ListPropertiesMapping.cs
│   │       │           └── ListPropertiesValidator.cs
│   │       ├── Infrastructure/
│   │       │   ├── Api/
│   │       │   ├── Handlers/
│   │       │   ├── Persistence/
│   │       │   ├── Results/
│   │       │   └── Validation/
│   │       └── Program.cs
│   └── tests/
│       └── RealtorApiTests/
│           └── UnitTests/
└── frontend/
    └── (sin cambios en esta spec)
```

**Structure Decision**: Se añade la feature en `app/backend/src/RealtorApi/Features/Properties/ListProperties` y se reutiliza la infraestructura base existente (`Infrastructure/Api`, `Infrastructure/Handlers`, `Infrastructure/Validation`, `Infrastructure/Results`). No se introducen controllers ni registries manuales.

## Fase 0: Investigación

La investigación se centra en cuatro decisiones concretas:

1. Contrato del slice y mapeo automático bajo `ISlice`.
2. Validación automática de query parameters con FluentValidation.
3. Proyección paginada con `AsNoTracking`, `CountAsync` y `ToListAsync`.
4. Construcción de la URL pública absoluta de la imagen con base en el request actual y una ruta pública `/assets/properties`.

## Fase 1: Diseño

El diseño generará:

- `research.md` con decisiones técnicas y alternativas descartadas.
- `data-model.md` con contratos de request/response y metadatos de paginación.
- `contracts/backend-minimal-api.md` con el contrato HTTP del endpoint.
- `quickstart.md` con escenarios de validación manual y verificación de la URL pública.

## Complejidad

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
