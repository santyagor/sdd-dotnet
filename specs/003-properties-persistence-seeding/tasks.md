# Tasks: Property Persistence and Seeding

**Input**: Design documents from `/specs/003-properties-persistence-seeding/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Crear la estructura de persistencia, los archivos de seed externos y la integración de assets en el proyecto.

- [X] T001 [P] Crear las carpetas de persistencia y dominio de la feature: `app/backend/src/RealtorApi/Domain/Properties` y `app/backend/src/RealtorApi/Infrastructure/Persistence/Configurations`.
- [X] T002 [P] Crear el directorio de soporte de seed `support/seed-data` con los archivos `properties.json`, `properties-statuses.json`, `seed-manifest.json` y la carpeta de imágenes `support/seed-data/properties/`.
- [X] T003 [P] Configurar `app/backend/src/RealtorApi/RealtorApi.csproj` para incluir los JSON de seed, el manifiesto y las imágenes bajo `support/seed-data` como contenido runtime/publish.
- [X] T004 [P] Actualizar `app/backend/src/RealtorApi/Program.cs` para registrar servicios de EF Core, servir archivos estáticos desde `wwwroot/images/properties`, y preparar el arranque de migraciones y seeding.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implementar el modelo EF Core, la configuración de entidad, el DbContext, la migración y el seeding idempotente. Esta fase debe completarse antes de las historias de usuario.

- [X] T005 Crear `app/backend/src/RealtorApi/Domain/Properties/PropertyStatus.cs` con los valores `Available`, `Rented` y `Maintenance`.
- [X] T006 Crear `app/backend/src/RealtorApi/Domain/Properties/Property.cs` con las propiedades `Id`, `Title`, `Description`, `Address`, `Price`, `Status`, `BedroomCount`, `BathroomCount`, `AreaSquareMeters`, `ImageUrl`, `CreatedAt` y `UpdatedAt`.
- [X] T007 Crear `app/backend/src/RealtorApi/Infrastructure/Persistence/AppDbContext.cs` con `DbSet<Property> Properties`, configuración desde el ensamblado y métodos `UseSeeding` / `UseAsyncSeeding`.
- [X] T008 Crear `app/backend/src/RealtorApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` y configurar la tabla `Properties` con longitudes, precisión `18,2`, conversiones de enum-string, valores requeridos y restricciones.
- [X] T009 Crear `app/backend/src/RealtorApi/Infrastructure/Persistence/DatabaseSeeder.cs` con métodos `Seed` y `SeedAsync`, lectura de `properties.json`, `properties-statuses.json` y `seed-manifest.json`, y lógica idempotente para insertar o actualizar propiedades.
- [X] T010 Crear `app/backend/src/RealtorApi/Infrastructure/Persistence/MigrationExtensions.cs` con extensiones para `MigrateAsync` y `UseAsyncSeeding`, y asegurarse de que `Program.cs` invoque el flujo de migración + seeding antes de `app.Run()`.
- [X] T011 Generar una única migración EF Core en `app/backend/src/RealtorApi/Migrations/` que represente el esquema completo de `Property` y revisar `Up`, `Down` y el snapshot.

---

## Phase 3: User Story 1 - Inicializar catálogo de propiedades con datos de support (Priority: P1)

**Goal**: Construir y validar el seeding idempotente desde archivos externos y el manifiesto, incluyendo copia/sincronización de imágenes a la ubicación pública.

**Independent Test**: Ejecutar las pruebas de seeding y startup para verificar que `MigrateAsync` y `UseAsyncSeeding` se ejecutan, los datos se insertan desde JSON/manifest y no se duplican en reinicios.

- [X] T012 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/PropertyMappingTests.cs` para validar la configuración EF de `Property` y la conversión de `PropertyStatus` a string.
- [X] T013 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/DatabaseSeederTests.cs` para validar que `DatabaseSeeder` lee `properties.json`, `properties-statuses.json` y `seed-manifest.json` desde runtime y realiza el seed idempotente.
- [X] T014 [P] [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/StartupSeedingTests.cs` para validar que el arranque de la aplicación ejecuta `MigrateAsync` y `UseAsyncSeeding` antes de `app.Run()`.
- [X] T015 [US1] Crear `app/backend/tests/RealtorApiTests/UnitTests/PropertyImageUrlTests.cs` para validar que las imágenes se copian a `wwwroot/images/properties` y que `Property.ImageUrl` contiene la ruta pública final consumible por la API.

---

## Phase 4: User Story 2 - Consultar propiedades con imagen pública (Priority: P2)

**Goal**: Validar que los datos seedados incluyen `ImageUrl` como una ruta pública de API y no como ruta física de soporte.

**Independent Test**: Ejecutar pruebas que validen `ImageUrl` persistido y la localización de imágenes públicas en `wwwroot/images/properties`.

- [X] T016 [P] [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/ImageUrlPublicPathTests.cs` para validar que el `ImageUrl` de `Property` usa una ruta pública, no una ruta de archivos del sistema.
- [X] T017 [US2] Crear `app/backend/tests/RealtorApiTests/UnitTests/PropertyImageHostingTests.cs` para validar la disponibilidad de las imágenes del seed en la ubicación pública final.

---

## Phase 5: User Story 3 - Definir y persistir estados de propiedad como enum string (Priority: P3)

**Goal**: Validar que `PropertyStatus` se persiste como texto y se reconstruye correctamente desde la base de datos.

**Independent Test**: Ejecutar pruebas que verifiquen el tipo de columna del esquema y la lectura del enum desde datos persistidos.

- [X] T018 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/PropertyStatusEnumPersistenceTests.cs` para validar la lectura/escritura del enum `PropertyStatus` desde la base de datos.
- [X] T019 [P] [US3] Crear `app/backend/tests/RealtorApiTests/UnitTests/MigrationSchemaTests.cs` para validar que la migración generada define la columna de estado como string.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Asegurar la calidad final, la documentación y la consistencia entre los artefactos de diseño y la implementación.

- [X] T020 [P] Actualizar `specs/003-properties-persistence-seeding/quickstart.md` con los pasos de validación finales si hay cambios en la implementación.
- [X] T021 [P] Revisar y ajustar `specs/003-properties-persistence-seeding/data-model.md` si la implementación final modifica el modelo de `Property` o `PropertyStatus`.
- [X] T022 [P] Verificar que `support/seed-data/seed-manifest.json` describe correctamente las rutas de origen y destino de las imágenes.
- [X] T024 [P] Validar explícitamente que no se han introducido controllers en el proyecto (revisión de código y pruebas de integración mínimas si aplica).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencias, puede comenzar inmediatamente.
- **Phase 2 Foundational**: Depende de Setup. No se debe iniciar US1/US2/US3 antes de completar esta fase.
- **Phase 3+ User Stories**: Dependen de Foundational, pero pueden ejecutarse en paralelo una vez la infraestructura esté lista.
- **Phase 6 Polish**: Depende de todas historias completadas y de la implementación final.

### User Story Dependencies

- **US1**: Primero validar el seeding y la ejecución de migración/startup.
- **US2**: Validar después de que el seeding y la generación del `ImageUrl` público estén implementados.
- **US3**: Validar después de que el modelo y la migración de `PropertyStatus` estén implementados.

### Parallel Opportunities

- Las tareas de creación de estructura y configuración de archivos (`T001`, `T002`, `T003`) son paralelizables.
- Las tareas de definición de entidades y persistencia (`T005`, `T006`, `T007`, `T008`, `T009`) se pueden iniciar en paralelo dentro de la fase Foundational si no dependen de una implementación previa concreta.
- Las pruebas unitarias de historias de usuario (`T012`-`T019`) pueden escribirse en paralelo por diferentes miembros del equipo.
- Las tareas de polish (`T020`-`T023`) son paralelizables después de la implementación principal.

---

## Implementation Strategy

1. Completar Phase 1 y Phase 2 para contar con la infraestructura de persistencia y seeding.
2. Implementar los tests de US1, US2 y US3 en paralelo si hay capacidad, priorizando US1 como MVP.
3. Validar la migración única y la idempotencia del seeding antes de pulir la documentación.
4. Finalizar con la verificación de los activos de soporte y la ruta pública de imagen.
