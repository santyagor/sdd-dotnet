# Plan de implementación: Consulta de propiedad por id

**Branch**: `007-properties-get-by-id` | **Date**: 2026-07-22 | **Spec**: [specs/007-properties-get-by-id/spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/007-properties-get-by-id/spec.md`

## Summary

Implementar un endpoint Minimal API de consulta unitaria en `GET /api/properties/{id}` que recupere una sola propiedad por id y devuelva exactamente los campos públicos usados por el listado 004, sin metadatos de paginación. La respuesta expondrá `imageUrl` como URL absoluta pública construida con el esquema y host de la request actual; cuando no exista imagen asociada, la API devolverá cadena vacía. La implementación seguirá la arquitectura Vertical Slice del backend con slice, handler, mapping y validación explícitos, sin controllers ni exposición de entidades EF.

## Technical Context

**Language/Version**: C# / .NET 11.

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- EF Core + Npgsql + PostgreSQL
- FluentValidation
- `ProblemDetails` / `ValidationProblemDetails`
- Infraestructura existente de `ISlice`, `IHandler` y mapeo de resultados

**Storage**: PostgreSQL para `Property`; imágenes públicas servidas desde `app/backend/src/RealtorApi/wwwroot/assets/properties`.

**Testing**: Validación manual documentada en `quickstart.md`; pruebas unitarias de handler, mapping y slice si la implementación lo requiere.

**Target Platform**: Backend ASP.NET Core ejecutable en Windows, Linux y macOS.

**Project Type**: Backend web service con Minimal APIs y arquitectura Vertical Slice.

**Performance Goals**: Responder la consulta unitaria con latencia p95 de 1 segundo o menos en una muestra funcional de 100 solicitudes válidas.

**Constraints**:
- No usar controllers.
- No devolver entidades EF directamente.
- Mantener `imageUrl` absoluta y pública con base en host y esquema actuales.
- No incluir paginación en esta operación.
- Mantener la lógica de consulta fuera del `Program.cs`.
- Respetar la gobernanza Spec-Driven y la compatibilidad con las specs 001-006.

**Scale/Scope**: Consulta unitaria por id, con o sin imagen persistida, sin metadatos de navegación.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La funcionalidad vive dentro de la solución única compartida.
- La implementación se modela como Vertical Slice en backend.
- No se usan controllers.
- El stack obligatorio se mantiene: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- La lógica de dominio no se mueve a UI ni a `DbContext`.
- Todo contenido markdown nuevo permanece en español.
- Se conservan las reglas de gobernanza Spec-Driven y la compatibilidad con specs previas.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Project Structure

### Documentation (this feature)

```text
specs/007-properties-get-by-id/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-get-by-id.openapi.yaml
└── spec.md
```

### Source Code (repository root)

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Features/
│   │       │   └── Properties/
│   │       │       └── GetPropertyById/
│   │       │           ├── GetPropertyByIdSlice.cs
│   │       │           ├── GetPropertyByIdHandler.cs
│   │       │           ├── GetPropertyByIdMapping.cs
│   │       │           ├── GetPropertyByIdValidator.cs
│   │       │           └── GetPropertyByIdContracts.cs
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
│               └── Features/
│                   └── Properties/
│                       └── GetPropertyById/
└── frontend/
  └── (sin cambios en esta spec)
```

**Structure Decision**: Se añade la feature en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById` y se reutiliza la infraestructura base existente (`ISlice`, `IHandler`, `ProblemDetails`, validación y persistencia). No se introducen controllers, registries manuales ni cambios frontend para esta spec.

## Phase 0: Research

La investigación ya resuelta se centra en estas decisiones concretas:

1. Reutilizar el contrato de lectura pública de la spec 004 para campos de respuesta.
2. Construir `imageUrl` como URL absoluta pública con el esquema y host de la request actual.
3. Representar la ausencia de imagen con cadena vacía para mantener el contrato explícito.
4. Tratar el id inválido como `400 Bad Request` con `ProblemDetails`.
5. Mantener la consulta como operación unitaria sin metadatos de paginación.
6. Resolver el recurso mediante Vertical Slice auto-descubierto sin controllers.

## Phase 1: Design

El diseño generará:

- `research.md` con decisiones técnicas y alternativas descartadas.
- `data-model.md` con contratos de entrada/salida y reglas de consulta.
- `contracts/backend-properties-get-by-id.openapi.yaml` con el contrato HTTP formal.
- `quickstart.md` con escenarios de validación manual y comprobación de imagen pública absoluta.

## Complexity Tracking

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
