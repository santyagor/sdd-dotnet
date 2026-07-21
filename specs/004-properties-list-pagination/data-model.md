# Modelo de datos: Paginación de listado de propiedades

## Entidades y contratos principales

### `ListPropertiesQuery`

Contrato de entrada para el endpoint `GET /api/properties`.

**Campos**:
- `page`: entero opcional; valor por defecto `1`.
- `pageSize`: entero opcional; valor por defecto `6`.

**Reglas**:
- `page` debe ser mayor que cero.
- `pageSize` debe ser mayor que cero.

### `PropertyListItem`

Contrato de salida por elemento de propiedad en el listado paginado.

**Campos**:
- `id`: identificador de la propiedad.
- `title`: título visible.
- `description`: descripción resumida o completa.
- `address`: dirección física.
- `price`: precio publicado.
- `status`: estado persistido de la propiedad.
- `bedroomCount`: número de habitaciones.
- `bathroomCount`: número de baños.
- `areaSquareMeters`: área en metros cuadrados.
- `imageUrl`: URL pública absoluta de la imagen.

**Reglas**:
- No debe mapear directamente la entidad `Property`.
- `imageUrl` debe formarse con `scheme://host/assets/properties/{fileName}`.
- El `fileName` debe derivarse del valor persistido de la imagen.

### `PaginatedPropertyListResponse`

Envelope de salida del endpoint.

**Campos**:
- `items`: colección de `PropertyListItem`.
- `page`: página actual.
- `pageSize`: tamaño de página solicitado o aplicado.
- `totalItems`: total de registros disponibles.
- `totalPages`: total de páginas calculadas.
- `hasNext`: indica si existe una página siguiente.
- `hasPrevious`: indica si existe una página previa.

**Reglas**:
- La colección `items` debe respetar el orden ascendente por `title`.
- El metadato `totalPages` debe calcularse a partir de `totalItems` y `pageSize`.
- Si la página solicitada excede el total disponible, `items` debe ser una colección vacía con metadatos consistentes.

## Relación con el modelo persistente existente

La feature reutiliza la entidad persistente `Property` creada por la spec 003. No se introducen nuevas entidades EF Core.

### Campos persistidos relevantes

- `Id`
- `Title`
- `Description`
- `Address`
- `Price`
- `Status`
- `BedroomCount`
- `BathroomCount`
- `AreaSquareMeters`
- `ImageUrl`

### Transformación requerida

El valor persistido de `ImageUrl` se considera una referencia relativa o un valor derivado de imagen pública. El slice debe transformar ese valor en la URL pública absoluta esperada por el contrato de lectura.

## Validaciones de dominio del caso de uso

- `page > 0`
- `pageSize > 0`
- La página calculada debe ser coherente con el total disponible.
- La respuesta no debe revelar rutas físicas internas.

## Notas

- La respuesta de esta funcionalidad es un contrato de lectura y no debe reutilizar directamente el tipo persistente.
- El orden y la paginación deben ocurrir en la capa de acceso a datos o en el handler del slice, no en el cliente.
