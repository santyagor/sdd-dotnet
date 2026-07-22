# Modelo de datos: Consulta de propiedad por id

## Entidades y contratos principales

### `GetPropertyByIdRoute`

Contrato de entrada para `GET /api/properties/{id}`.

**Campos**:
- `id`: identificador de la propiedad en la ruta.

**Reglas**:
- El valor de `id` debe poder interpretarse como un identificador válido de la propiedad.
- Si el valor no tiene formato válido, la operación debe responder según el contrato público documentado para ids inválidos.

### `PropertyDetailResponse`

Contrato de salida para devolver una única propiedad.

**Campos**:
- `id`: identificador de la propiedad.
- `title`: título visible.
- `description`: descripción visible.
- `address`: dirección física.
- `price`: precio publicado.
- `status`: estado persistido de la propiedad.
- `bedroomCount`: número de habitaciones.
- `bathroomCount`: número de baños.
- `areaSquareMeters`: área en metros cuadrados.
- `imageUrl`: URL pública absoluta de la imagen o cadena vacía cuando no existe imagen asociada.

**Reglas**:
- No debe incluir metadatos de paginación.
- No debe exponer rutas físicas internas.
- Debe reflejar el estado final persistido de la propiedad consultada.

### `Property`

Entidad persistente reutilizada por la feature.

**Campos relevantes**:
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
- `CreatedAt`
- `UpdatedAt`

**Reglas**:
- La entidad persistente no debe exponerse directamente en la respuesta.
- `ImageUrl` persistida debe transformarse a una URL absoluta pública usando el esquema y host actuales.

### `PropertyDetailProjection`

Concepto funcional para la proyección de lectura de una propiedad.

**Campos**:
- `id`
- `title`
- `description`
- `address`
- `price`
- `status`
- `bedroomCount`
- `bathroomCount`
- `areaSquareMeters`
- `imageUrl`

**Reglas**:
- Si la propiedad no tiene imagen persistida, `imageUrl` debe ser cadena vacía.
- Si existe imagen persistida, `imageUrl` debe seguir el patrón `http(s)://{host}/assets/properties/{fileName}`.

## Relación entre contratos y persistencia

- La consulta usa una sola entidad `Property`.
- El id de la ruta identifica el recurso a recuperar.
- El contrato de lectura reutiliza exactamente los campos públicos del listado 004, pero sin paginación.
- La URL de imagen se construye a partir del valor persistido y del contexto HTTP actual.

## Validaciones de dominio del caso de uso

- `id` debe ser válido según el contrato público del proyecto.
- Si la propiedad no existe, debe responder `404 Not Found`.
- Si el formato del `id` es inválido, debe responder `400 Bad Request` según el contrato documentado.
- La respuesta no debe revelar detalles internos de persistencia.

## Notas

- Esta feature define una consulta unitaria y no un listado.
- El comportamiento de lectura mantiene la misma convención pública de imágenes usada por las features previas.
- La proyección de detalle debe seguir siendo compatible con clientes externos que consumen el catálogo.
