# Modelo de datos: Alta de propiedades

## Entidades y contratos principales

### `CreatePropertyRequest`

Contrato de entrada para `POST /api/properties`.

**Campos**:
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
- `imageFile` puede omitirse.
- Si se envía, debe ser una imagen PNG o JPG válida.
- El tamaño máximo permitido es 5 MB.

### `CreatePropertyResponse`

Contrato de salida para confirmar la creación.

**Campos**:
- `id`: identificador de la propiedad creada.
- `title`: título persistido.
- `description`: descripción persistida.
- `address`: dirección persistida.
- `price`: precio persistido.
- `status`: estado persistido.
- `bedroomCount`: número de habitaciones.
- `bathroomCount`: número de baños.
- `areaSquareMeters`: área en metros cuadrados.
- `imageUrl`: ruta pública/relativa persistida o `null`.

**Reglas**:
- La respuesta debe reflejar el estado final persistido.
- `imageUrl` debe ser `null` cuando no se adjunte imagen.
- Si existe imagen, la respuesta debe incluir la ruta pública/relativa del archivo almacenado.

### `Property`

Entidad persistente reutilizada por la nueva spec.

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
- `ImageUrl` debe admitir `null` cuando no exista imagen.
- El archivo físico no forma parte de la entidad, solo su referencia pública/relativa.

### `PropertyImageUpload`

Concepto funcional para el archivo recibido en la solicitud.

**Campos**:
- `fileName`: nombre original del archivo.
- `contentType`: tipo MIME recibido.
- `size`: tamaño en bytes.

**Reglas**:
- Solo se aceptan PNG/JPG.
- No debe superar 5 MB.
- El archivo almacenado debe recibir un nombre único nuevo.

## Relación entre contratos y persistencia

- La solicitud crea una sola entidad `Property`.
- La imagen es opcional y no debe bloquear la creación básica.
- La referencia persistida debe ser consumible por el catálogo posterior.
- El almacenamiento físico de la imagen y la persistencia de la entidad deben quedar coherentes; si la validación falla, no debe quedar un estado parcial.

## Validaciones de dominio del caso de uso

- `title` requerido y no vacío.
- `description` requerido y no vacío.
- `address` requerido y no vacío.
- `price` obligatorio.
- `status` debe corresponder a un valor válido de `PropertyStatus`.
- `bedroomCount` y `bathroomCount` obligatorios.
- `areaSquareMeters` obligatorio.
- `imageFile` opcional, pero si está presente debe cumplir formato y tamaño.

## Notas

- La respuesta de alta no expone detalles internos del sistema de archivos.
- El contrato prioriza la trazabilidad del alta y la compatibilidad con el listado posterior.
