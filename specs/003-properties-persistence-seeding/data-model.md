# data-model.md

## Entidades principales

### PropertyStatus

- Tipo: `enum PropertyStatus`
- Valores:
  - `Available`
  - `Rented`
  - `Maintenance`
- Uso: representa el estado de una propiedad en el modelo de dominio.
- Persistencia: convertido a `string` en la base de datos mediante `HasConversion<string>()` en EF Core.

### Property

- Tipo: entidad de dominio y persistencia.
- Campos:
  - `Guid Id`
  - `string Title`
  - `string Description`
  - `string Address`
  - `decimal Price`
  - `PropertyStatus Status`
  - `int BedroomCount`
  - `int BathroomCount`
  - `decimal AreaSquareMeters`
  - `string ImageUrl`
  - `DateTime CreatedAt`
  - `DateTime UpdatedAt`

## Reglas de validación / restricciones de persistencia

- `Title`: requerido, longitud máxima sugerida 200.
- `Description`: requerido, longitud máxima sugerida 1000.
- `Address`: requerido, longitud máxima sugerida 300.
- `Price`: requerido, precisión `18,2`.
- `BedroomCount`: requerido, entero no negativo.
- `BathroomCount`: requerido, entero no negativo.
- `AreaSquareMeters`: requerido, precisión `10,2`.
- `ImageUrl`: requerido, longitud máxima sugerida 2048, debe ser ruta pública de API.
- `CreatedAt`: requerido, timestamp de creación.
- `UpdatedAt`: requerido, timestamp de última actualización.

## Configuración EF Core

### AppDbContext

- Tabla `Properties` mapeada desde `DbSet<Property> Properties`.
- Aplicar configuraciones de entidad desde `Infrastructure/Persistence/Configurations` usando `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
- Registrar `UseSeeding` y `UseAsyncSeeding` en el constructor o en extenciones de `DbContext`.

### PropertyConfiguration

- Configura la tabla `Properties`.
- Define clave primaria `Id`.
- Configura longitudes y tipos de datos.
- Define conversión `PropertyStatus` -> `string`.
- Configura valores por defecto para las marcas de tiempo si corresponde.

## Semántica de seeding

### SeedManifest

El manifiesto controlará las fuentes de datos y destinos de imagen.

- `propertiesJson`: ruta al archivo `properties.json`.
- `propertyStatusesJson`: ruta al archivo `properties-statuses.json`.
- `imagesSourceDirectory`: ruta de origen de las imágenes de soporte.
- `imagesPublicDirectory`: ruta de destino pública final, por ejemplo `wwwroot/images/properties`.
- `imageUrlBase`: base pública para `ImageUrl`, por ejemplo `/images/properties`.

### Flujo de seed

1. Leer `seed-manifest.json`.
2. Cargar y deserializar `properties.json` y `properties-statuses.json`.
3. Validar que los estados en `properties.json` existen en `properties-statuses.json`.
4. Copiar/sincronizar imágenes referenciadas por el manifiesto al destino público.
5. Construir entidades `Property` con `ImageUrl` pública y marcas de tiempo.
6. Insertar/actualizar datos de forma idempotente en la base de datos.

## Notas de diseño

- No se usa `HasData`.
- El `ImageUrl` persistido debe ser una ruta HTTP/HTTPS relativa pública, no una ruta del sistema de archivos.
- Las imágenes finales se servirán desde `wwwroot` o un FileProvider configurado en el backend.
