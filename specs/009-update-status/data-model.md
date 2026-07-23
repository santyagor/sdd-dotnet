# Modelo de diseño: Actualización de estado de propiedad

## 1) `Property`

Entidad persistida que representa una propiedad existente en el sistema.

| Campo | Tipo | Regla |
|---|---|---|
| `Id` | Guid | Identificador único de la propiedad. |
| `Title` | string | No se modifica en esta feature. |
| `Description` | string | No se modifica en esta feature. |
| `Address` | string | No se modifica en esta feature. |
| `Price` | decimal | No se modifica en esta feature. |
| `Status` | `PropertyStatus` | Único campo editable en esta feature. |
| `BedroomCount` | int | No se modifica en esta feature. |
| `BathroomCount` | int | No se modifica en esta feature. |
| `AreaSquareMeters` | decimal | No se modifica en esta feature. |
| `ImageUrl` | string? | Debe conservarse intacto. |

**Relaciones**:
- Se persiste en PostgreSQL.
- Es la fuente de verdad para la respuesta actualizada.

## 2) `UpdatePropertyStatusRequest`

Contrato de entrada del endpoint `PATCH /api/properties/{id}/status`.

| Campo | Tipo | Regla |
|---|---|---|
| `id` | Guid | Debe venir en la ruta y ser válido. |
| `status` | `PropertyStatus` | Debe venir en el body JSON y ser válido. |

**Reglas de validación**:
- Si `status` falta, la solicitud falla con `400 Bad Request`.
- Si `status` no corresponde a un valor admitido del enum, la solicitud falla con `400 Bad Request`.
- No se admiten campos adicionales para edición parcial.
- No se admiten archivos ni `multipart/form-data`.

## 3) `UpdatePropertyStatusResponse`

Contrato de salida con la propiedad actualizada.

| Campo | Tipo | Regla |
|---|---|---|
| `Id` | Guid | Debe coincidir con el id solicitado. |
| `Title` | string | Debe conservar el valor persistido. |
| `Description` | string | Debe conservar el valor persistido. |
| `Address` | string | Debe conservar el valor persistido. |
| `Price` | decimal | Debe conservar el valor persistido. |
| `Status` | `PropertyStatus` | Debe reflejar el nuevo estado. |
| `BedroomCount` | int | Debe conservar el valor persistido. |
| `BathroomCount` | int | Debe conservar el valor persistido. |
| `AreaSquareMeters` | decimal | Debe conservar el valor persistido. |
| `ImageUrl` | string? | Debe conservar el valor persistido sin cambios. |

**Relaciones**:
- Se devuelve al consumidor tras una actualización exitosa.
- Reutiliza la misma forma pública que el resto de consultas de propiedad.

## 4) `PropertyStatus`

Enum del dominio usado para representar el estado de una propiedad.

| Valor | Significado |
|---|---|
| `Available` | La propiedad está disponible. |
| `Rented` | La propiedad está alquilada. |
| `Maintenance` | La propiedad está en mantenimiento. |

**Reglas de integridad**:
- Solo estos valores pueden aceptarse en la actualización.
- El cuerpo JSON debe transportar el valor numérico correspondiente al enum.

## 5) `UpdatePropertyStatusResult`

Resultado funcional del caso de uso.

| Estado | Condición |
|---|---|
| `Success` | La propiedad existe y el estado se actualiza correctamente. |
| `NotFound` | No existe una propiedad con el id solicitado. |
| `ValidationError` | El cuerpo no contiene `status` válido o contiene datos no permitidos. |

## Reglas de negocio

- Solo se actualiza `Status`.
- El resto de campos persisten sin cambios.
- No se permite carga de archivos.
- No se modifica la semántica de otros endpoints.
- La respuesta debe ser apta para documentación OpenAPI y para consumo manual desde `.http`.
