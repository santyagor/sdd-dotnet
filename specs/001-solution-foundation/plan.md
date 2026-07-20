# Implementation Plan: Fundación de Solución Realtor

**Branch**: `001-solution-foundation` | **Date**: 2026-07-20 | **Spec**: [specs/001-solution-foundation/spec.md](spec.md)

**Input**: Feature specification describing project structure, project organization and initial tooling configuration

**Note**: This plan captures the research and technical blueprint for foundation work; implementation follows in `/speckit.tasks` and execution via `/speckit.implement`.

## Summary

Crear la estructura base de la solución Realtor con dos aplicaciones (.NET 11): backend ASP.NET Core Minimal API en arquitecura Vertical Slice y frontend Blazor Web App. Ambas incluyen proyectos de tests con framework xUnit. La solución compila exitosamente sin lógica de negocio ni features implementadas.

## Technical Context

**Language/Version**: C# / .NET 11.0.100-preview.6.26359.118 (from global.json at repository root)

**Primary Dependencies**:
- ASP.NET Core minimal hosting model
- EF Core (setup deferred to subsequent feature)
- Blazor Web App (interactive server-side)
- xUnit + FluentAssertions (testing)
- Refit (typed HTTP clients, integration deferred)
- FluentValidation (setup deferred)
- Serilog (structured logging, setup deferred)

**Storage**: PostgreSQL via Npgsql EF Core provider (configuration deferred to data feature)

**Testing**: xUnit for backend unit/integration tests; MSTest or similar for Blazor component testing

**Target Platform**: .NET (Windows, macOS, Linux)

**Project Type**: Multi-project web solution (backend API + frontend web app)

**Performance Goals**: `dotnet build` completes in < 2 minutes on standard dev machine

**Constraints**:
- ASP.NET Core Minimal APIs only (no MVC controllers)
- Vertical Slice architecture for backend (no global service folders)
- Blazor Web App standard structure (no custom scaffolding)
- CSS in `wwwroot/app.css` only (no framework CSS)
- .NET SDK from global.json is source of truth

**Scale/Scope**: Single solution, 2 applications (backend + frontend), 4 projects (2 src + 2 test)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Compliance Status**: PASS ✅

| Principle | Requirement | Compliance | Notes |
|-----------|-------------|-----------|-------|
| I. Solución Única | Non-divided, monolithic evolution | ✅ Pass | Single Realtor.sln contains backend, frontend, tests |
| II. Spec-Driven | Specs source of truth for all work | ✅ Pass | This plan driven by spec.md and .github/prompts/001-foundation-solution.prompt.md |
| III. Arquitectura Canónica | Backend Vertical Slice; Frontend Blazor | ✅ Pass | Structure designed for VSA; Blazor Web App standard layout |
| IV. Stack Tecnológico | .NET 11, ASP.NET Core, Blazor, EF Core, Npgsql, PostgreSQL, Refit, FluentValidation, ProblemDetails, CSS, Lucide | ✅ Pass | All tech choices align; persistence layers deferred appropriately |
| V. Calidad de Dominio | Business logic separate from UI/endpoints/DbContext | ⏳ Deferred | Foundation establishes structure; domain model integrity verified in subsequent features |

**Non-Violations**: No breaking changes to existing architecture (no existing architecture yet).

## Project Structure

### Documentation (this feature)

```text
specs/001-solution-foundation/
├── spec.md              # This specification
├── plan.md              # This file
└── tasks.md             # Created by /speckit.tasks (implementation checklist)
```

### Source Code (repository app/ subdirectory)

```text
app/
├── Realtor.sln                          # Solution file containing all projects
├── .gitignore                           # Excludes bin/, obj/, .vs/, etc.
├── backend/
│   ├── src/
│   │   └── RealtorApi/
│   │       ├── RealtorApi.csproj        # ASP.NET Core minimal hosting
│   │       ├── Program.cs               # Configuration base: services, middleware, endpoint mapping
│   │       ├── appsettings.json         # Base configuration (no secrets)
│   │       ├── appsettings.Development.json
│   │       ├── Properties/
│   │       │   └── launchSettings.json
│   │       └── Features/                # Vertical slice structure (ready for features)
│   │           └── .gitkeep
│   └── tests/
│       └── RealtorApiTests/
│           ├── RealtorApiTests.csproj   # xUnit + FluentAssertions
│           ├── Usings.cs                # Global using statements
│           ├── UnitTests/
│           │   └── .gitkeep
│           └── IntegrationTests/
│               └── .gitkeep
├── frontend/
│   ├── src/
│   │   └── RealtorWeb/
│   │       ├── RealtorWeb.csproj        # Blazor Web App
│   │       ├── Program.cs               # Configuration base: services, routing, interactivity
│   │       ├── appsettings.json
│   │       ├── appsettings.Development.json
│   │       ├── Components/
│   │       │   ├── App.razor            # Root component
│   │       │   └── Layout/
│   │       │       └── MainLayout.razor
│   │       ├── Pages/
│   │       │   └── Home.razor
│   │       ├── Shared/
│   │       │   └── .gitkeep
│   │       └── wwwroot/
│   │           ├── app.css              # All custom CSS here (no framework CSS)
│   │           └── app.js               # Minimal JS interop if needed
│   └── test/
│       └── RealtorWeb/
│           ├── RealtorWeb.Tests.csproj  # Blazor component testing
│           ├── Usings.cs
│           └── Components/
│               └── .gitkeep
└── global.json                          # Reference only; DO NOT MODIFY
```

**Structure Decision**: 
Two-tier structure: `backend/` (ASP.NET Core src + tests) and `frontend/` (Blazor src + tests) under `app/` root. Each tier has `src/` and `tests/` subdirectories for clarity. This mirrors the constitutional requirement that both layers belong to one solution while keeping concerns separate.

Backend uses Vertical Slice structure (Features/ folder ready for slices); frontend uses standard Blazor components + pages + shared layout structure.

Solution file aggregates both tiers and enables single `dotnet build` / `dotnet test` execution from `app/` directory.

## Complexity Tracking

No notable complexity violations. This is a pure foundation task following the constitution exactly:
- Single solution consolidates both tiers
- No controversial architectural decisions
- Stack is declared and fixed in constitution
- All complexity is deferred to feature-driven work

| Item | Status | Notes |
|------|--------|-------|
| Scope creep risk | Low | Feature explicitly prohibits business logic and features |
| Dependency risk | Low | All dependencies declared in constitution; versions locked in global.json |
| Integration risk | Low | Backend and frontend are separate projects; integration via typed Refit clients deferred |
| DevEx risk | Low | Standard .NET tooling and Blazor scaffolding; minimal custom infrastructure |

## Next Phase

Upon completion of `/speckit.implement`, the solution will:
1. Compile successfully with `dotnet build` from `app/`
2. Discover and run test placeholders with `dotnet test` from `app/`
3. Be ready to receive first feature initiative (e.g., 002-user-authentication or domain feature)
4. Maintain structure consistency for all subsequent features
