# Tareas: Consulta de propiedad por id

**Input**: Documentos de diseño desde `/specs/007-properties-get-by-id/`

**Prerequisites**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Tests**: No se incluyen tareas de pruebas automatizadas porque la spec no las solicita de forma explícita. La validación se cubre con `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo con otras tareas en archivos distintos
- **[Story]**: Historia de usuario a la que pertenece la tarea, por ejemplo `US1`, `US2`, `US3`
- Incluir rutas exactas de archivo en la descripción

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar la estructura de la feature y los ejemplos manuales de validación.

- [X] T001 [P] Crear la estructura de feature y test para `GetPropertyById` en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/` y `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/GetPropertyById/`.
- [X] T002 [P] Agregar escenarios manuales de `GET /api/properties/{id}` en `app/backend/src/RealtorApi/RealtorApi.http` para consulta exitosa, imagen ausente, id inexistente e id inválido.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Definir los contratos, el mapeo, la validación y la base técnica común que todas las historias necesitan.

**⚠️ CRITICAL**: No debe comenzar ninguna historia de usuario hasta completar esta fase.

- [X] T003 [P] Crear los contratos compartidos en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdContracts.cs` con `GetPropertyByIdRequest` y `PropertyDetailResponse` para el binding y la respuesta pública.
- [X] T004 [P] Crear el mapeo base en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` para proyectar `Property` a `PropertyDetailResponse` y construir `imageUrl` absoluta pública con esquema y host actuales.
- [X] T005 [P] Crear el validador del request en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdValidator.cs` para rechazar ids vacíos o con formato inválido con `400`.
- [X] T006 Crear el handler del caso de uso en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs` con búsqueda por id, `AppDbContext`, `HttpContext` y `CancellationToken`.
- [X] T007 Crear el slice del endpoint en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs` para exponer `GET /api/properties/{id}` mediante `ISlice` y devolver `200 OK` o `ProblemDetails`.

**Checkpoint**: La base técnica debe estar lista para implementar cada historia sin introducir controllers ni lógica fuera del slice.

---

## Phase 3: User Story 1 - Consultar una propiedad existente (Priority: P1) 🎯 MVP

**Goal**: Permitir consultar una propiedad existente por id y devolver el contrato público de detalle con `imageUrl` absoluta cuando exista imagen.

**Independent Test**: Consultar `GET /api/properties/{id}` con un id existente y comprobar que devuelve los campos públicos esperados con una `imageUrl` absoluta pública o una cadena vacía si no hay imagen.

### Implementation for User Story 1

- [X] T008 [US1] Completar en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs` la recuperación exitosa de la propiedad existente y la devolución del contrato de detalle público.
- [X] T009 [P] [US1] Ajustar en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` la proyección de salida para devolver los campos públicos de la propiedad y construir `imageUrl` absoluta pública según la request actual.

**Checkpoint**: En este punto, la consulta exitosa de una propiedad existente debe funcionar y ser verificable independientemente.

---

## Phase 4: User Story 2 - Recibir error cuando la propiedad no existe (Priority: P2)

**Goal**: Responder con `404 Not Found` cuando el id consultado no corresponda a ninguna propiedad persistida.

**Independent Test**: Consultar el endpoint con un id válido que no exista en la base de datos y confirmar una respuesta `404` con `ProblemDetails`.

### Implementation for User Story 2

- [X] T010 [US2] Implementar en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs` la rama `404 Not Found` con `ProblemDetails` cuando no exista la propiedad consultada.

**Checkpoint**: En este punto, la ausencia de un recurso debe resolverse con el contrato de error esperado.

---

## Phase 5: User Story 3 - Rechazar ids inválidos de forma consistente (Priority: P3)

**Goal**: Rechazar ids con formato inválido como error de validación pública para evitar confundir formato incorrecto con recurso inexistente.

**Independent Test**: Enviar un valor de id con formato inválido y confirmar la respuesta definida por el contrato público del proyecto.

### Implementation for User Story 3

- [X] T011 [US3] Ajustar en `app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs` el mapeo de errores de validación para traducir un id inválido en `400 Bad Request` con `ProblemDetails` sin duplicar la responsabilidad del validador.

**Checkpoint**: En este punto, el endpoint debe distinguir correctamente un id mal formado de un recurso ausente.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Revisar documentación operativa, coherencia del contrato y ausencia de controllers.

- [X] T012 [P] Validar y ajustar `specs/007-properties-get-by-id/quickstart.md` con pasos finales para id existente, sin imagen, inexistente e inválido.
- [X] T013 [P] Confirmar en `specs/007-properties-get-by-id/contracts/backend-properties-get-by-id.openapi.yaml` y `specs/007-properties-get-by-id/data-model.md` que el contrato final, la forma de `imageUrl` y los códigos de error coinciden con la implementación.
- [X] T014 [P] Verificar en `app/backend/src/RealtorApi/` que no se introdujeron controllers y que `GetPropertyByIdSlice` queda auto-descubierto mediante `ISlice`.
- [X] T015 [P] Agregar en `specs/007-properties-get-by-id/quickstart.md` una comprobación manual de rendimiento para 100 consultas válidas y registrar el criterio de éxito de `SC-005`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias; puede comenzar de inmediato.
- **Foundational (Phase 2)**: Depende de Setup y bloquea todas las historias de usuario.
- **User Stories (Phase 3+)**: Dependen de Foundational. Se ejecutan en orden de prioridad o en paralelo si el equipo lo permite.
- **Polish (Final Phase)**: Depende de la finalización y verificación de las historias necesarias.

### User Story Dependencies

- **User Story 1 (P1)**: Base del flujo de consulta unitaria; desbloquea el MVP.
- **User Story 2 (P2)**: Se apoya en el flujo base para manejar el no encontrado con `404`.
- **User Story 3 (P3)**: Endurece la validación de id sobre la misma feature y depende de la base técnica ya establecida.

### Within Each User Story

- Completar el comportamiento base antes de endurecer errores o ramas alternativas.
- Mantener la lógica de negocio en el handler y la proyección de respuesta en el mapping.
- Validar que cada historia siga siendo verificable de forma independiente.

### Parallel Opportunities

- `T001` y `T002` pueden ejecutarse en paralelo porque viven en archivos distintos.
- `T003`, `T004` y `T005` pueden ejecutarse en paralelo una vez creada la estructura base.
- `T009` puede ejecutarse en paralelo con `T008` porque modifica un archivo distinto.
- `T012`, `T013`, `T014` y `T015` pueden ejecutarse en paralelo porque tocan archivos distintos.

---

## Parallel Example: User Story 1

```text
Task: "Completar en app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs la recuperación exitosa de la propiedad existente y la devolución del contrato de detalle público."
Task: "Ajustar en app/backend/src/RealtorApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs la proyección de salida para devolver los campos públicos de la propiedad y construir imageUrl absoluta pública según la request actual."
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1: Setup.
2. Completar Phase 2: Foundational.
3. Completar Phase 3: User Story 1.
4. Validar manualmente `GET /api/properties/{id}` con una propiedad existente.
5. Detenerse si la consulta básica, la proyección o la URL pública no son correctas.

### Incremental Delivery

1. Entregar la consulta unitaria exitosa.
2. Añadir la rama `404 Not Found` para recursos ausentes.
3. Endurecer la validación de ids inválidos.
4. Verificar la documentación operativa y la consistencia del contrato final.

### Estrategia paralela

Con más de un desarrollador:

1. Un miembro puede trabajar en contratos, mapping y validator.
2. Otro miembro puede implementar el handler base y el slice.
3. Otro miembro puede cerrar la rama de `404` y la validación del id.
4. Otro miembro puede ajustar la documentación y la verificación manual.

---

## Notas

- Todas las tareas siguen el formato checklist requerido.
- Las tareas marcadas con `[P]` pueden ejecutarse en paralelo solo si no comparten el mismo archivo.
- Cada historia es independiente y verificable por separado.
- No se introducen controllers ni registries manuales.
