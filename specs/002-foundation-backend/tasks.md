# Tasks: Backend Foundation

**Input**: Design documents from `/specs/002-foundation-backend/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Crear la estructura base del backend y el soporte de archivos necesarios para la infraestructura transversal.

- [ ] T001 [P] Create `app/backend/src/RealtorApi/Infrastructure/Api/ISlice.cs` to define the Minimal API slice contract
- [ ] T002 [P] Create `app/backend/src/RealtorApi/Infrastructure/Handlers/IHandler.cs` to define the handler marker contract
- [ ] T003 [P] Create `app/backend/src/RealtorApi/Infrastructure/Validation/ValidationFilterFactory.cs` to support automatic FluentValidation execution
- [ ] T004 [P] Create `app/backend/src/RealtorApi/Infrastructure/Validation/FluentValidationServiceCollectionExtensions.cs` to register validators by assembly scanning
- [ ] T005 [P] Create `app/backend/src/RealtorApi/Infrastructure/Results/ResultProblemDetailsMapper.cs` to centralize mapping of Result errors to ProblemDetails
- [ ] T006 [P] Create `app/backend/tests/RealtorApiTests/Infrastructure/` and `app/backend/tests/RealtorApiTests/UnitTests/` folders for infrastructure-focused test artifacts

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement core backend infrastructure that debe completarse antes de comenzar con las historias de usuario.

- [ ] T007 Implement `app/backend/src/RealtorApi/Infrastructure/Api/SliceRegistrationExtensions.cs` to discover and register all `ISlice` implementations from loaded assemblies
- [ ] T008 Implement `app/backend/src/RealtorApi/Infrastructure/Api/SliceMappingExtensions.cs` to map endpoints for each discovered `ISlice` in a centralized call
- [ ] T009 Implement `app/backend/src/RealtorApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs` to auto-register `IHandler` implementations by assembly scanning
- [ ] T010 Implement `app/backend/src/RealtorApi/Infrastructure/Validation/FluentValidationRegistrationExtensions.cs` to discover `IValidator<T>` implementations and wire them into the pipeline
- [ ] T011 Implement `app/backend/src/RealtorApi/Infrastructure/Validation/ValidationFilterFactory.cs` so missing validators pass through and failures return `ValidationProblemDetails` with HTTP 400
- [ ] T012 Implement `app/backend/src/RealtorApi/Program.cs` changes to configure logging, register infrastructure services, and map slices through the central discovery mechanism without manual slice registration
- [ ] T013 Implement `app/backend/src/RealtorApi/Infrastructure/Results/ResultProblemDetailsMapper.cs` to translate expected `Result` errors into structured `ProblemDetails` responses and to log errors with `ILogger`

---

## Phase 3: User Story 1 - Registrar slices automáticamente (Priority: P1)

**Goal**: Permitir que una implementación de `ISlice` se descubra y se registre sin modificar `Program.cs`.

**Independent Test**: Agregar una clase `ISlice` de prueba en el proyecto de tests, arrancar la aplicación y verificar que el endpoint se registra una sola vez.

- [ ] T014 [US1] Create `app/backend/tests/RealtorApiTests/Infrastructure/TestSlices/TestHealthSlice.cs` in the test project as a proof-of-concept `ISlice`
- [ ] T015 [US1] Create `app/backend/tests/RealtorApiTests/UnitTests/RegisterSlicesTests.cs` to verify `RegisterSlices` discovers the test `ISlice` and does not register duplicates
- [ ] T016 [US1] Create `app/backend/src/RealtorApi/Features/Health/HealthSlice.cs` to expose `/health` as a production `ISlice` endpoint
- [ ] T017 [US1] Create `app/backend/tests/RealtorApiTests/UnitTests/HealthSliceTests.cs` to verify the `/health` `ISlice` is mapped and responds successfully

---

## Phase 4: User Story 2 - Validación automática de requests (Priority: P2)

**Goal**: Validar automáticamente request bodies cuando exista un `IValidator<T>`, y continuar sin error cuando no exista.

**Independent Test**: Exponer un endpoint de prueba en el proyecto de tests y verificar que una request inválida devuelve `400 ValidationProblemDetails` y que una request válida continúa.

- [ ] T018 [US2] Create `app/backend/tests/RealtorApiTests/Infrastructure/TestSlices/ValidationTestSlice.cs` with a request-body endpoint for test validation
- [ ] T019 [US2] Create `app/backend/tests/RealtorApiTests/Infrastructure/TestValidators/ValidationRequestValidator.cs` to validate the test request type
- [ ] T020 [US2] Create `app/backend/tests/RealtorApiTests/UnitTests/ValidationPipelineTests.cs` to verify valid requests pass and invalid requests return `ValidationProblemDetails` with HTTP 400
- [ ] T021 [US2] Extend `app/backend/tests/RealtorApiTests/UnitTests/ValidationPipelineTests.cs` to verify the same endpoint passes through successfully when no validator exists for the request type

---

## Phase 5: User Story 3 - Manejo consistente de errores esperados (Priority: P3)

**Goal**: Convertir errores esperados en `ProblemDetails` uniformes y registrar los eventos de error de forma estructurada.

**Independent Test**: Simular un error esperado en la infraestructura de prueba y verificar el código de estado y el payload de `ProblemDetails`.

- [ ] T022 [US3] Create `app/backend/tests/RealtorApiTests/Infrastructure/TestSlices/ErrorResultTestSlice.cs` exposing a test endpoint that returns an expected `Result` error
- [ ] T023 [US3] Create `app/backend/tests/RealtorApiTests/UnitTests/ResultProblemDetailsMappingTests.cs` to verify expected `Result` errors map to the correct HTTP status and `ProblemDetails` payload
- [ ] T024 [US3] Ensure `app/backend/src/RealtorApi/Infrastructure/Results/ResultProblemDetailsMapper.cs` logs structured error details using `ILogger`

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Asegurar que la infraestructura transversal esté documentada y que los tests de apoyo sean sólidos.

- [ ] T025 [P] Update `specs/002-foundation-backend/quickstart.md` with exact validation steps for the implemented infrastructure
- [ ] T026 [P] Review `app/backend/src/RealtorApi/Program.cs` and add comments explaining the central slice discovery, validation registration, and error mapping flow
- [ ] T027 [P] Review and refine `app/backend/tests/RealtorApiTests/UnitTests/*Tests.cs` to ensure the names and assertions clearly reflect the infrastructure intent

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup** can start immediately and focuses on file-level structure and contracts
- **Phase 2: Foundational** depends on Setup completion and blocks all user stories until finished
- **Phase 3+: User Stories** depend on Foundational completion, but the implementation and test definitions within each story can proceed after Phase 2
- **Phase 6: Polish** depends on the infrastructure and story tests being in place

### Story Dependencies

- **US1**: Requires foundational slice discovery and mapping support before the `ISlice` test and `/health` endpoint can be verified
- **US2**: Requires foundational validation registration and filter support before test endpoint validation can be verified
- **US3**: Requires foundational error mapping support before expected `Result` error behavior can be verified

### Parallel Opportunities

- Setup tasks `T001` through `T006` are marked `[P]` and can run in parallel because they create independent files and folders
- Foundational registration and mapping implementations `T007` through `T011` can be implemented in parallel if they are kept in separate files, but `T012` depends on those infrastructure pieces being available
- The user story test files in distinct folders can be created in parallel once foundational infrastructure exists
- Polish tasks `T025` through `T027` are `[P]` and can be executed after implementation to finalize documentation and comments

## Implementation Strategy

### MVP First

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: US1 and verify `/health` registration
4. Validate the core slice discovery mechanism independently

### Incremental Delivery

1. With foundational infrastructure in place, implement US1 and verify `ISlice` registration
2. Add US2 validation pipeline verification and verify invalid requests return `400`
3. Add US3 error mapping verification and verify `ProblemDetails` responses for expected errors
4. Finish with polish and documentation updates

### Parallel Execution Example

- Developer A: `T001`, `T002`, `T003`, `T005`
- Developer B: `T007`, `T008`, `T009`, `T010`
- Developer C: `T014`, `T015`, `T018`, `T022`

> Note: `T012` must wait for the core infrastructure extension classes to exist before updating `Program.cs`.
