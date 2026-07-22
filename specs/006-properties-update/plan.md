# Plan de implementación: Actualización de propiedades

**Branch**: `006-properties-update` | **Date**: 2026-07-22 | **Spec**: [specs/006-properties-update/spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/006-properties-update/spec.md`

## Summary

Implementar un endpoint Minimal API de actualización de propiedades en `PUT /api/properties/{id}` mediante un slice dedicado de Vertical Slice Architecture. El caso de uso buscará primero la propiedad por id, aplicará una actualización completa de los campos editables, conservará la `imageUrl` cuando no se envíe archivo y, si llega una imagen válida, guardará el nuevo archivo en `app/backend/src/RealtorApi/wwwroot/assets/properties` con nombre interno único, persistiendo la nueva referencia pública/relativa y eliminando la imagen anterior tras una actualización exitosa. La respuesta devolverá la propiedad actualizada y los errores seguirán una semántica consistente con `ProblemDetails` y los códigos definidos por la spec.

## Technical Context

**Language/Version**: C# / .NET 11.

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- EF Core + Npgsql + PostgreSQL
- FluentValidation
- ProblemDetails / ValidationProblemDetails
- Infraestructura existente de `ISlice`, `IHandler` y `Result`
- `IFormFile` y `multipart/form-data`

**Storage**: PostgreSQL para la entidad `Property` y sistema de archivos bajo `app/backend/src/RealtorApi/wwwroot/assets/properties` para imágenes públicas.

**Performance Goals**: Procesar una actualización válida con latencia p95 de 2 segundos o menos en una muestra funcional de 100 solicitudes válidas.

**Constraints**:
- No usar controllers.
- Mantener el endpoint público y sin autenticación.
- No introducir frameworks adicionales.
- Respetar la arquitectura de slices auto-descubiertos.
- Reutilizar la convención pública de imágenes ya usada en 005.
- Mantener la actualización atómica desde el punto de vista funcional.
- No agregar migraciones porque no cambia el esquema.
- Conservar la lógica de negocio fuera del mapping.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La funcionalidad vive dentro de la solución única compartida.
- La implementación se modela como Vertical Slice en backend.
- No se usan controllers.
- El stack obligatorio se mantiene: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- La lógica de dominio no se mueve a UI ni a `DbContext`.
- La validación y los errores están alineados con los contratos del proyecto.
- Todo contenido markdown nuevo permanece en español.
- Se conservan las reglas de gobernanza Spec-Driven y la compatibilidad con las specs 001-005.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Project Structure

### Documentation (this feature)

```text
specs/006-properties-update/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
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
│   │       │       └── UpdateProperty/
│   │       │           ├── UpdatePropertySlice.cs
│   │       │           ├── UpdatePropertyHandler.cs
│   │       │           ├── UpdatePropertyMapping.cs
│   │       │           ├── UpdatePropertyValidator.cs
│   │       │           └── UpdatePropertyContracts.cs
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
│                       └── UpdateProperty/
└── frontend/
    └── (sin cambios en esta spec)
```

**Structure Decision**: Se añade la feature en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty` y se reutiliza la infraestructura base existente (`Result`, `ProblemDetails`, validación y persistencia). No se introducen controllers, servicios paralelos ni cambios frontend para esta spec.

## Phase 0: Research

La investigación ya resuelta se centra en estas decisiones concretas:

1. Contrato de actualización completa mediante `PUT /api/properties/{id}` con `multipart/form-data`.
2. Separación entre validación de negocio y validación/gestión de imagen para conservar los códigos HTTP requeridos.
3. Estrategia de almacenamiento de imagen nueva en `wwwroot/assets/properties` con nombre interno UUID + extensión.
4. Eliminación de la imagen anterior solo tras una actualización exitosa para evitar archivos huérfanos.
5. Respuesta exitosa con el recurso actualizado y `200 OK`.
6. Reutilización del esquema existente sin migración nueva.

## Phase 1: Design

El diseño generará:

- `research.md` con decisiones técnicas y alternativas descartadas.
- `data-model.md` con contratos de request/response, reglas de validación y semántica de error.
- `contracts/backend-minimal-api.md` con el contrato HTTP del endpoint.
- `quickstart.md` con escenarios de validación manual y verificación de rendimiento básico.

## Complexity Tracking

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
