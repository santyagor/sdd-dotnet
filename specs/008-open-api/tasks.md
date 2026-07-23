# Tareas: Documento OpenAPI v1 de RealtorApi

**Input**: Documentos de diseño desde `/specs/008-open-api/`
**Prerequisites**: plan.md (requerido), spec.md (requerido), research.md, data-model.md, contracts/, quickstart.md
**Tests**: Incluidas porque la especificación exige verificación automatizada de drift, lint externo y smoke test de NSwag.
**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [US?] Descripción con ruta exacta`

- **[P]**: Puede ejecutarse en paralelo con otras tareas sin dependencias de archivos.
- **[US]**: Historia de usuario a la que pertenece la tarea.
- Incluir rutas exactas en la descripción.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar los artefactos base de validación y el soporte de generación OpenAPI.

- [X] T001 Crear la configuración obligatoria de Redocly en .redocly.yaml con security-defined deshabilitada y operation-4xx-response como warning
- [X] T002 Crear dotnet-tools.json en la raíz con NSwag.ConsoleCore registrado para restauración mediante dotnet tool restore
- [X] T003 Actualizar app/backend/src/RealtorApi/RealtorApi.csproj para habilitar generación de documentos OpenAPI en build-time con Microsoft.Extensions.ApiDescription.Server y salida en wwwroot/openapi
- [X] T004 [P] Ajustar app/backend/src/RealtorApi/Program.cs para registrar AddOpenApi() sin exponer UI interactiva en runtime

**Checkpoint**: La base de publicación y validación del contrato queda preparada.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Infraestructura común necesaria para todas las historias del contrato OpenAPI.

- [X] T005 [P] Crear utilidades de normalización de rutas para drift en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/EndpointRouteNormalization.cs
- [X] T006 [P] Crear utilidades comunes para ejecutar comandos externos en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/ExternalToolProcessRunner.cs
- [X] T007 [P] Añadir metadatos base de OpenAPI a los endpoints compartidos en app/backend/src/RealtorApi/Features/Health/HealthSlice.cs
- [X] T008 [P] Añadir metadatos base de OpenAPI a los endpoints compartidos en app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs
- [X] T009 [P] Añadir metadatos base de OpenAPI a los endpoints compartidos en app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs
- [X] T010 [P] Añadir metadatos base de OpenAPI a los endpoints compartidos en app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs
- [X] T011 [P] Añadir metadatos base de OpenAPI a los endpoints compartidos en app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs

**Checkpoint**: La superficie HTTP queda lista para documentarse y probarse sin introducir dependencias entre historias.

---

## Phase 3: User Story 1 - Contrato público versionado (Priority: P1)

**Goal**: Publicar un único documento OpenAPI v1 generado desde el backend real y servible como archivo estático estable.

**Independent Test**: Ejecutar el backend y confirmar que GET /openapi/v1.json devuelve 200 OK con JSON que describe los 5 endpoints en alcance.

### Implementation for User Story 1

- [X] T012 [US1] Configurar la generación build-time del documento v1 en app/backend/src/RealtorApi/RealtorApi.csproj y app/backend/src/RealtorApi/Program.cs para emitir app/backend/src/RealtorApi/wwwroot/openapi/v1.json
- [X] T013 [P] [US1] Documentar en app/backend/src/RealtorApi/Features/Health/HealthSlice.cs la operación GET /health con respuestas 200 y 500
- [X] T014 [P] [US1] Documentar en app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs la operación GET /api/properties con query parameters y responses de éxito y error
- [X] T015 [P] [US1] Documentar en app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs la operación GET /api/properties/{id} con parámetros, respuestas y ProblemDetails
- [X] T016 [P] [US1] Documentar en app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs la operación POST /api/properties con request body, multipart form data y response 201
- [X] T017 [P] [US1] Documentar en app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs la operación PUT /api/properties/{id} con request body, multipart form data y responses de éxito y error
- [X] T018 [US1] Verificar que el archivo generado se publica como recurso estático estable en app/backend/src/RealtorApi/wwwroot/openapi/v1.json y responde mediante GET /openapi/v1.json

**Checkpoint**: El contrato versionado v1 queda publicado y accesible sin UI interactiva.

---

## Phase 4: User Story 2 - Validación de alineación automática (Priority: P1)

**Goal**: Detectar desalineaciones entre la implementación real y el documento OpenAPI publicado.

**Independent Test**: Introducir una divergencia entre endpoints reales y paths documentados y comprobar que la prueba de drift falla.

### Tests for User Story 2

- [X] T019 [P] [US2] Crear pruebas de drift contra EndpointDataSource en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiEndpointDriftTests.cs
- [X] T020 [P] [US2] Crear prueba de normalización de constraints de ruta en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/EndpointRouteNormalizationTests.cs
- [X] T021 [P] [US2] Crear prueba de validación Redocly invocando npx @redocly/cli lint en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiRedoclyLintTests.cs

### Implementation for User Story 2

- [X] T022 [US2] Implementar el análisis del inventario de endpoints y la comparación con openapi/v1.json en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiEndpointDriftTests.cs
- [X] T023 [US2] Implementar la invocación a Redocly con .redocly.yaml desde app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiRedoclyLintTests.cs y el helper app/backend/tests/RealtorApiTests/UnitTests/OpenApi/ExternalToolProcessRunner.cs
- [X] T024 [US2] Integrar la normalización de rutas ASP.NET Core para tratar equivalentes {id:guid} y {id} en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/EndpointRouteNormalization.cs

**Checkpoint**: Las pruebas de drift y de lint externo detectan desalineación real del contrato.

---

## Phase 5: User Story 3 - Consumo externo confiable (Priority: P2)

**Goal**: Confirmar que el documento puede consumirse por herramientas externas y que los tipos compartidos están centralizados.

**Independent Test**: Ejecutar NSwag sobre el documento generado y comprobar que produce un cliente tipado en artifacts/openapi-client-smoke/ sin correcciones manuales.

### Tests for User Story 3

- [X] T025 [P] [US3] Crear prueba de smoke test NSwag en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiNswagSmokeTests.cs
- [X] T026 [P] [US3] Crear prueba de exposición de esquemas compartidos en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiSharedSchemaTests.cs

### Implementation for User Story 3

- [X] T027 [US3] Implementar la generación del cliente tipado con NSwag contra app/backend/src/RealtorApi/wwwroot/openapi/v1.json en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiNswagSmokeTests.cs
- [X] T028 [US3] Verificar que PropertyStatus, ProblemDetails y HttpValidationProblemDetails se exponen como componentes reutilizables en el documento en app/backend/tests/RealtorApiTests/UnitTests/OpenApi/OpenApiSharedSchemaTests.cs

**Checkpoint**: El contrato queda validado como consumible por herramientas externas y con esquemas compartidos reutilizables.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cerrar el flujo reproducible y documentar la validación operativa.

- [X] T029 [P] Actualizar support/scripts/generate-openapi-v1.ps1 para ejecutar dotnet build, npx @redocly/cli lint y dotnet tool run nswag en secuencia con fallos explícitos
- [X] T030 [P] [US1] Crear la guía operativa final en specs/008-open-api/quickstart.md para prerequisitos, regeneración y validación del contrato
- [X] T031 [P] [US1] Confirmar en specs/008-open-api/contracts/backend-openapi-v1.md que el alcance, la normalización y la exclusión de UI quedan documentados
- [X] T032 Verificar que todos los tests y checks de contrato quedan descritos y alineados en specs/008-open-api/data-model.md y specs/008-open-api/research.md

**Checkpoint**: La feature queda lista para validación final y entrega.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias; puede comenzar de inmediato.
- **Foundational (Phase 2)**: Depende de Setup; bloquea todas las historias.
- **User Story 1 (Phase 3)**: Depende de Foundational.
- **User Story 2 (Phase 4)**: Depende de Foundational y del contrato base publicado.
- **User Story 3 (Phase 5)**: Depende de Foundational y del contrato base publicado.
- **Polish (Phase 6)**: Depende de las historias que se quieran cerrar.

### User Story Dependencies

- **US1 (P1)**: Base del contrato; no depende de otras historias.
- **US2 (P1)**: Puede validarse en paralelo con US1 una vez exista la superficie HTTP base.
- **US3 (P2)**: Requiere el documento publicado pero no depende de cambios funcionales adicionales.

### Within Each User Story

- Las pruebas de cada historia deben escribirse y fallar antes de implementar la lógica correspondiente.
- La documentación del contrato debe quedar alineada con las rutas reales de la feature.
- La validación funcional de una historia no debe requerir completar otras historias.

### Parallel Opportunities

- T001–T004 pueden ejecutarse en paralelo por tocar archivos distintos.
- T005–T011 pueden ejecutarse en paralelo porque cada tarea toca archivos independientes.
- T013–T017 pueden ejecutarse en paralelo por slice.
- T019–T021 pueden ejecutarse en paralelo dentro de US2.
- T025–T026 pueden ejecutarse en paralelo dentro de US3.
- T029–T031 pueden ejecutarse en paralelo al final.

---

## Parallel Example: User Story 1

```text
T013, T014, T015, T016, T017
```

## Parallel Example: User Story 2

```text
T019, T020, T021
```

## Parallel Example: User Story 3

```text
T025, T026
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Setup.
2. Completar Foundational.
3. Completar US1.
4. Validar que `GET /openapi/v1.json` responde correctamente.
5. Detenerse si el contrato no está accesible o no coincide con los endpoints en alcance.

### Incremental Delivery

1. Setup + Foundational.
2. US1: publicación del contrato.
3. US2: drift, lint externo y normalización de rutas.
4. US3: smoke test NSwag y esquemas compartidos.
5. Polish: script reproducible y documentación operativa final.

### Parallel Team Strategy

1. Un desarrollador prepara Setup.
2. Un segundo desarrollador implementa US1.
3. Un tercero implementa US2.
4. Un cuarto valida US3 y el script reproducible.

---

## Notes

- Cada tarea debe permanecer trazada a una historia o al bloque de infraestructura compartida.
- Los archivos markdown nuevos permanecen en español.
- El archivo `tasks.md` debe ser ejecutable por un LLM sin contexto adicional.
- La feature no debe introducir Swagger UI ni ReDoc UI en runtime.
