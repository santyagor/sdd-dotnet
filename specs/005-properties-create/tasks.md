# Tareas: Alta de propiedades

**Input**: Documentos de diseño desde `/specs/005-properties-create/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar la estructura de la feature, la documentación operativa y la ruta pública donde se almacenarán las imágenes.

- [X] T001 [P] Crear la estructura de feature y test para el caso de uso en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/` y `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/`.
- [X] T002 [P] Agregar las solicitudes manuales de `POST /api/properties` sin imagen, con imagen válida e inválida en `app/backend/src/RealtorApi/RealtorApi.http`.
- [X] T003 Ajustar `app/backend/src/RealtorApi/Program.cs` y `support/seed-data/seed-manifest.json` para servir `/assets/properties` desde `app/backend/src/RealtorApi/wwwroot/assets/properties` y copiar allí los activos seedados.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Definir los contratos, el mapeo base, la validación, la persistencia nullable y la base técnica que comparten todas las historias.

- [X] T004 [P] Crear los contratos compartidos del alta en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyContracts.cs` con `CreatePropertyRequest`, `CreatePropertyResponse` y los tipos auxiliares necesarios para el binding multipart.
- [X] T005 [P] Crear el mapeo base del alta en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyMapping.cs` para proyectar `Property` a respuesta y construir la referencia pública sin lógica de negocio.
- [X] T006 [P] Crear el validador del request en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyValidator.cs` con reglas para campos obligatorios, imagen PNG/JPG y tamaño máximo de 5 MB.
- [X] T007 [P] Ajustar `app/backend/src/RealtorApi/Domain/Properties/Property.cs` y `app/backend/src/RealtorApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` para permitir `ImageUrl` nula cuando no se envíe imagen.
- [X] T008 [P] Crear la migración de base de datos en `app/backend/src/RealtorApi/Migrations/` y actualizar `app/backend/src/RealtorApi/Migrations/AppDbContextModelSnapshot.cs` para volver `ImageUrl` nullable.
- [X] T009 Crear el handler del caso de uso en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` usando `AppDbContext`, `IWebHostEnvironment`, persistencia de archivo y `CancellationToken`.
- [X] T010 Crear el slice del endpoint en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs` para mapear `POST /api/properties` mediante `ISlice`, consumir `multipart/form-data` y devolver `201 Created`.

**Checkpoint**: La base técnica debe estar lista para implementar cada historia sin introducir controllers ni lógica fuera del slice.

---

## Phase 3: User Story 1 - Crear una propiedad sin imagen (Priority: P1) 🎯 MVP

**Goal**: Permitir crear una propiedad válida sin adjuntar imagen y persistir `imageUrl` en null.

**Independent Test**: Crear una propiedad con todos los campos requeridos y sin archivo; la respuesta debe ser exitosa y la base de datos debe guardar `imageUrl = null`.

### Tests for User Story 1

- [X] T011 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertyHandlerTests.cs` para validar la creación sin imagen y la persistencia de `imageUrl` nula.
- [X] T012 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertySliceTests.cs` para verificar que el endpoint se descubre como `ISlice` y responde con creación exitosa.

### Implementation for User Story 1

- [X] T013 [US1] Implementar en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` la creación de propiedades sin imagen, persistiendo `ImageUrl = null`.
- [X] T014 [US1] Implementar en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertySlice.cs` el binding multipart y la respuesta `201 Created` para el alta sin imagen.

**Checkpoint**: En este punto, el alta básica debe funcionar y ser verificable independientemente sin adjuntar archivo.

---

## Phase 4: User Story 2 - Crear una propiedad con imagen válida (Priority: P2)

**Goal**: Permitir crear una propiedad con una imagen PNG/JPG válida, guardar el archivo en la ruta pública y persistir la referencia correspondiente.

**Independent Test**: Crear una propiedad con una imagen PNG o JPG de hasta 5 MB; el archivo debe guardarse con un nombre único y la respuesta debe incluir la referencia persistida.

### Tests for User Story 2

- [X] T015 [P] [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertyImageUploadTests.cs` para validar que una imagen válida se almacena con nombre único y extensión original.
- [X] T016 [P] [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertyMappingTests.cs` para verificar la proyección de respuesta y la referencia pública/relativa de la imagen.

### Implementation for User Story 2

- [X] T017 [US2] Implementar en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` el guardado del archivo en `app/backend/src/RealtorApi/wwwroot/assets/properties` con nombre único y extensión original.
- [X] T018 [US2] Ajustar en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyMapping.cs` la proyección de salida para devolver la referencia persistida de imagen cuando exista.

**Checkpoint**: En este punto, el alta con imagen válida debe quedar lista para consumo público sin exponer rutas físicas.

---

## Phase 5: User Story 3 - Rechazar altas inválidas y archivos no permitidos (Priority: P3)

**Goal**: Rechazar solicitudes inválidas de alta y archivos no admitidos con errores claros, sin persistir datos parciales.

**Independent Test**: Enviar campos obligatorios faltantes, formato no admitido o imágenes mayores a 5 MB y verificar que la API responda con `400 Bad Request` sin crear registros.

### Tests for User Story 3

- [X] T019 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertyValidatorTests.cs` para validar campos obligatorios, formatos PNG/JPG y límite de tamaño.
- [X] T020 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/Features/Properties/CreateProperty/CreatePropertyValidationPipelineTests.cs` para verificar que la validación devuelve `ValidationProblemDetails` con HTTP 400.

### Implementation for User Story 3

- [X] T021 [US3] Conectar `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyValidator.cs` al endpoint del slice para activar la validación automática del multipart.
- [X] T022 [US3] Asegurar en `app/backend/src/RealtorApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` que una validación fallida o un error de escritura no deje persistencia parcial ni archivos huérfanos.

**Checkpoint**: En este punto, el endpoint debe rechazar entradas inválidas con contratos de error consistentes y sin efectos secundarios parciales.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Revisar documentación operativa, coherencia del contrato y ausencia de controllers.

- [X] T023 [P] Revisar y ajustar `specs/005-properties-create/quickstart.md` si algún paso de validación cambia durante la implementación.
- [X] T024 [P] Validar explícitamente que no se introdujeron controllers en `app/backend/src/RealtorApi/` y que el slice sigue siendo auto-descubierto.
- [X] T025 [P] Verificar que `specs/005-properties-create/contracts/backend-minimal-api.md` y `specs/005-properties-create/data-model.md` reflejan la forma final del payload, la respuesta y las reglas de validación.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No tiene dependencias; puede comenzar de inmediato.
- **Phase 2 Foundational**: Depende de Setup. Debe completarse antes de cualquier historia de usuario.
- **Phase 3+ User Stories**: Dependen de Foundational, pero pueden ejecutarse en paralelo una vez establecida la base.
- **Phase 6 Polish**: Depende de todas las historias completadas y verificadas.

### User Story Dependencies

- **US1**: Base del alta sin imagen; desbloquea el MVP.
- **US2**: Se apoya en la base de US1 para añadir la imagen válida.
- **US3**: Se apoya en el validator y el handler para endurecer la validación y prevenir persistencia parcial.

### Parallel Opportunities

- `T001` y `T002` pueden ejecutarse en paralelo.
- `T004`, `T005` y `T006` pueden ejecutarse en paralelo porque viven en archivos distintos.
- `T011` y `T012` pueden ejecutarse en paralelo dentro de US1.
- `T015` y `T016` pueden ejecutarse en paralelo dentro de US2.
- `T019` y `T020` pueden ejecutarse en paralelo dentro de US3.

### Orden de implementación sugerido

1. Completar Setup y Foundational.
2. Entregar US1 como MVP del alta sin imagen.
3. Añadir US2 para el upload válido y la persistencia del archivo.
4. Añadir US3 para endurecer la validación y los errores esperados.
5. Cerrar con Polish y validación final.

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1: Setup.
2. Completar Phase 2: Foundational.
3. Completar Phase 3: User Story 1.
4. Validar manualmente `POST /api/properties` sin imagen.
5. Detenerse si la creación básica, la persistencia o la respuesta no son correctas.

### Incremental Delivery

1. Entregar el alta sin imagen.
2. Añadir el upload válido y el guardado en disco.
3. Endurecer la validación de tamaño, formato y campos obligatorios.
4. Verificar la documentación operativa y la consistencia del contrato final.

### Estrategia paralela

Con más de un desarrollador:

1. Un miembro puede trabajar en el slice y el handler.
2. Otro miembro puede preparar tests de US1.
3. Otro miembro puede implementar el upload válido y los tests de US2.
4. Otro miembro puede cerrar la validación y tests de US3.

---

## Notas

- Todas las tareas siguen el formato checklist requerido.
- Las tareas marcadas con `[P]` pueden ejecutarse en paralelo solo si no comparten el mismo archivo.
- Cada historia es independiente y verificable por separado.
- No se introducen controllers ni registries manuales.