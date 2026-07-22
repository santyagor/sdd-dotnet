# Contrato del backend Minimal API: Alta de propiedades

## Alcance

Contrato HTTP para el alta de propiedades expuesto por el backend.

## Recurso

- Método: `POST`
- Ruta: `/api/properties`
- Autenticación: no requerida
- Tipo de contenido: `multipart/form-data`

## Campos de la solicitud

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

- La solicitud debe incluir todos los campos de texto requeridos.
- `imageFile` puede omitirse.
- Si `imageFile` existe, debe ser PNG o JPG.
- El tamaño máximo admitido es 5 MB.
- Si la validación falla, la propiedad no debe persistirse.
- Si la imagen es válida, debe almacenarse en `app/backend/src/RealtorApi/wwwroot/assets/properties` con un nombre único nuevo.
- Si no hay imagen, `imageUrl` debe persistirse en `null`.

## Respuesta exitosa

- Código HTTP: `201 Created`
- Tipo de contenido: `application/json`

### Forma del payload

```json
{
  "id": "b7d4dbd6-4e73-4f38-bc79-9b7afbf2f9a4",
  "title": "Casa Coral Way con jardín tropical",
  "description": "Casa familiar con espacios amplios...",
  "address": "2450 SW 22nd St, Miami, FL 33145",
  "price": 4200.00,
  "status": "Available",
  "bedroomCount": 3,
  "bathroomCount": 2,
  "areaSquareMeters": 185.50,
  "imageUrl": null
}
```

## Respuesta de error de validación

- Código HTTP: `400 Bad Request`
- Tipo de contenido: `application/problem+json`
- Debe devolverse un contrato consistente con `ValidationProblemDetails`.

## Reglas contractuales

- El endpoint debe ser público.
- No deben exponerse rutas físicas del servidor.
- La lógica de negocio debe residir en un slice con handler, validator y mapping explícitos.
- La respuesta debe reflejar el estado final persistido.

## Notas

Este contrato cubre exclusivamente el alta de propiedades. El listado y otros casos de uso se manejan en specs separadas.
