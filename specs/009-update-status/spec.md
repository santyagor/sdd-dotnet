# Especificación de la funcionalidad: Actualización de estado de propiedad

**Feature Branch**: `[009-update-status]`

**Created**: 2026-07-22

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Crear una nueva spec llamada 009-update-status para el backend de Realtor API, basada en el estilo de la spec 006-properties-update, pero con un alcance mucho más pequeño.

Objetivo:
Crear un endpoint público para actualizar únicamente el estado de una propiedad existente identificada por su id.

Alcance:

Solo se puede actualizar el campo status.
No se pueden modificar otros campos como title, description, address, price, bedroomCount, bathroomCount, areaSquareMeters ni imageUrl.
No debe incluir imagen ni subida de archivos.
Debe seguir el patrón de Minimal API + Vertical Slice usado en el proyecto.
El endpoint debe ser público, sin autenticación.
Debe incluir ejemplo de prueba manual en el archivo .http del proyecto.
Debe incluir contrato OpenAPI, quickstart, tasks y documentación en español.

Comportamiento esperado:

Usar PATCH, por ejemplo /api/properties/{id}/status.
Si la propiedad no existe, responder 404 Not Found.
Si el status viene omitido o inválido, responder 400 Bad Request.
Si la actualización es exitosa, responder 200 OK.
La respuesta debe devolver la propiedad actualizada, conservando intactos los demás datos persistidos, incluyendo imageUrl.

Criterios mínimos:

Actualización correcta del status por id.
Rechazo con 404 si no existe la propiedad.
Rechazo con 400 si el body no trae status o si el valor no es válido.
Verificación manual desde .http.
Mantener consistencia con ProblemDetails y con el estilo de las specs anteriores.

Importante:

No implementar actualización parcial de otros campos.
No incluir imagen.
No cambiar la semántica de endpoints existentes.
Mantener el alcance alineado con la constitución y el flujo Speckit del repositorio."

## Clarificaciones

### Session 2026-07-22

- Q: Formato del body para actualizar el estado → A: PATCH con `application/json` y body `{"status":0}` usando el valor numérico del enum `PropertyStatus`.

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Actualizar el estado de una propiedad existente (Prioridad: P1)

Como usuario del sistema, necesito cambiar el estado de una propiedad existente mediante su id para reflejar su situación actual sin alterar sus demás datos.

**Por qué es prioritario**: Es el caso de uso principal y el único valor funcional que aporta esta feature.

**Prueba independiente**: Puede verificarse enviando un `PATCH` válido desde el archivo `.http` del proyecto a una propiedad existente; el sistema debe devolver `200 OK` y la propiedad con el nuevo estado.

**Escenarios de aceptación**:

1. **Dado** que la propiedad existe y envío un estado válido, **cuando** confirmo la actualización, **entonces** el sistema devuelve `200 OK` y la propiedad refleja el nuevo estado.
2. **Dado** que la propiedad existe y el estado cambia correctamente, **cuando** consulto la respuesta, **entonces** los demás datos persistidos permanecen intactos, incluyendo `imageUrl`.

---

### Historia de usuario 2 - Rechazar solicitudes sin estado válido (Prioridad: P2)

Como usuario del sistema, necesito recibir un error claro cuando la solicitud no incluye un estado o el valor no es válido para poder corregir el cuerpo de la petición.

**Por qué es prioritario**: Protege la integridad de los datos y evita actualizaciones ambiguas o incompletas.

**Prueba independiente**: Puede verificarse enviando solicitudes `PATCH` sin `status`, con `status` inválido o con datos que intenten modificar otros campos; el sistema debe devolver `400 Bad Request`.

**Escenarios de aceptación**:

1. **Dado** que la solicitud no incluye `status`, **cuando** intento actualizar la propiedad, **entonces** recibo `400 Bad Request`.
2. **Dado** que la solicitud incluye un valor de estado inválido o campos no permitidos, **cuando** intento actualizar la propiedad, **entonces** recibo `400 Bad Request` y no se modifica ningún dato.

---

### Historia de usuario 3 - Rechazar actualizaciones de propiedades inexistentes (Prioridad: P3)

Como usuario del sistema, necesito recibir una respuesta clara cuando intento actualizar una propiedad que no existe para saber que debo usar un id válido.

**Por qué es prioritario**: Evita operaciones engañosas y deja claro que la propiedad objetivo no está disponible.

**Prueba independiente**: Puede verificarse enviando un `PATCH` con un id inexistente; el sistema debe devolver `404 Not Found`.

**Escenarios de aceptación**:

1. **Dado** que la propiedad no existe, **cuando** envío una actualización de estado, **entonces** recibo `404 Not Found`.
2. **Dado** que el id no corresponde a ninguna propiedad persistida, **cuando** reviso el resultado, **entonces** no se modifica ningún dato existente.

---

### Casos borde

- ¿Qué ocurre si el body no trae `status`? La operación debe devolver `400 Bad Request`.
- ¿Qué ocurre si `status` contiene un valor no válido? La operación debe devolver `400 Bad Request`.
- ¿Qué ocurre si la solicitud intenta incluir otros campos editables? La operación debe rechazarlos y no modificar nada más allá del estado.
- ¿Qué ocurre si se envía una imagen o un archivo? La operación debe rechazarlo porque esta feature no admite subida de archivos.
- ¿Qué ocurre si la propiedad existe pero el estado nuevo coincide con el actual? La operación debe responder correctamente con `200 OK` sin alterar otros datos.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer un endpoint público `PATCH /api/properties/{id}/status` para actualizar el estado de una propiedad sin requerir autenticación o autorización.
- **FR-002**: El sistema DEBE aceptar una solicitud `application/json` que permita modificar únicamente el campo `status` de la propiedad mediante un valor numérico del enum `PropertyStatus`.
- **FR-003**: El sistema NO DEBE permitir modificar `title`, `description`, `address`, `price`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` ni `imageUrl` en esta operación.
- **FR-004**: El sistema NO DEBE aceptar imagen ni subida de archivos en esta feature.
- **FR-005**: Si la propiedad no existe, el sistema DEBE responder `404 Not Found` y no modificar ningún dato.
- **FR-006**: Si `status` viene omitido, no es un valor numérico válido o no corresponde a un valor admitido de `PropertyStatus`, el sistema DEBE responder `400 Bad Request`.
- **FR-007**: Si la actualización es exitosa, el sistema DEBE responder `200 OK`.
- **FR-008**: La respuesta DEBE devolver la propiedad actualizada con todos sus demás datos persistidos sin cambios, incluyendo `imageUrl`.
- **FR-009**: El sistema DEBE mantener la semántica de errores consistente con `ProblemDetails`.
- **FR-010**: La solución DEBE mantener el patrón de Minimal API + Vertical Slice para este caso de uso.
- **FR-011**: La solución DEBE incluir un ejemplo de prueba manual en el archivo `.http` del proyecto para validar la actualización de estado.
- **FR-012**: La solución DEBE mantener intacta la semántica de los demás endpoints existentes.

### Entidades clave

- **Propiedad**: Registro persistido de un inmueble con datos de negocio, estado e imagen pública.
- **Estado de propiedad**: Valor de dominio que representa la situación actual de la propiedad.
- **Solicitud de actualización de estado**: Petición enviada para cambiar únicamente el estado de una propiedad existente.
- **Respuesta de actualización**: Representación de la propiedad actualizada devuelta al consumidor.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las solicitudes válidas a `PATCH /api/properties/{id}/status` devuelve `200 OK` con la propiedad actualizada.
- **SC-002**: El 100% de las solicitudes con `status` omitido o inválido devuelve `400 Bad Request`.
- **SC-003**: El 100% de las solicitudes sobre propiedades inexistentes devuelve `404 Not Found`.
- **SC-004**: En una verificación manual desde `.http`, el flujo de actualización de estado puede completarse sin modificar otros campos de la propiedad.
- **SC-005**: El 100% de las respuestas exitosas conserva sin cambios los datos persistidos distintos de `status`, incluyendo `imageUrl`.

## Suposiciones

- Ya existen la solución base, la persistencia de propiedades y el modelo de estado necesarios para esta operación.
- La actualización de estado reutiliza la arquitectura Vertical Slice ya establecida en el backend.
- La validación de contrato y la documentación OpenAPI se alinearán con la superficie pública existente del proyecto.
- El archivo `.http` del proyecto ya es el lugar adecuado para la prueba manual de esta feature.
- El alcance de esta spec se limita exclusivamente a actualizar el estado de propiedades existentes.
