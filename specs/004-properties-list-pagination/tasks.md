# Tareas: Paginación de listado de propiedades

**Input**: Documentos de diseño desde `/specs/004-properties-list-pagination/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar la estructura de la feature, el contrato manual del endpoint y la ruta pública de assets que consumirá el slice.

- [X] T001 [P] Crear la estructura de feature y test para el caso de uso en `app/backend/src/RealtorApi/Features/Properties/ListProperties/` y `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/`.
- [X] T002 [P] Agregar las pruebas manuales de `GET /api/properties` y `GET /api/properties?page=1&pageSize=6` en `app/backend/src/RealtorApi/RealtorApi.http`.
- [X] T003 Configurar `app/backend/src/RealtorApi/Program.cs` para servir `/assets/properties` desde `wwwroot/images/properties` sin romper el auto-descubrimiento de slices.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Definir los contratos compartidos del listado, el mapeo base y la infraestructura común que necesita cualquier historia para funcionar.

- [X] T004 [P] Crear los contratos compartidos del listado en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesContracts.cs` con `ListPropertiesQuery`, `PropertyListItem` y `PaginatedPropertyListResponse`.
- [X] T005 [P] Crear el mapeo base del listado en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` para proyectar `Property` a `PropertyListItem` y construir URLs públicas absolutas desde `HttpContext`.
- [X] T006 [P] Crear el validador del request en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesValidator.cs` con reglas para `page` y `pageSize` mayores que cero.
- [X] T007 Crear el handler del caso de uso en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` usando `AppDbContext`, `AsNoTracking`, orden por `Title`, paginación y `CancellationToken`.
- [X] T008 Crear el slice del endpoint en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` para mapear `GET /api/properties` mediante `ISlice` y delegar en el handler.

---

## Phase 3: User Story 1 - Listar propiedades paginadas (Priority: P1) 🎯 MVP

**Goal**: Devolver propiedades paginadas con valores por defecto y orden estable por `title` ascendente.

**Independent Test**: Ejecutar `GET /api/properties` y `GET /api/properties?page=2&pageSize=6` verificando que la página por defecto sea `1`, el tamaño por defecto sea `6`, el orden sea ascendente por `title` y los metadatos reflejen el total disponible.

### Tests for User Story 1

- [X] T009 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` para validar orden por `title`, paginación por defecto y metadatos `totalItems`/`totalPages`.
- [X] T010 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesSliceTests.cs` para verificar que el endpoint se descubre como `ISlice` y devuelve el envelope esperado.

### Implementation for User Story 1

- [X] T011 [US1] Implementar la lectura paginada en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` con `CountAsync`, `Skip`, `Take` y proyección al contrato de respuesta.
- [X] T012 [US1] Implementar el mapeo del endpoint en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` para soportar el comportamiento por defecto de `page = 1` y `pageSize = 6`.

**Checkpoint**: En este punto, el listado paginado debe funcionar con datos seedados y producir una respuesta estable sin validar todavía los parámetros inválidos.

---

## Phase 4: User Story 2 - Validar parámetros de paginación (Priority: P2)

**Goal**: Rechazar solicitudes inválidas de paginación con `400` y `ValidationProblemDetails`.

**Independent Test**: Ejecutar el endpoint con `page <= 0` o `pageSize <= 0` y verificar que la respuesta sea `400 Bad Request` con detalle de validación.

### Tests for User Story 2

- [X] T013 [P] [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesValidatorTests.cs` para validar reglas positivas de `page` y `pageSize`.
- [X] T014 [P] [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesValidationPipelineTests.cs` para verificar que el slice devuelve `ValidationProblemDetails` con HTTP 400 cuando la validación falla.

### Implementation for User Story 2

- [X] T015 [US2] Conectar `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesValidator.cs` al endpoint del slice para activar la validación automática mediante el pipeline existente.
- [X] T016 [US2] Asegurar en `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` que el `CancellationToken` se propaga y que el error de validación no ejecuta el handler.

**Checkpoint**: En este punto, el endpoint debe rechazar parámetros inválidos con un contrato de error consistente y sin afectar el flujo válido.

---

## Phase 5: User Story 3 - Exponer imágenes públicas absolutas (Priority: P3)

**Goal**: Devolver `imageUrl` absoluta y pública por cada propiedad utilizando la ruta `/assets/properties/{fileName}`.

**Independent Test**: Consultar el listado con un host local como `http://localhost:5023` y verificar que `imageUrl` cumple el patrón `http://localhost:5023/assets/properties/{fileName}` sin exponer rutas físicas.

### Tests for User Story 3

- [X] T017 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesImageUrlTests.cs` para validar la construcción de URLs absolutas públicas y la ausencia de rutas físicas.
- [X] T018 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/ListProperties/ListPropertiesMappingTests.cs` para verificar que la proyección transforma el valor persistido de imagen en la URL pública esperada.

### Implementation for User Story 3

- [X] T019 [US3] Completar `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` para construir `imageUrl` con `scheme://host/assets/properties/{fileName}`.
- [X] T020 [US3] Ajustar `app/backend/src/RealtorApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` para usar el mapeo de imagen pública al proyectar los elementos de respuesta.

**Checkpoint**: En este punto, el endpoint debe devolver imágenes listas para consumo en cliente web sin rutas internas del servidor.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Revisar documentación operativa, verificar la ausencia de controllers y cerrar inconsistencias transversales.

- [X] T021 [P] Revisar y ajustar `specs/004-properties-list-pagination/quickstart.md` si algún paso de validación cambia durante la implementación.
- [X] T022 [P] Validar explícitamente que no se introdujeron controllers en `app/backend/src/RealtorApi/` y que el slice sigue siendo auto-descubierto.
- [X] T023 [P] Verificar que `specs/004-properties-list-pagination/contracts/backend-minimal-api.md` refleja la forma final del payload y el contrato de validación.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No tiene dependencias; puede comenzar de inmediato.
- **Phase 2 Foundational**: Depende de Setup. Debe completarse antes de cualquier historia de usuario.
- **Phase 3+ User Stories**: Dependen de Foundational, pero pueden ejecutarse en paralelo una vez establecida la base.
- **Phase 6 Polish**: Depende de todas las historias completadas y verificadas.

### User Story Dependencies

- **US1**: Base del endpoint paginado; desbloquea la entrega mínima del listado.
- **US2**: Se apoya en el endpoint de US1 para rechazar solicitudes inválidas.
- **US3**: Se apoya en el mapeo y en el resultado de US1 para enriquecer la respuesta con URLs públicas absolutas.

### Parallel Opportunities

- `T001` y `T002` pueden ejecutarse en paralelo.
- `T004`, `T005` y `T006` pueden ejecutarse en paralelo porque viven en archivos distintos.
- `T009` y `T010` pueden ejecutarse en paralelo dentro de la historia US1.
- `T013` y `T014` pueden ejecutarse en paralelo dentro de la historia US2.
- `T017` y `T018` pueden ejecutarse en paralelo dentro de la historia US3.

### Orden de implementación sugerido

1. Completar Setup y Foundational.
2. Entregar US1 como MVP del listado paginado.
3. Añadir US2 para endurecer la validación de entrada.
4. Añadir US3 para completar la URL pública absoluta de la imagen.
5. Cerrar con Polish y validación final.

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1: Setup.
2. Completar Phase 2: Foundational.
3. Completar Phase 3: User Story 1.
4. Validar manualmente `GET /api/properties` con los seeds existentes.
5. Detenerse si el orden, la paginación o los metadatos no son correctos.

### Incremental Delivery

1. Entregar el listado paginado básico.
2. Añadir validación automática para parámetros inválidos.
3. Añadir la construcción de `imageUrl` absoluta pública.
4. Verificar la documentación operativa y la consistencia del contrato final.

### Estrategia paralela

Con más de un desarrollador:

1. Un miembro puede trabajar en el slice y el handler.
2. Otro miembro puede preparar tests de US1.
3. Otro miembro puede implementar validación y tests de US2.
4. Otro miembro puede cerrar el mapeo de imagen pública y tests de US3.

---

## Notas

- Todas las tareas siguen el formato checklist requerido.
- Las tareas marcadas con `[P]` pueden ejecutarse en paralelo solo si no comparten el mismo archivo.
- Cada historia es independiente y verificable por separado.
- No se introducen controllers ni registries manuales.
