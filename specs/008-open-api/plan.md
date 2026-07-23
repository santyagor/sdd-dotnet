# Plan de implementación: Documento OpenAPI v1 de RealtorApi

**Branch**: `008-open-api` | **Date**: 2026-07-22 | **Spec**: [spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/008-open-api/spec.md`

## Summary

Generar y publicar un único documento OpenAPI v1 derivado de la implementación real de `RealtorApi`, servido como archivo estático en `GET /openapi/v1.json`, y respaldado por validación reproducible: build del backend, lint con Redocly, smoke test con NSwag y pruebas xUnit de drift contra el inventario real de endpoints.

## Technical Context

**Language/Version**: C# / .NET 11.

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- `Microsoft.AspNetCore.OpenApi`
- `Microsoft.Extensions.ApiDescription.Server`
- EF Core + Npgsql + PostgreSQL
- FluentValidation
- xUnit + FluentAssertions
- `@redocly/cli` vía `npx`
- NSwag.ConsoleCore vía `dotnet tool run nswag`

**Storage**: PostgreSQL para la app; documento OpenAPI versionado en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`; artefactos de smoke test en `artifacts/openapi-client-smoke/`.

**Testing**: xUnit para verificación de drift y consumo del contrato; validación externa con Redocly y NSwag ejecutada por script reproducible.

**Target Platform**: Backend ASP.NET Core en Windows, Linux y macOS; herramientas de validación en máquina de desarrollo Windows.

**Project Type**: Servicio web backend con Minimal APIs y contrato HTTP público versionado.

**Performance Goals**: Generación y validación reproducibles en una sola pasada de desarrollo; servir el contrato estático sin coste adicional de renderizado de UI.

**Constraints**:
- No agregar Swagger UI, ReDoc UI ni superficie visual interactiva.
- No editar manualmente el JSON generado.
- Normalizar las restricciones de ruta ASP.NET Core en la verificación de drift.
- Mantener la única versión de contrato en v1.
- Exigir prerequisitos de Node.js 20+, npm, npx, `dotnet tool restore` y NSwag disponible.

**Scale/Scope**: Un solo documento OpenAPI público que cubre 5 endpoints existentes y un conjunto acotado de validaciones de contrato.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La solución sigue siendo única y compartida; no se crean soluciones paralelas.
- La feature se mantiene dentro del flujo Spec-Driven obligatorio.
- La arquitectura backend sigue siendo por features y Minimal APIs; no se introducen controllers.
- El stack obligatorio permanece intacto: .NET 11, Minimal APIs, EF Core, Npgsql, PostgreSQL, FluentValidation y ProblemDetails.
- La documentación markdown nueva permanece íntegramente en español.
- Los artefactos planificados están trazados a la feature `008-open-api`.
- La exclusión de Swagger UI/ReDoc UI respeta la constitución y la spec vigente.
- Las validaciones externas (Redocly y NSwag) se consideran tooling de desarrollo, no superficie runtime.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Project Structure

### Documentation (this feature)

```text
specs/008-open-api/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-openapi-v1.md
├── checklists/
│   └── requirements.md
└── spec.md
```

### Source Code (repository root)

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Program.cs
│   │       ├── Features/
│   │       │   ├── Health/
│   │       │   └── Properties/
│   │       │       ├── CreateProperty/
│   │       │       ├── GetPropertyById/
│   │       │       ├── ListProperties/
│   │       │       └── UpdateProperty/
│   │       └── wwwroot/
│   │           └── openapi/
│   │               └── v1.json
│   └── tests/
│       └── RealtorApiTests/
│           └── UnitTests/
│               └── OpenApi/
├── frontend/
│   └── (sin cambios para esta feature)
├── support/
│   └── scripts/
│       └── generate-openapi-v1.ps1
├── .redocly.yaml
└── dotnet-tools.json
```

**Structure Decision**: Se reutiliza la solución existente `app/backend` y se añade un flujo de contrato abierto versionado con artefactos de documentación en `specs/008-open-api/` y validación reproducible mediante `support/scripts/generate-openapi-v1.ps1`. No se introducen módulos paralelos ni un segundo backend.

## Phase 0: Research

La investigación de esta feature resuelve las decisiones críticas siguientes:

1. **Generación del documento**: usar la compatibilidad nativa de OpenAPI en ASP.NET Core con generación en build y documento nombrado `v1`.
2. **Publicación**: servir el archivo generado como recurso estático desde `wwwroot/openapi/v1.json` con `UseStaticFiles`, sin UI interactiva ni endpoint de exploración.
3. **Alineación del contrato**: añadir metadatos explícitos en las Minimal APIs para que el documento generado incluya parámetros, cuerpos, respuestas y componentes reutilizables.
4. **Drift detection**: comparar el inventario real de endpoints del runtime con los paths documentados usando normalización de constraints de ruta.
5. **Validación externa**: ejecutar Redocly con `.redocly.yaml` y NSwag como smoke test mediante el script reproducible.
6. **Tooling de desarrollo**: documentar prerequisitos Node.js/npm/npx y `dotnet tool restore` para evitar fallos sorpresivos durante validación.

## Phase 1: Design

El diseño generará los siguientes artefactos:

- `research.md` con decisiones técnicas, alternativas y justificación.
- `data-model.md` con el modelo conceptual del documento OpenAPI, inventario de endpoints y artefactos de validación.
- `contracts/backend-openapi-v1.md` con el contrato de publicación, alcance, validación y reglas de normalización.
- `quickstart.md` con pasos reproducibles para verificar prerequisitos, regenerar el documento y ejecutar la validación completa.

## Complexity Tracking

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
