# Tareas: Actualización de estado de propiedad

**Input**: Documentos de diseño desde `/specs/009-update-status/`
**Prerequisites**: plan.md (requerido), spec.md (requerido), research.md, data-model.md, contracts/, quickstart.md
**Tests**: No se incluyen porque la especificación no los solicita de forma explícita.
**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story?] Descripción con ruta exacta` 

- **[P]**: Puede ejecutarse en paralelo con otras tareas sin dependencias de archivos.
- **[Story]**: Historia de usuario a la que pertenece la tarea.
- Incluir rutas exactas en las descripciones.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar la estructura base del nuevo caso de uso y su contrato HTTP.

- [X] T001 [P] Crear los contratos de request y response para la actualización de estado en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusContracts.cs
- [X] T002 [P] Crear el slice del endpoint PATCH y su metadato OpenAPI en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusSlice.cs

**Checkpoint**: La superficie pública del endpoint queda declarada y lista para completar.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Construir las piezas comunes que soportan todas las historias del caso de uso.

- [X] T003 [P] Implementar la validación del cuerpo JSON de actualización de estado en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusValidator.cs
- [X] T004 [P] Implementar el mapeo de la entidad Property a la respuesta pública en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusMapping.cs
- [X] T005 [P] Añadir el ejemplo manual de PATCH en app/backend/src/RealtorApi/RealtorApi.http para validar la actualización de estado desde el editor HTTP
- [X] T006 Implementar el handler base que busca la propiedad por id y prepara la respuesta de error 404 en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandler.cs

**Checkpoint**: La feature ya puede resolver la entidad objetivo, validar la entrada y exponer una forma de prueba manual.

---

## Phase 3: User Story 1 - Actualizar el estado de una propiedad existente (Priority: P1)

**Goal**: Permitir cambiar únicamente el estado de una propiedad existente sin alterar los demás datos persistidos.

**Independent Test**: Ejecutar el ejemplo manual exitoso desde `RealtorApi.http` y verificar que la respuesta devuelve `200 OK` con la propiedad actualizada.

### Implementation for User Story 1

- [X] T007 [US1] Completar la ruta feliz del handler para persistir solo `Status` y devolver la propiedad actualizada en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandler.cs
- [X] T008 [P] [US1] Ajustar el slice para devolver `200 OK` con la respuesta mapeada en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusSlice.cs

**Checkpoint**: El endpoint actualiza el estado y devuelve la propiedad completa con los datos persistidos intactos.

---

## Phase 4: User Story 2 - Rechazar solicitudes sin estado válido (Priority: P2)

**Goal**: Rechazar cuerpos inválidos o incompletos antes de aplicar cualquier cambio.

**Independent Test**: Ejecutar el ejemplo manual sin `status` o con un valor inválido y verificar que la API responde `400 Bad Request`.

### Implementation for User Story 2

- [X] T009 [P] [US2] Endurecer la validación para rechazar `status` omitido o no admitido en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusValidator.cs
- [X] T010 [US2] Mantener el contrato de entrada restringido únicamente al campo `status` y sin soporte para archivos en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusContracts.cs y app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusSlice.cs

**Checkpoint**: Las solicitudes inválidas se rechazan con `400 Bad Request` y no llegan a persistir cambios.

---

## Phase 5: User Story 3 - Rechazar actualizaciones de propiedades inexistentes (Priority: P3)

**Goal**: Responder `404 Not Found` cuando el id no corresponde a una propiedad persistida.

**Independent Test**: Ejecutar el ejemplo manual con un id inexistente y comprobar que la API responde `404 Not Found`.

### Implementation for User Story 3

- [X] T011 [US3] Mantener la rama de no encontrado en el handler para devolver `404 Not Found` sin efectos secundarios en app/backend/src/RealtorApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandler.cs
- [X] T012 [P] [US3] Documentar la verificación manual de ids inexistentes y la preservación de la propiedad actualizada en app/backend/src/RealtorApi/RealtorApi.http

**Checkpoint**: El endpoint distingue correctamente entre una propiedad existente y una inexistente.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Cerrar la feature y alinear la documentación operativa con la implementación.

- [X] T013 [P] Revisar la guía rápida en specs/009-update-status/quickstart.md y el contrato en specs/009-update-status/contracts/backend-properties-update-status.openapi.yaml para asegurar que reflejan la implementación final

**Checkpoint**: La documentación operativa y el contrato HTTP quedan alineados con el comportamiento real.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias; puede comenzar de inmediato.
- **Foundational (Phase 2)**: Depende de Setup; bloquea todas las historias.
- **User Story 1 (Phase 3)**: Depende de Foundational.
- **User Story 2 (Phase 4)**: Depende de Foundational y del contrato base del endpoint.
- **User Story 3 (Phase 5)**: Depende de Foundational y de la lógica base del handler.
- **Polish (Phase 6)**: Depende de la implementación de las historias.

### User Story Dependencies

- **US1 (P1)**: Historia base; no depende de otras historias.
- **US2 (P2)**: Puede validarse de forma independiente una vez exista el contrato de entrada.
- **US3 (P3)**: Depende del handler base, pero su validación sigue siendo independiente.

### Within Each User Story

- La estructura de entrada debe existir antes de completar el handler.
- La rama de no encontrado debe quedar resuelta antes de cerrar la historia de actualización exitosa.
- La documentación manual debe reflejar el comportamiento final del endpoint.

### Parallel Opportunities

- T001–T005 pueden ejecutarse en paralelo porque tocan archivos distintos.
- T003–T005 pueden ejecutarse en paralelo dentro de la infraestructura común.
- T007 y T008 pueden ejecutarse en paralelo una vez exista la base del handler.
- T009 y T010 pueden ejecutarse en paralelo dentro de US2.
- T012 y T013 pueden ejecutarse en paralelo al final.

---

## Parallel Example: User Story 1

```text
T007, T008
```

## Parallel Example: User Story 2

```text
T009, T010
```

## Parallel Example: User Story 3

```text
T011, T012
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Setup.
2. Completar Foundational.
3. Completar US1.
4. Validar manualmente la actualización exitosa desde `RealtorApi.http`.
5. Detenerse si la respuesta no devuelve la propiedad actualizada completa.

### Incremental Delivery

1. Setup + Foundational.
2. US1: actualización exitosa del estado.
3. US2: rechazo de cuerpos inválidos.
4. US3: rechazo de ids inexistentes.
5. Polish: revisar documentación y contrato.

### Parallel Team Strategy

1. Una persona prepara Setup.
2. Otra persona implementa la infraestructura común.
3. Una tercera persona completa US1.
4. Una cuarta persona cierra US2 y US3 con la documentación manual.

---

## Notes

- [P] tareas = archivos distintos, sin dependencias directas.
- [Story] etiqueta la historia de usuario para trazabilidad.
- La feature debe permanecer alineada con la constitución y con el estilo de las specs previas.
- No se incluyen pruebas porque esta spec no las solicita de forma explícita.
