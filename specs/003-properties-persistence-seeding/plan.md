# Implementation Plan: Property Persistence and Seeding

**Branch**: `003-properties-persistence-seeding` | **Date**: 2026-07-20 | **Spec**: [specs/003-properties-persistence-seeding/spec.md](spec.md)

**Input**: Feature specification from `/specs/003-properties-persistence-seeding/spec.md`

**Note**: Este plan define el diseño de persistencia, migraciones y seeding para el modelo de `Property` y `PropertyStatus`.

## Summary

Implementar el modelo persistente de propiedades y estados en `RealtorApi` con EF Core y PostgreSQL, junto con un seeding idempotente basado en archivos externos (`properties.json`, `properties-statuses.json` y `seed-manifest.json`). El plan incluye la configuración de entidades vía `IEntityTypeConfiguration`, la creación de `AppDbContext`, un `DatabaseSeeder` sync/async, y la integración de assets de seed y de imágenes en el proyecto para que sean consumibles en runtime.

## Technical Context

**Language/Version**: C# / .NET 11 (definido por `global.json` del repositorio).

**Primary Dependencies**:
- ASP.NET Core Minimal APIs
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.Design
- Microsoft.Extensions.FileProviders
- xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing para pruebas

**Storage**: PostgreSQL mediante EF Core; archivos de seed y assets en `support/seed-data`.

**Testing**: xUnit + FluentAssertions para pruebas unitarias e integración ligera. Las pruebas validarán la configuración de EF, la migración generada, la idempotencia del seeding, la lectura de JSON/manifest y la disponibilidad de imágenes públicas.

**Target Platform**: ASP.NET Core backend ejecutable en .NET 11 sobre Windows, Linux y macOS.

**Project Type**: Backend web service minimal API con persistencia de datos y recursos estáticos.

**Performance Goals**: La solución debe compilar y arrancar con migraciones y seeding ejecutándose de forma determinista en el arranque. El proceso de seed debe ser idempotente y no duplicarse en reinicios.

**Constraints**:
- No usar controllers.
- No usar `HasData` para el seeding.
- No condicionar `MigrateAsync` por migraciones pendientes.
- Mantener la arquitectura de slices de backend existente evitando cambios no requeridos fuera de persistencia.
- Integrar archivos externos en el proyecto y asegurar su disponibilidad en runtime.
- Guardar `ImageUrl` como ruta pública de API, no como ruta física.

**Scale/Scope**: Persistencia básica de propiedades y estado + seeding inicial desde archivos externos. No se implementan endpoints de consulta en esta spec.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Respeta la estructura de specs en la raíz del repositorio.
- Mantiene la arquitectura backend en `app/backend/src/RealtorApi` y no crea proyectos paralelos.
- Cumple con el stack obligatorio: .NET 11, ASP.NET Core Minimal APIs, EF Core, PostgreSQL.
- No usa controllers.
- No crea entidades de dominio fuera de la carpeta `Domain/Properties` ni lógica de negocio en el espacio de infraestructura.
- Los archivos de soporte y el manifiesto central se integrarán en el proyecto según la regla de persistencia.

Resultado: ningún gate crítico de la constitución detectado. El plan es compatible con las reglas vigentes.

## Project Structure

### Documentation (this feature)

```text
specs/003-properties-persistence-seeding/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── spec.md
```

### Source Code (repository root)

```text
app/
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── Domain/
│   │       │   └── Properties/
│   │       │       ├── Property.cs
│   │       │       └── PropertyStatus.cs
│   │       ├── Features/
│   │       │   └── Health/  # existente, no se modifica fuera de persistencia
│   │       ├── Infrastructure/
│   │       │   └── Persistence/
│   │       │       ├── AppDbContext.cs
│   │       │       ├── DatabaseSeeder.cs
│   │       │       ├── MigrationExtensions.cs
│   │       │       └── Configurations/
│   │       │           └── PropertyConfiguration.cs
│   │       ├── Migrations/  # migración única generada por esta spec
│   │       ├── Program.cs
│   │       ├── RealtorApi.csproj
│   │       └── appsettings.json
│   └── tests/
│       └── RealtorApiTests/
│           └── UnitTests/  # pruebas de persistencia, migración y seeding
└── frontend/  # no cambia en esta spec

support/
└── seed-data/
    ├── properties.json
    ├── properties-statuses.json
    ├── seed-manifest.json
    └── properties/  # imágenes de seed
```

**Structure Decision**: Mantener la organización de backend actual en `app/backend/src/RealtorApi` y agregar solo el subdominio de persistencia requerido bajo `Domain/Properties` e `Infrastructure/Persistence`. El soporte de archivos externos se organiza en `support/seed-data` para separar datos de seed del código fuente.

## Complexity Tracking

No se identifican violaciones de la constitución que requieran justificación adicional para esta fase.
