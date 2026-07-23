# Plan de implementación: Actualización de estado de propiedad

**Branch**: `009-update-status` | **Date**: 2026-07-22 | **Spec**: [spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/009-update-status/spec.md`

## Resumen

Implementar un endpoint público Minimal API en `PATCH /api/properties/{id}/status` para actualizar únicamente el estado de una propiedad existente sin modificar ningún otro dato persistido. El caso de uso seguirá el patrón Vertical Slice del proyecto con slice, handler, mapping y validación explícitos, devolverá `200 OK` con la propiedad actualizada, responderá `400 Bad Request` cuando falte `status` o el valor no sea válido y responderá `404 Not Found` si la propiedad no existe. La feature incluirá ejemplo manual en el archivo `.http`, contrato OpenAPI, guía rápida y trazabilidad completa en español.

## Contexto técnico

**Language/Version**: C# / .NET 11.

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- EF Core + Npgsql + PostgreSQL
- FluentValidation
- `ProblemDetails` / `HttpValidationProblemDetails`
- Infraestructura existente de `ISlice`, `IHandler` y mapeo de resultados
- `System.Text.Json` para el contrato JSON del body

**Storage**: PostgreSQL para la entidad `Property`; no se requieren cambios de esquema.

**Testing**: Validación manual documentada en `quickstart.md` y en `RealtorApi.http`; pruebas unitarias de slice/handler/validator/mapping si la implementación lo requiere.

**Target Platform**: Backend ASP.NET Core ejecutable en Windows, Linux y macOS.

**Project Type**: Servicio web backend con Minimal APIs y arquitectura Vertical Slice.

**Performance Goals**: Mantener una latencia funcional baja para una actualización puntual por id; la operación debe comportarse como una escritura sencilla sobre una sola entidad.

**Constraints**:
- No usar controllers.
- El endpoint debe ser público y sin autenticación.
- No aceptar imagen ni subida de archivos.
- No modificar otros campos además de `status`.
- Mantener la semántica existente de los demás endpoints.
- Documentación markdown nueva en español.

**Scale/Scope**: Un solo endpoint de actualización de estado, más documentación operativa, contrato HTTP y ejemplo manual en `.http`.

## Verificación constitucional

*Puerta: debe aprobarse antes de la investigación de la fase 0. Volver a verificar después del diseño de la fase 1.*

- La funcionalidad vive dentro de la solución única compartida.
- La implementación se modela como Vertical Slice en backend.
- No se usan controllers.
- El stack obligatorio se mantiene: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- La lógica de dominio no se mueve a UI ni a `DbContext`.
- Todo contenido markdown nuevo permanece en español.
- Se conservan las reglas de gobernanza Spec-Driven y la compatibilidad con las specs previas.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Estructura del proyecto

### Documentación (esta feature)

```text
specs/009-update-status/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-update-status.openapi.yaml
└── spec.md
```

### Código fuente (raíz del repositorio)

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Features/
│   │       │   └── Properties/
│   │       │       └── UpdatePropertyStatus/
│   │       ├── Infrastructure/
│   │       │   ├── Api/
│   │       │   ├── Handlers/
│   │       │   ├── Persistence/
│   │       │   ├── Results/
│   │       │   └── Validation/
│   │       ├── RealtorApi.http
│   │       └── Program.cs
│   └── tests/
│       └── RealtorApiTests/
│           └── UnitTests/
│               └── Features/
│                   └── Properties/
│                       └── UpdatePropertyStatus/
└── frontend/
    └── (sin cambios en esta spec)
```

**Decisión de estructura**: Se añade la feature en `app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus` y se reutiliza la infraestructura base existente (`ISlice`, `IHandler`, validación, `ProblemDetails` y persistencia). No se introducen controllers, servicios paralelos ni cambios frontend para esta spec.

## Fase 0: Investigación

La investigación ya resuelta se centra en estas decisiones concretas:

1. Exponer la actualización como `PATCH /api/properties/{id}/status` con cuerpo JSON.
2. Representar `status` como valor numérico del enum de dominio `PropertyStatus`.
3. Rechazar cualquier intento de actualizar otros campos o enviar archivos.
4. Buscar primero la propiedad por id y devolver `404` si no existe.
5. Devolver `200 OK` con la propiedad actualizada y conservar intactos `imageUrl` y el resto de campos persistidos.
6. Documentar la validación manual en `RealtorApi.http` y mantener el contrato HTTP en español.
7. Reutilizar el esquema existente sin migración nueva.

## Fase 1: Diseño

El diseño generará:

- `research.md` con decisiones técnicas y alternativas descartadas.
- `data-model.md` con contratos de request/response, reglas de validación y semántica de error.
- `contracts/backend-properties-update-status.openapi.yaml` con el contrato HTTP formal.
- `quickstart.md` con escenarios de validación manual y comprobación de actualización por estado.

## Seguimiento de complejidad

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
