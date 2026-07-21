# Contrato del backend Minimal API: Listado paginado de propiedades

## Alcance

Contrato HTTP para el recurso de lectura paginada de propiedades expuesto por la API.

## Recurso

- Método: `GET`
- Ruta: `/api/properties`

## Parámetros de consulta

- `page` (opcional): entero mayor que cero. Valor por defecto: `1`.
- `pageSize` (opcional): entero mayor que cero. Valor por defecto: `6`.

## Respuesta exitosa

- Código HTTP: `200 OK`
- Tipo de contenido: `application/json`

### Forma del payload

```json
{
  "items": [
    {
      "id": "a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1001",
      "title": "Casa Coral Way con jardín tropical",
      "description": "Casa familiar con espacios amplios...",
      "address": "2450 SW 22nd St, Miami, FL 33145",
      "price": 4200.00,
      "status": "Available",
      "bedroomCount": 3,
      "bathroomCount": 2,
      "areaSquareMeters": 185.50,
      "imageUrl": "http://localhost:5023/assets/properties/1.png"
    }
  ],
  "page": 1,
  "pageSize": 6,
  "totalItems": 10,
  "totalPages": 2,
  "hasNext": true,
  "hasPrevious": false
}
```

## Validación

Si `page` o `pageSize` son inválidos:

- Código HTTP: `400 Bad Request`
- Tipo de contenido: `application/problem+json`
- Debe devolverse un contrato de validación consistente con `ValidationProblemDetails`.

## Reglas contractuales

- La respuesta no debe devolver entidades EF Core.
- `items` debe contener datos ordenados por `title` ascendente.
- `imageUrl` debe ser una URL absoluta pública con formato `scheme://host/assets/properties/{fileName}`.
- No deben exponerse rutas físicas del servidor.
- La implementación debe ser discoverable como `ISlice` sin registrar endpoints manualmente en `Program.cs`.

## Notas

Este contrato define exclusivamente el listado paginado. Los endpoints de creación, edición, detalle o eliminación de propiedades quedan fuera del alcance de esta spec.
