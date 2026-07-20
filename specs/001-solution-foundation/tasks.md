# Tasks: Fundación de Solución Realtor

**Input**: Design documents from `specs/001-solution-foundation/spec.md`, `plan.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Prerequisites**: plan.md (required), spec.md (required for user stories)

**Tests**: Placeholder tests are included to satisfy build requirements for this foundation phase; the actual feature tests are added en iniciativas posteriores.

**Organization**: Tasks grouped by user story and foundation phases to support independent implementation and verification.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Crear la estructura base de la solución y preparar el espacio de trabajo.

- [X] T001 [P] Create `app/backend/src/`, `app/backend/tests/`, `app/frontend/src/`, `app/frontend/test/` directories
- [X] T002 [P] Create `app/.gitignore` with standard .NET exclusions: `bin/`, `obj/`, `.vs/`, `*.user`, `launchSettings.json`
- [X] T003 Create empty `app/Realtor.sln`

**Checkpoint**: La estructura física de la solución existe y el control de versiones ignora artefactos locales.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implementar los proyectos fuente y los proyectos de prueba básicos que permiten compilar y preparar la solución.

### Backend foundation

- [X] T004 Create `app/backend/src/RealtorApi/` as an ASP.NET Core Minimal API project targeting net11.0
  - Command: `dotnet new webapi --minimal -n RealtorApi -o app/backend/src/RealtorApi`
  - Remove any generated `Controllers/` folder
- [X] T005 [P] Create `app/backend/src/RealtorApi/Features/` with `.gitkeep`
- [X] T006 [P] Create `app/backend/src/RealtorApi/appsettings.json` with base logging and `AllowedHosts`
- [X] T007 [P] Create `app/backend/src/RealtorApi/appsettings.Development.json`

### Frontend foundation

- [X] T008 Create `app/frontend/src/RealtorWeb/` as a Blazor Web App project
  - Command: `dotnet new blazor -n RealtorWeb -o app/frontend/src/RealtorWeb`
  - Verify standard folders: `Components/`, `Pages/`, `Shared/`, `wwwroot/`
- [X] T009 [P] Verify `app/frontend/src/RealtorWeb/wwwroot/app.css` exists and is the single custom CSS entry point
- [X] T010 [P] Verify `app/frontend/src/RealtorWeb/Components/App.razor` and `Layout/` exist

### Test project foundation

- [X] T011 Create `app/backend/tests/RealtorApiTests/` as an xUnit project
  - Command: `dotnet new xunit -n RealtorApiTests -o app/backend/tests/RealtorApiTests`
- [X] T012 Create `app/frontend/test/RealtorWeb/` as an xUnit project
  - Command: `dotnet new xunit -n RealtorWeb.Tests -o app/frontend/test/RealtorWeb`
- [X] T013 [P] Add `FluentAssertions` and `Microsoft.AspNetCore.Mvc.Testing` package references to `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
- [X] T014 [P] Add `FluentAssertions` and `bunit` package references to `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj`
- [X] T015 [P] Add project reference from `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` to `app/backend/src/RealtorApi/RealtorApi.csproj`
- [X] T016 [P] Add project reference from `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj` to `app/frontend/src/RealtorWeb/RealtorWeb.csproj`
- [X] T017 [P] Create `app/backend/tests/RealtorApiTests/Usings.cs` with global using directives
- [X] T018 [P] Create `app/frontend/test/RealtorWeb/Usings.cs` with global using directives

### Solution integration

- [X] T019 Add `app/backend/src/RealtorApi/RealtorApi.csproj` to `app/Realtor.sln`
- [X] T020 [P] Add `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` to `app/Realtor.sln`
- [X] T021 Add `app/frontend/src/RealtorWeb/RealtorWeb.csproj` to `app/Realtor.sln`
- [X] T022 [P] Add `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj` to `app/Realtor.sln`

**Checkpoint**: Todos los proyectos fuente y de prueba existen y están referenciados desde `app/Realtor.sln`.

---

## Phase 3 - User Story 1: Developer configura la solución base (Prioridad: P1)

**Goal**: Garantizar que la solución completa compila y que los proyectos básicos arrancan sin error.

**Independent Test**: `dotnet build app/` se completa con éxito sin errores y `dotnet test app/` descubre proyectos de prueba placeholder.

- [X] T023 [US1] Configure `app/backend/src/RealtorApi/Program.cs` with minimal hosting, placeholder service registration, middleware, y un endpoint `GET /` de salud
- [X] T024 [US1] Configure `app/frontend/src/RealtorWeb/Program.cs` con los servicios Blazor necesarios, `HttpClient` registrado y la configuración de routing estándar
- [X] T025 [US1] Create `app/backend/tests/RealtorApiTests/UnitTests/ProgramTests.cs` placeholder test that builds `WebApplication`
- [X] T026 [US1] Create `app/frontend/test/RealtorWeb/Components/AppTests.cs` placeholder test that renders `App`
- [X] T027 [US1] Execute `dotnet build app/` from repository root and verify successful compilation
- [X] T028 [US1] Execute `dotnet test app/` from repository root and verify placeholder tests pass

**Checkpoint**: La solución base puede compilarse y ejecutarse en modo de prueba sin lógica de negocio.

---

## Phase 4 - User Story 2: Developer entiende la estructura de la solución (Prioridad: P2)

**Goal**: Documentar y validar la organización de carpetas para que cualquier developer comprenda la solución.

**Independent Test**: La estructura de carpetas respeta `app/backend/src/`, `app/backend/tests/`, `app/frontend/src/`, `app/frontend/test/` y el README explica el layout.

- [X] T029 [US2] Verify `app/backend/src/RealtorApi/` uses `Features/` and contains no `Controllers/` folder
- [X] T030 [US2] Verify `app/frontend/src/RealtorWeb/` includes `Components/`, `Pages/`, `Shared/` and `wwwroot/app.css`
- [X] T031 [US2] Create or update `app/README.md` with solution overview, folder structure, build and test commands, y referencias a `specs/001-solution-foundation/`
- [X] T032 [US2] Create or update `specs/001-solution-foundation/quickstart.md` with validation steps for build and test
- [X] T033 [US2] Verify `app/backend/src/RealtorApi/Properties/launchSettings.json` exists for local debugging

**Checkpoint**: La estructura es transparente y la documentación guía a un nuevo developer.

---

## Phase 5 - User Story 3: Tests de backend y frontend se ejecutan aisladamente (Prioridad: P3)

**Goal**: Garantizar que los proyectos de prueba están separados, configurados y se ejecutan de forma independiente.

**Independent Test**: `dotnet test` descubre los proyectos `RealtorApiTests` y `RealtorWeb.Tests`, y cada uno puede ejecutarse por separado.

- [X] T034 [US3] Verify `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` references `app/backend/src/RealtorApi/RealtorApi.csproj`
- [X] T035 [US3] Verify `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj` references `app/frontend/src/RealtorWeb/RealtorWeb.csproj`
- [X] T036 [US3] Verify required packages are present in `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` and `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj`
- [X] T037 [US3] Execute `dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj` and verify it passes
- [X] T038 [US3] Execute `dotnet test app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj` and verify it passes
- [X] T039 [US3] Execute `dotnet test app/` and verify both test projects are discovered

**Checkpoint**: Los proyectos de prueba se ejecutan aisladamente y la solución los descubre correctamente.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Revisar los artefactos de entrega y confirmar que la solución cumple las reglas de la constitución.

- [X] T040 [P] Verify `app/Realtor.sln` contains exactly 4 projects: `RealtorApi`, `RealtorApiTests`, `RealtorWeb`, `RealtorWeb.Tests`
- [X] T041 [P] Verify no `Controllers/` folder exists in `app/backend/src/RealtorApi/`
- [X] T042 [P] Verify no global orchestration folders exist in backend beyond `Features/`
- [X] T043 [P] Verify `app/frontend/src/RealtorWeb/wwwroot/app.css` exists and no CSS frameworks are imported
- [X] T044 [P] Verify `app/backend/src/RealtorApi/Program.cs` and `app/frontend/src/RealtorWeb/Program.cs` contain only base configuration and no business logic
- [X] T045 [P] Verify `global.json` was not modified by this initiative
- [X] T046 [P] Run `dotnet build -c Release app/` and verify release build passes
- [X] T047 [P] Run `dotnet format` if configured and confirm formatting consistency
- [X] T048 [P] Verify `app/README.md` and `specs/001-solution-foundation/quickstart.md` explain how to build and test the solution

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion and blocks user stories
- **User Stories (Phase 3-5)**: Depend on Foundational phase completion
- **Polish (Phase 6)**: Depends on all user story phases

### User Story Dependencies

- **US1**: Requires Phase 2 completion and is the MVP delivery for compile+health behavior
- **US2**: Requires Phase 2 completion and focuses on structure and documentation
- **US3**: Requires Phase 2 completion and focuses on test project isolation

### Parallel Opportunities

- All Phase 1 tasks marked [P] can run in parallel
- Phase 2 backend and frontend preparation can proceed in parallel after Phase 1
- Package and reference updates in test projects can run in parallel
- Story-specific verification tasks can run in parallel after implementation tasks complete
- Cross-cutting reviews in Phase 6 are mostly parallelizable

### Recommended Sequence

1. Complete Phase 1
2. Complete Phase 2 (backend, frontend y proyectos de prueba)
3. Complete US1, US2 y US3 en paralelo según capacidad
4. Finish Phase 6 review and documentation
5. Commit once all verification tasks pass

---

## Implementation Strategy

- MVP first: asegurar que `dotnet build app/` y `dotnet test app/` funcionan
- Incremental delivery: cada historia añade valor y puede verificarse independientemente
- Evitar lógica de negocio en esta fase: el focus es estructura, compilación y pruebas placeholder
- Respetar la constitución: Minimal API sin `Controllers/`, Blazor estándar, .NET 11 de `global.json`

---

## Validation Checklist (Post-Implementation)

- [X] `app/Realtor.sln` contiene exactamente 4 proyectos
- [X] Backend estructura: `app/backend/src/RealtorApi` y `app/backend/tests/RealtorApiTests`
- [X] Frontend estructura: `app/frontend/src/RealtorWeb` y `app/frontend/test/RealtorWeb`
- [X] No existe carpeta `Controllers/` en `RealtorApi`
- [X] No global service folders in RealtorApi (ready for Vertical Slice)
- [X] Frontend has Components/, Pages/, Shared/, wwwroot/ standard structure
- [X] `dotnet build` from `app/` completes in < 2 minutes
- [X] `dotnet test` from `app/` discovers all tests and passes
- [X] global.json unchanged (source of truth for .NET version)
- [X] Files don't contain business logic or feature implementations
- [X] README documents project structure and quick start

---

## Success Criteria Mapping

Each success criterion from spec.md is covered by corresponding tasks:

| SC | Description | Verified By |
|----|-------------|------------|
| SC-001 | `dotnet build` < 2 mins | T029 |
| SC-002 | `dotnet test` passes all projects | T030 |
| SC-003 | Clear structure respecting conventions | T031, T032, T033, T037-T040 |
| SC-004 | DevX: developer understands structure in < 10 mins | T033 (README) |
| SC-005 | Solution ready for next feature | T029-T032, T036 |
| SC-006 | Documentation exists | T033 |
