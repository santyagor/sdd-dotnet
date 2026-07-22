# Modelo de datos: Actualización de propiedades

## Entidades y contratos principales

### `UpdatePropertyRequest`

Contrato de entrada para `PUT /api/properties/{id}`.

**Campos**:
- `id`: identificador de la propiedad en la ruta.
- `title`: texto obligatorio.
- `description`: texto obligatorio.
- `address`: texto obligatorio.
- `price`: número decimal obligatorio.
- `status`: estado obligatorio de la propiedad.
- `bedroomCount`: entero obligatorio.
- `bathroomCount`: entero obligatorio.
- `areaSquareMeters`: número decimal obligatorio.
- `imageFile`: archivo opcional.

**Reglas**:
- La solicitud debe enviarse como `multipart/form-data`.
- Todos los campos de negocio editables son obligatorios porque la actualización es completa.
- `imageFile` puede omitirse.
- Si `imageFile` existe, debe corresponder a una imagen PNG o JPG válida.
- El tamaño máximo permitido es 5 MB.
- Si `imageFile` no se envía, la `imageUrl` existente debe conservarse.

### `UpdatePropertyResponse`

Contrato de salida para confirmar la actualización.

**Campos**:
- `id`: identificador de la propiedad actualizada.
- `title`: título persistido.
- `description`: descripción persistida.
- `address`: dirección persistida.
- `price`: precio persistido.
- `status`: estado persistido.
- `bedroomCount`: número de habitaciones.
- `bathroomCount`: número de baños.
- `areaSquareMeters`: área en metros cuadrados.
- `imageUrl`: ruta pública/relativa persistida.

**Reglas**:
- La respuesta debe reflejar el estado final persistido.
- Si no se envía imagen, `imageUrl` debe conservar el valor anterior.
- Si existe imagen nueva, la respuesta debe devolver la nueva referencia pública/relativa.

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
- La entidad ya admite `ImageUrl` nula.
- La actualización solo modifica los campos de negocio editables y la referencia de imagen cuando el request incluye archivo válido.

### `PropertyImageReplacement`

Concepto funcional para la sustitución de imagen durante la actualización.

**Campos**:
- `previousImageUrl`: referencia actual antes de la actualización.
- `newFileName`: nombre interno generado para la nueva imagen.
- `newImageUrl`: referencia pública/relativa persistida.

**Reglas**:
- Si no hay archivo nuevo, no se modifica `previousImageUrl`.
- Si hay archivo nuevo, se genera un identificador único nuevo para el nombre interno.
- La imagen anterior se elimina después de una actualización exitosa.

## Relación entre contratos y persistencia

- La actualización usa una sola entidad `Property`.
- El id de la ruta identifica el recurso a modificar.
- La imagen es opcional y no debe obligar a reemplazar la referencia existente.
- La operación debe evitar estados parciales: si falla la persistencia o el almacenamiento de imagen, no debe quedar una actualización inconsistente.

## Validaciones de dominio del caso de uso

- `title` requerido y no vacío.
- `description` requerido y no vacío.
- `address` requerido y no vacío.
- `price` obligatorio.
- `status` debe corresponder a un valor válido de `PropertyStatus`.
- `bedroomCount` y `bathroomCount` obligatorios.
- `areaSquareMeters` obligatorio.
- `imageFile` opcional, pero si está presente debe cumplir formato, contenido y tamaño.

## Reglas de error funcional

- `404 Not Found` cuando la propiedad no exista.
- `400 Bad Request` cuando los campos de negocio editables sean inválidos.
- `415 Unsupported Media Type` cuando la imagen esté vacía, tenga formato no permitido o su contenido real sea inválido.
- `413 Payload Too Large` cuando la imagen supere 5 MB.
- `500 Internal Server Error` cuando falle el almacenamiento interno de la imagen.

## Notas

- La respuesta de actualización no expone detalles internos del sistema de archivos.
- El contrato prioriza la conservación de la imagen existente cuando no se envía una nueva.
- La compatibilidad con el listado posterior depende de que la referencia persistida siga siendo pública y relativa.
