# Plan de implementación: Alta de propiedades

**Branch**: `005-properties-create` | **Date**: 2026-07-20 | **Spec**: [specs/005-properties-create/spec.md](spec.md)

**Input**: Especificación de funcionalidad desde `/specs/005-properties-create/spec.md`

## Resumen

Implementar un endpoint Minimal API de alta de propiedades en `POST /api/properties` mediante un slice dedicado de Vertical Slice Architecture. El caso de uso recibirá los datos de la propiedad en `multipart/form-data`, permitirá una imagen opcional, persistirá `imageUrl` en null cuando no haya archivo y guardará la imagen válida en `app/backend/src/RealtorApi/wwwroot/assets/properties` con un nombre único y la extensión original. La respuesta devolverá una confirmación de creación y los errores de validación seguirán el pipeline estándar de `ProblemDetails`.

## Contexto técnico

**Language/Version**: C# / .NET 11.

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- EF Core + Npgsql + PostgreSQL
- FluentValidation
- ProblemDetails / ValidationProblemDetails
- Sistema existente de `ISlice` e `IHandler`
- `IFormFile` y `multipart/form-data`

**Storage**: PostgreSQL para la entidad `Property` y sistema de archivos bajo `app/backend/src/RealtorApi/wwwroot/assets/properties` para imágenes públicas.

**Testing**: Unit tests con xUnit y FluentAssertions para handler, validator, mapping, slice y validación de archivo; validación manual documentada en `RealtorApi.http`.

**Target Platform**: Backend ASP.NET Core ejecutable en Windows, Linux y macOS.

**Project Type**: Backend web service con Minimal APIs y arquitectura Vertical Slice.

**Performance Goals**: Procesar una creación con una sola escritura de base de datos y una sola escritura de archivo, evitando cargas completas en memoria cuando la imagen sea válida.

**Constraints**:
- No usar controllers.
- Mantener el endpoint público y sin autenticación.
- No introducir frameworks adicionales.
- Respetar la arquitectura de slices auto-descubiertos.
- Validar tamaño máximo de imagen de 5 MB.
- Aceptar solo imágenes PNG/JPG.
- Conservar la lógica de negocio fuera del mapping.

**Scale/Scope**: Alta de una propiedad con o sin imagen, persistencia del archivo opcional y respuesta de creación lista para consumo por el cliente.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La funcionalidad vive dentro de la solución única compartida.
- La implementación se modela como Vertical Slice en backend.
- No se usan controllers.
- El stack obligatorio se mantiene: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- La lógica de dominio no se mueve a UI ni a `DbContext`.
- Todo contenido markdown nuevo permanece en español.

Resultado: no se detectan violaciones críticas de la constitución para esta fase.

## Estructura del proyecto

### Documentación de esta funcionalidad

```text
specs/005-properties-create/
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
│   │       │       └── CreateProperty/
│   │       │           ├── CreatePropertySlice.cs
│   │       │           ├── CreatePropertyHandler.cs
│   │       │           ├── CreatePropertyMapping.cs
│   │       │           ├── CreatePropertyValidator.cs
│   │       │           └── CreatePropertyContracts.cs
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

**Structure Decision**: Se añade la feature en `app/backend/src/RealtorApi/Features/Properties/CreateProperty` y se reutiliza la infraestructura base existente. No se introducen controllers, servicios paralelos ni carpetas técnicas globales nuevas.

## Fase 0: Investigación

La investigación se centra en cinco decisiones concretas:

1. Contrato de entrada en `multipart/form-data` para mezclar campos de texto y archivo.
2. Estrategia de almacenamiento físico en `wwwroot/assets/properties` con nombre único y extensión original.
3. Validación del archivo opcional: tipo permitido y tamaño máximo de 5 MB.
4. Ajuste del modelo persistente para admitir `imageUrl` nula cuando no exista imagen.
5. Respuesta de éxito con un contrato de creación claro y consistente con el resto de la API.

## Fase 1: Diseño

El diseño generará:

- `research.md` con decisiones técnicas y alternativas descartadas.
- `data-model.md` con contratos de request/response y reglas de validación.
- `contracts/backend-minimal-api.md` con el contrato HTTP del endpoint.
- `quickstart.md` con escenarios de validación manual y verificación del almacenamiento de imagen.

## Complejidad

No existen violaciones de la constitución que requieran justificación adicional para esta funcionalidad.
