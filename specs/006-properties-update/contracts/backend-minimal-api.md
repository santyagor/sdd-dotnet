# Contrato del backend Minimal API: Actualización de propiedades

## Alcance

Contrato HTTP para la actualización de propiedades expuesto por el backend.

## Recurso

- Método: `PUT`
- Ruta: `/api/properties/{id}`
- Autenticación: no requerida
- Tipo de contenido: `multipart/form-data`

## Campos de la solicitud

### Parámetro de ruta

- `id`: identificador de la propiedad a actualizar.

### Campos de texto requeridos

- `title`
- `description`
- `address`
- `price`
- `status`
- `bedroomCount`
- `bathroomCount`
- `areaSquareMeters`

### Campo opcional de archivo

- `imageFile`: archivo opcional con imagen PNG o JPG.

## Reglas de validación

- El sistema debe buscar la propiedad por `id` antes de aplicar cambios.
- Si la propiedad no existe, debe responder `404 Not Found`.
- Todos los campos de texto de negocio son obligatorios porque la actualización es completa.
- `imageFile` puede omitirse.
- Si `imageFile` existe, debe ser PNG o JPG válido.
- El tamaño máximo admitido es 5 MB.
- Si `imageFile` está vacío, tiene formato no permitido o el contenido real no corresponde a una imagen soportada, debe responder `415 Unsupported Media Type`.
- Si la imagen supera 5 MB, debe responder `413 Payload Too Large`.
- Si la validación de negocio falla, debe responder `400 Bad Request`.
- Si falla el almacenamiento de imagen, debe responder `500 Internal Server Error`.
- Si no hay imagen nueva, `imageUrl` debe conservarse sin cambios.
- Si hay imagen nueva válida, debe almacenarse en `app/backend/src/RealtorApi/wwwroot/assets/properties` con un nombre interno único nuevo y debe persistirse la nueva referencia pública/relativa.
- La imagen anterior debe eliminarse después de una actualización exitosa.

## Respuesta exitosa

- Código HTTP: `200 OK`
- Tipo de contenido: `application/json`

### Forma del payload

```json
{
  "id": "b7d4dbd6-4e73-4f38-bc79-9b7afbf2f9a4",
  "title": "Casa Coral Way renovada",
  "description": "Casa familiar actualizada...",
  "address": "2450 SW 22nd St, Miami, FL 33145",
  "price": 4500.00,
  "status": "Available",
  "bedroomCount": 4,
  "bathroomCount": 3,
  "areaSquareMeters": 195.50,
  "imageUrl": "/assets/properties/9f7b2f2c0c2c4df2b1cf2a7e8d4db2a8.png"
}
```

## Respuestas de error

### 404 Not Found

- Código HTTP: `404 Not Found`
- Tipo de contenido: `application/problem+json`
- Se devuelve cuando la propiedad no existe.

### 400 Bad Request

- Código HTTP: `400 Bad Request`
- Tipo de contenido: `application/problem+json`
- Se devuelve cuando los campos de negocio editables son inválidos.

### 415 Unsupported Media Type

- Código HTTP: `415 Unsupported Media Type`
- Tipo de contenido: `application/problem+json`
- Se devuelve cuando el archivo está vacío, no es un tipo permitido o su contenido real es inválido.

### 413 Payload Too Large

- Código HTTP: `413 Payload Too Large`
- Tipo de contenido: `application/problem+json`
- Se devuelve cuando la imagen supera 5 MB.

### 500 Internal Server Error

- Código HTTP: `500 Internal Server Error`
- Tipo de contenido: `application/problem+json`
- Se devuelve cuando falla el almacenamiento interno de la imagen.

## Reglas contractuales

- El endpoint debe ser público.
- No deben exponerse rutas físicas del servidor.
- La lógica de negocio debe residir en un slice con handler, validator y mapping explícitos.
- La respuesta debe reflejar el estado final persistido.
- La imagen previa no debe cambiar cuando no se envía una nueva.

## Notas

Este contrato cubre exclusivamente la actualización de propiedades existentes. La creación, el listado y otros casos de uso se manejan en specs separadas.
