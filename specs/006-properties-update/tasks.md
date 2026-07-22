# Tareas: Actualización de propiedades

**Input**: Documentos de diseño desde `/specs/006-properties-update/`

**Prerequisites**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `contracts/`

**Tests**: No se incluyen tareas de pruebas automatizadas porque la spec no las solicita de forma explícita. La validación se cubre con los criterios independientes de cada historia y con `quickstart.md`.

**Organización**: Las tareas se agrupan por historia de usuario para permitir implementación y validación independientes.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo con otras tareas en archivos distintos
- **[Story]**: Historia de usuario a la que pertenece la tarea, por ejemplo `US1`, `US2`, `US3`
- Incluir rutas exactas de archivo en la descripción

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar la estructura de la feature y los ejemplos manuales de validación.

- [X] T001 [P] Crear la estructura de feature y test para `UpdateProperty` en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/` y `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/UpdateProperty/`.
- [X] T002 [P] Agregar escenarios manuales de `PUT /api/properties/{id}` en `app/backend/src/RealtorApi/RealtorApi.http` para actualización sin imagen, con imagen válida y rechazos `404`, `415` y `413`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Definir los contratos, el mapeo, la validación y la base técnica común que todas las historias necesitan.

**⚠️ CRITICAL**: No debe comenzar ninguna historia de usuario hasta completar esta fase.

- [X] T003 [P] Crear los contratos compartidos de actualización en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyContracts.cs` con `UpdatePropertyRequest`, `UpdatePropertyResponse` y los tipos auxiliares necesarios para el binding multipart.
- [X] T004 [P] Crear el mapeo base en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs` para proyectar `Property` a respuesta y construir la referencia pública de imagen sin lógica de negocio.
- [X] T005 [P] Crear el validador del request en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyValidator.cs` con reglas para campos obligatorios, `multipart/form-data`, imagen PNG/JPG y tamaño máximo de 5 MB.
- [X] T006 [P] Crear el handler del caso de uso en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` con búsqueda por id, base de persistencia y actualización completa de la entidad.
- [X] T007 [P] Crear el slice del endpoint en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs` para exponer `PUT /api/properties/{id}` mediante `ISlice`, consumir `multipart/form-data` y devolver `200 OK`.

**Checkpoint**: La base técnica debe estar lista para implementar cada historia sin introducir controllers ni lógica fuera del slice.

---

## Phase 3: User Story 1 - Actualizar una propiedad sin imagen nueva (Priority: P1) 🎯 MVP

**Goal**: Permitir actualizar una propiedad existente sin adjuntar una imagen nueva y conservar la `imageUrl` previa.

**Independent Test**: Actualizar una propiedad existente con campos válidos y sin archivo; la respuesta debe ser exitosa, los cambios de negocio deben persistirse y la `imageUrl` debe permanecer sin cambios.

### Implementation for User Story 1

- [X] T008 [US1] Completar en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` la ruta de actualización sin imagen, conservando `ImageUrl` previa y actualizando `UpdatedAt`.
- [X] T009 [P] [US1] Ajustar en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs` la proyección de salida para reflejar exactamente el estado persistido cuando el request no incluye archivo.

**Checkpoint**: En este punto, la actualización básica sin imagen debe funcionar y ser verificable independientemente.

---

## Phase 4: User Story 2 - Actualizar una propiedad con imagen válida (Priority: P2)

**Goal**: Permitir actualizar una propiedad existente con una nueva imagen válida, guardar el archivo en la ruta pública y persistir la nueva referencia.

**Independent Test**: Actualizar una propiedad existente con una imagen PNG o JPG válida de hasta 5 MB; el archivo debe guardarse con un nombre único y la respuesta debe incluir la nueva referencia persistida.

### Implementation for User Story 2

- [X] T010 [US2] Implementar en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` el almacenamiento de la nueva imagen válida en `app/backend/src/RealtorApi/wwwroot/assets/properties` con nombre interno UUID + extensión.
- [X] T011 [US2] Implementar en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` la eliminación de la imagen previa después de persistir con éxito la nueva referencia pública/relativa.

**Checkpoint**: En este punto, la actualización con imagen válida debe quedar lista para consumo público sin exponer rutas físicas.

---

## Phase 5: User Story 3 - Rechazar actualizaciones inválidas y errores de almacenamiento (Priority: P3)

**Goal**: Rechazar solicitudes inválidas, propiedades inexistentes e incidencias de almacenamiento con errores claros y sin persistencia parcial.

**Independent Test**: Enviar un id inexistente, campos inválidos, imágenes vacías o no permitidas, imágenes mayores a 5 MB y fallos de almacenamiento; el sistema debe responder con el código esperado y sin cambios parciales.

### Implementation for User Story 3

- [X] T012 [US3] Endurecer `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyValidator.cs` para rechazar campos de negocio inválidos y archivos vacíos o con formato no permitido con `400` y `415` según contrato.
- [X] T013 [US3] Completar en `app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` la resolución de `404 Not Found`, `413 Payload Too Large` y `500 Internal Server Error` sin dejar cambios parciales ni archivos huérfanos.

**Checkpoint**: En este punto, el endpoint debe rechazar entradas inválidas con contratos de error consistentes y sin efectos secundarios parciales.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Revisar documentación operativa, coherencia del contrato y ausencia de controllers.

- [X] T014 [P] Validar y ajustar `specs/006-properties-update/quickstart.md` con los pasos finales de verificación manual para actualización sin imagen, con imagen, errores y la comprobación de latencia p95 para 100 solicitudes válidas.
- [X] T015 [P] Confirmar en `specs/006-properties-update/contracts/backend-minimal-api.md` y `specs/006-properties-update/data-model.md` que el contrato final, la respuesta y los códigos de error coinciden con la implementación.
- [X] T016 [P] Verificar en `app/backend/src/RealtorApi/` que no se introdujeron controllers y que el slice `UpdatePropertySlice` queda auto-descubierto mediante `ISlice`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias; puede comenzar de inmediato.
- **Foundational (Phase 2)**: Depende de Setup y bloquea todas las historias de usuario.
- **User Stories (Phase 3+)**: Dependen de Foundational. Se ejecutan en orden de prioridad o en paralelo si el equipo lo permite.
- **Polish (Final Phase)**: Depende de la finalización y verificación de las historias necesarias.

### User Story Dependencies

- **User Story 1 (P1)**: Base del flujo de actualización sin imagen; desbloquea el MVP.
- **User Story 2 (P2)**: Se apoya en el flujo base de actualización para añadir el reemplazo de imagen.
- **User Story 3 (P3)**: Endurece el comportamiento de errores sobre la misma feature y depende de la base técnica ya establecida.

### Within Each User Story

- Completar el comportamiento base antes de endurecer errores o reemplazo de imagen.
- Mantener la lógica de negocio en el handler y la proyección de respuesta en el mapping.
- Validar que cada historia siga siendo verificable de forma independiente.

### Parallel Opportunities

- `T001` y `T002` pueden ejecutarse en paralelo porque viven en archivos distintos.
- `T003`, `T004`, `T005`, `T006` y `T007` pueden ejecutarse en paralelo una vez creada la estructura base.
- `T009` puede ejecutarse en paralelo con `T008` porque modifica un archivo distinto.
- `T014`, `T015` y `T016` pueden ejecutarse en paralelo porque tocan archivos distintos.

---

## Parallel Example: User Story 1

```text
Task: "Completar en app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs la ruta de actualización sin imagen, conservando ImageUrl previa y actualizando UpdatedAt."
Task: "Ajustar en app/backend/src/RealtorApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs la proyección de salida para reflejar exactamente el estado persistido cuando el request no incluye archivo."
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1: Setup.
2. Completar Phase 2: Foundational.
3. Completar Phase 3: User Story 1.
4. Validar manualmente `PUT /api/properties/{id}` sin imagen.
5. Detenerse si la actualización básica, la persistencia o la respuesta no son correctas.

### Incremental Delivery

1. Entregar la actualización sin imagen.
2. Añadir el upload válido y el reemplazo de imagen.
3. Endurecer la validación de tamaño, formato y campos obligatorios.
4. Verificar la documentación operativa y la consistencia del contrato final.

### Estrategia paralela

Con más de un desarrollador:

1. Un miembro puede trabajar en contratos, mapping y validator.
2. Otro miembro puede implementar el handler base y el slice.
3. Otro miembro puede añadir el guardado de imagen y la limpieza de archivos antiguos.
4. Otro miembro puede cerrar la validación documental y la verificación manual.

---

## Notas

- Todas las tareas siguen el formato checklist requerido.
- Las tareas marcadas con `[P]` pueden ejecutarse en paralelo solo si no comparten el mismo archivo.
- Cada historia es independiente y verificable por separado.
- No se introducen controllers ni registries manuales.
