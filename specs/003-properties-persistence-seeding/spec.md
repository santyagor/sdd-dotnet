# Feature Specification: Persistence and Seeding de Propiedades

**Feature Branch**: `003-properties-persistence-seeding`

**Created**: 2026-07-20

**Estado**: Implementada

**Input**: User description: "Crear la especificación 003-properties-persistence-seeding."

## Clarifications

### Session 2026-07-20

- Q: Para esta spec, ¿debo incluir endpoints de consulta de propiedades usando el patrón de slices (al menos `ListProperties` y `GetPropertyById`) como parte del alcance, o debo limitarme solo a la infraestructura de persistencia, migración y seeding? → A: Limitar la spec al modelo persistente, migración, seeding y hosting de imágenes; los endpoints quedan para otra spec.

### Seed Manifest Schema (resumen)

- `propertiesJson`: string — ruta relativa al archivo `properties.json` dentro de `support/seed-data`.
- `propertyStatusesJson`: string — ruta relativa al archivo `properties-statuses.json`.
- `imagesSourceDirectory`: string — directorio de origen donde residen las imágenes de soporte.
- `imagesPublicDirectory`: string — directorio público de destino (por ejemplo `wwwroot/images/properties`).
- `imageUrlBase`: string — base pública para construir `ImageUrl` (por ejemplo `/images/properties`).

Nota: el seeder debe resolver rutas usando los campos del manifiesto y no emplear rutas hardcodeadas.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Inicializar catálogo de propiedades con datos de support (Priority: P1)

Un administrador del sistema necesita que la API cree automáticamente el catálogo inicial de propiedades desde archivos de soporte externos cada vez que arranca la aplicación.

**Why this priority**: Sin datos iniciales no se puede demostrar la persistencia de propiedades ni el acceso a imágenes públicas.

**Independent Test**: Verificar que un arranque de la aplicación aplica migraciones y ejecuta el seeding async, dejando propiedades en la base de datos con `ImageUrl` apuntando a rutas servidas por la API.

**Acceptance Scenarios**:

1. **Given** una base de datos nueva, **When** la aplicación inicia, **Then** la migración se aplica y los datos de `properties.json` y `properties-statuses.json` se insertan idempotentemente.
2. **Given** que la aplicación se inicia de nuevo, **When** arranca, **Then** el seeding no duplica registros y no falla.
3. **Given** el manifiesto `seed-manifest.json`, **When** el seeder procesa rutas, **Then** las imágenes referenciadas se copian o sincronizan al destino final público de la API.

---

### User Story 2 - Consultar propiedades con imagen pública (Priority: P2)

Un consumidor de API necesita leer propiedades persistidas y obtener la ruta pública de la imagen para mostrarla en una interfaz.

**Why this priority**: El valor principal de la persistencia es que las propiedades y sus imágenes sean consumibles en peticiones posteriores.

**Independent Test**: Consultar propiedades guardadas y validar que `ImageUrl` es una ruta pública del API que sirve la imagen final.

**Acceptance Scenarios**:

1. **Given** propiedades seedadas, **When** se consulta la colección de propiedades, **Then** cada propiedad incluye `ImageUrl` con ruta de acceso público en el API.
2. **Given** una propiedad con estado `Available`, **When** se consulta la propiedad, **Then** el estado se devuelve sin ambigüedad y el valor persiste como cadena en la base de datos.

---

### User Story 3 - Definir y persistir estados de propiedad como enum string (Priority: P3)

El dominio debe mantener estados de propiedad tipados para `Available`, `Rented` y `Maintenance`, mientras que la base de datos guarda esos valores como texto legible.

**Why this priority**: Garantiza consistencia entre el dominio y el esquema relacional sin depender de valores numéricos.

**Independent Test**: Validar la configuración EF que convierte `PropertyStatus` a string y que el esquema generado refleja el tipo de datos correcto.

**Acceptance Scenarios**:

1. **Given** la entidad `Property` con `Status`, **When** se crea la migración, **Then** la columna de estado es un `string` en el esquema generado.
2. **Given** un valor de estado existente, **When** se lee la entidad de la base de datos, **Then** se reconstruye el enum `PropertyStatus` correctamente.

---

### Edge Cases

- Qué ocurre cuando `seed-manifest.json` apunta a una imagen inexistente en el directorio de soporte.
- Cómo se comporta el seeding si los JSON de datos contienen propiedades repetidas o estados no válidos.
- Qué pasa si la aplicación arranca con el directorio de imágenes ya existente en el destino público.
- Cómo se gestiona el caso en que la ruta pública de imagen no pueda resolverse desde el manifiesto.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema debe definir la entidad de dominio `PropertyStatus` con valores `Available`, `Rented` y `Maintenance`.
- **FR-002**: El sistema debe definir la entidad persistente `Property` con los campos `Id`, `Title`, `Description`, `Address`, `Price`, `Status`, `BedroomCount`, `BathroomCount`, `AreaSquareMeters`, `ImageUrl`, `CreatedAt` y `UpdatedAt`.
- **FR-003**: El estado `Status` debe almacenarse como texto en la base de datos mediante conversión EF Core.
- **FR-004**: La configuración de las entidades debe residir en `IEntityTypeConfiguration` dentro de `Infrastructure/Persistence/Configurations`, sin mapping inline en `OnModelCreating`.
- **FR-005**: `AppDbContext` debe exponer los `DbSet` necesarios y aplicar la configuración de entidades desde `Infrastructure/Persistence/Configurations`.
- **FR-006**: El proyecto debe incluir una única migración EF Core que cubra todo el modelo de `Property` y `PropertyStatus` de esta spec.
- **FR-007**: El database seeding debe ser implementado en `DatabaseSeeder` con versiones sincrónica y asíncrona equivalentes, y debe ser idempotente.
- **FR-008**: `Program` debe ejecutar `MigrateAsync` antes de `Run` en cada inicio de aplicación.
- **FR-009**: El seeding debe usar `UseSeeding` y `UseAsyncSeeding` en `DbContext`, incluso si no hay migraciones pendientes.
- **FR-010**: Los archivos `properties.json`, `properties-statuses.json`, `seed-manifest.json` y las imágenes de `support` deben integrarse en `RealtorApi.csproj` para estar disponibles en runtime.
- **FR-011**: El seed debe resolver rutas desde `seed-manifest.json` y no usar rutas hardcodeadas dispersas.
- **FR-012**: El `ImageUrl` de `Property` debe almacenar la ruta final pública servida por la API, no la ruta física de soporte.
- **FR-013**: El proyecto no debe usar `HasData` para este escenario de seeding.
- **FR-014**: No se deben usar controllers para esta feature.
- **FR-015**: No se debe condicionar `MigrateAsync` a la existencia de migraciones pendientes.

### Key Entities *(include if feature involves data)*

- **PropertyStatus**: Enum de dominio que representa el estado de una propiedad. Debe persistirse como `string` y permitir los valores `Available`, `Rented` y `Maintenance`.
- **Property**: Entidad de dominio y persistencia que representa un inmueble con título, descripción, dirección, precio, cantidad de dormitorios y baños, área, estado, URL pública de imagen y marcas de tiempo de creación/actualización.
- **SeedManifest**: Estructura de control de seeding que describe los archivos JSON de entrada y las rutas de imagen externas que deben copiarse o sincronizarse hacia el destino público del API.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El proyecto compila sin errores con `Property` y `PropertyStatus` definidos según la spec.
- **SC-002**: Existe una única migración EF Core que define el esquema completo requerido y su snapshot es coherente con el modelo.
- **SC-003**: Un arranque limpio de la aplicación ejecuta `MigrateAsync` y el seeding async sin fallos.
- **SC-004**: Reiniciar la aplicación repite el seeding sin duplicar datos ni generar errores.
- **SC-005**: Los archivos JSON y el manifiesto se leen correctamente desde runtime y el seed se realiza desde esos archivos.
- **SC-006**: Las imágenes de `support` se copian/sincronizan al destino público del API y `ImageUrl` guarda la ruta pública consumible.
- **SC-007**: Los datos seedados incluyen `ImageUrl` que apunta a una ruta pública que podrá ser servida por la API en despliegues o specs posteriores.

Nota: Esta spec cubre únicamente persistencia, migración, seeding y hosting de imágenes. La exposición de endpoints de consulta queda para una spec posterior.

## Assumptions

- Se asume que el backend sigue usando ASP.NET Core Minimal APIs y que la persistencia se realizará con EF Core sobre PostgreSQL.
- Se asume que las reglas de persistencia definidas en `database.instructions.md` aplican de forma estricta a esta spec.
- Se asume que los activos de soporte pueden publicarse desde la API en una carpeta estática o equivalente que sea accesible en tiempo de ejecución.
- Se asume que el manifest central (`support/seed-data/seed-manifest.json`) será la fuente única de verdad para la ubicación de los archivos de seed.
- Se asume que no hay necesidad de exponer un endpoint de CRUD completo en esta spec; el foco es el modelo persistente, la migración y el seeding.
