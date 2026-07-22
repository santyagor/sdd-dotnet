# Especificación de la funcionalidad: Actualización de propiedades

**Feature Branch**: `006-properties-update`

**Created**: 2026-07-22

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Crear la especificación 006-properties-update para definir un nuevo caso de uso de actualización de propiedades mediante un slice Minimal API en backend.

La especificación debe cubrir un endpoint público de actualización por id (por ejemplo PUT /api/properties/{id}), que reciba los datos editables de la propiedad y permita de forma opcional el upload de imagen, manteniendo la misma lógica implementada en 005-properties-create para manejo de imágenes.

Reglas funcionales obligatorias:
1. Antes de actualizar, el sistema debe buscar la propiedad por id en base de datos.
2. Si la propiedad no existe, debe responder Not Found (404) y no realizar ningún cambio.
3. Si existe, debe aplicar la actualización de los campos de negocio válidos.
4. Si el request incluye imagen válida (PNG/JPG, máximo 5 MB), debe almacenarse en el directorio de propiedades (wwwroot/assets/properties o file:properties según la convención vigente), con nombre interno único (UUID + extensión válida), y debe persistirse la nueva imageUrl pública/relativa en la base de datos.
5. Si el request no incluye imagen, no se debe modificar la imageUrl existente; debe conservarse el valor anterior.
6. Si falla el guardado de imagen durante la actualización, la operación debe ser atómica: no debe quedar actualización parcial inconsistente.
7. Mantener semántica de errores consistente con 005:
   - 400 para validaciones de negocio inválidas.
   - 415 para formato no permitido, archivo vacío (0 bytes) o contenido real inválido.
   - 413 para imagen mayor a 5 MB.
   - 500 para fallo interno de almacenamiento de imagen.
   - 404 cuando la entidad no exista.

La spec debe incluir:
1. Historias por prioridad (P1/P2/P3) con escenarios independientes:
   - P1: actualización sin imagen (conservando imageUrl previa).
   - P2: actualización con imagen válida (reemplazando imageUrl).
   - P3: rechazos por validación y manejo de errores.
2. Requisitos funcionales testables (FR) equivalentes en detalle a 005.
3. Edge cases, incluyendo:
   - id inexistente.
   - imagen vacía o con contenido real inválido.
   - colisión de nombre resuelta por UUID.
   - no alteración de imageUrl cuando no se envía imagen.
4. Criterios de éxito medibles (SC), incluyendo latencia p95 en muestra funcional de solicitudes válidas.
5. Alineación explícita con arquitectura Vertical Slice existente:
   - slice + handler + mapping + validator.
   - auto-registro por IHandler e ISlice.
   - mapping sin lógica de negocio.
6. Compatibilidad con specs previas de foundation, persistencia, listado y create (001-005), y con gobernanza Spec-Driven del repositorio."

## Clarificaciones

### Session 2026-07-22

- Q: Alcance de la actualización de campos editables → A: Actualización completa; todos los campos de negocio editables son obligatorios en `PUT`.
- Q: Manejo de la imagen previa al reemplazarla → A: Eliminar la imagen anterior después de guardar la nueva y persistir correctamente la actualización.
## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Actualizar una propiedad sin imagen nueva (Prioridad: P1)

Como usuario del sistema, necesito actualizar los datos de una propiedad existente sin adjuntar una imagen nueva para conservar el recurso visual ya publicado y corregir información de negocio cuando sea necesario.

**Por qué es prioritario**: Es el flujo de mayor frecuencia y debe preservar la imagen existente cuando no se proporciona una nueva.

**Prueba independiente**: Puede verificarse actualizando una propiedad existente con campos válidos y sin imagen; el sistema debe persistir los cambios de negocio y mantener la `imageUrl` anterior sin modificaciones.

**Escenarios de aceptación**:

1. **Dado** que la propiedad existe y envío datos válidos sin imagen nueva, **cuando** confirmo la actualización, **entonces** se guardan los cambios de negocio y la `imageUrl` permanece igual.
2. **Dado** que la propiedad existe y la solicitud no incluye archivo, **cuando** el sistema responde, **entonces** recibo una confirmación exitosa con la propiedad actualizada y sin cambios en la referencia de imagen.

---

### Historia de usuario 2 - Actualizar una propiedad con imagen válida (Prioridad: P2)

Como usuario del sistema, necesito actualizar una propiedad existente con una nueva imagen válida para reemplazar la presentación visual publicada cuando dispongo de un recurso mejor.

**Por qué es prioritario**: La actualización con imagen completa el caso de uso principal y debe reemplazar la referencia existente de forma consistente.

**Prueba independiente**: Puede verificarse actualizando una propiedad existente con una imagen PNG o JPG válida de hasta 5 MB; el sistema debe guardar el archivo y persistir la nueva referencia pública/relativa.

**Escenarios de aceptación**:

1. **Dado** que la propiedad existe y envío una imagen válida junto con datos válidos, **cuando** confirmo la actualización, **entonces** la nueva imagen se almacena en la ubicación pública prevista y la propiedad persiste con la nueva referencia.
2. **Dado** que la propiedad tenía una imagen previa, **cuando** actualizo con una nueva imagen válida, **entonces** la referencia visible por clientes cambia a la nueva ubicación pública/relativa.

---

### Historia de usuario 3 - Rechazar actualizaciones inválidas y errores de almacenamiento (Prioridad: P3)

Como usuario del sistema, necesito recibir errores claros cuando intento actualizar una propiedad inexistente, envío datos inválidos o adjunto imágenes no permitidas, para corregir la solicitud sin dejar datos inconsistentes.

**Por qué es prioritario**: Evita actualizaciones incompletas, protege la integridad de la información y mantiene una semántica de errores consistente.

**Prueba independiente**: Puede verificarse enviando solicitudes con id inexistente, campos de negocio inválidos, imágenes vacías o no admitidas, imágenes mayores a 5 MB y fallos de almacenamiento; el sistema debe responder con el código esperado y sin cambios parciales.

**Escenarios de aceptación**:

1. **Dado** que la propiedad no existe, **cuando** intento actualizarla, **entonces** recibo `404 Not Found` y no se modifica ningún dato.
2. **Dado** que envío una imagen vacía, un formato no admitido o contenido real inválido, **cuando** intento actualizar la propiedad, **entonces** recibo el error correspondiente sin persistir la actualización.
3. **Dado** que la imagen supera 5 MB o el almacenamiento falla durante la operación, **cuando** intento actualizar la propiedad, **entonces** recibo el código esperado y no queda una actualización parcial.

### Casos borde

- ¿Qué ocurre si el id no existe? La operación debe devolver `404 Not Found` sin cambios.
- ¿Qué ocurre si la imagen llega vacía o con contenido real inválido? La operación debe rechazarse con `415 Unsupported Media Type` y no debe persistirse el cambio.
- ¿Qué ocurre si la imagen supera el tamaño máximo? La operación debe rechazarse con `413 Payload Too Large`.
- ¿Qué ocurre si el nombre interno generado colisiona? Debe resolverse con un identificador único nuevo para evitar sobrescrituras.
- ¿Qué ocurre si no se envía archivo? La `imageUrl` existente debe conservarse sin alteración.
- ¿Qué ocurre con la imagen anterior cuando se reemplaza por una nueva? Debe eliminarse después de una actualización exitosa para evitar archivos huérfanos.
- ¿Qué ocurre si falla el almacenamiento de imagen? La operación debe responder con `500 Internal Server Error` y no dejar una actualización parcial.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer un endpoint público `PUT /api/properties/{id}` para actualizar propiedades sin requerir autenticación o autorización.
- **FR-002**: El sistema DEBE buscar la propiedad por id antes de aplicar cambios.
- **FR-003**: Si la propiedad no existe, el sistema DEBE responder `404 Not Found` y no modificar ningún dato.
- **FR-004**: El sistema DEBE aceptar la solicitud de actualización mediante `multipart/form-data` con campos de texto editables y archivo opcional.
- **FR-005**: El sistema DEBE permitir actualizar todos los campos de negocio editables de la propiedad existente en una solicitud completa.
- **FR-006**: Si no se envía imagen, el sistema NO DEBE modificar la `imageUrl` existente.
- **FR-007**: Si la imagen es válida, el sistema DEBE almacenar el archivo con un nombre interno único y persistir la nueva referencia pública/relativa.
- **FR-008**: Si la imagen es válida, el sistema DEBE conservar la coherencia entre el archivo almacenado y la referencia persistida.
- **FR-009**: El sistema DEBE rechazar imágenes vacías, con contenido real inválido o con formato no permitido con `415 Unsupported Media Type`.
- **FR-010**: El sistema DEBE rechazar imágenes mayores a 5 MB con `413 Payload Too Large`.
- **FR-011**: El sistema DEBE devolver `400 Bad Request` cuando los datos de negocio editables sean inválidos.
- **FR-012**: El sistema DEBE devolver `500 Internal Server Error` cuando falle el almacenamiento de imagen durante la actualización.
- **FR-013**: El sistema NO DEBE dejar una actualización parcial si falla la persistencia de imagen o la escritura de datos.
- **FR-014**: El sistema DEBE mantener la lógica de actualización en un slice dedicado con handler, validator y mapping explícitos.
- **FR-015**: El sistema DEBE integrar la actualización con el modelo de datos existente para que la propiedad siga siendo consumible por el listado posterior.

### Entidades clave

- **Propiedad**: Representa un inmueble registrado por el sistema; contiene los datos de negocio editables y la referencia de imagen pública/relativa.
- **Solicitud de actualización de propiedad**: Representa los datos que un usuario envía para modificar una propiedad existente, con una imagen opcional.
- **Imagen de propiedad**: Representa el archivo cargado para reemplazar la presentación visual de la propiedad.
- **Confirmación de actualización**: Representa la respuesta de éxito que informa al consumidor que la propiedad quedó actualizada correctamente.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las actualizaciones válidas sin imagen conservan la `imageUrl` previa y aplican los cambios de negocio.
- **SC-002**: El 100% de las actualizaciones válidas con imagen PNG o JPG de hasta 5 MB persisten una nueva referencia pública/relativa utilizable.
- **SC-003**: El 100% de las solicitudes inválidas reciben el código HTTP esperado y no crean estados parciales.
- **SC-004**: En una muestra funcional de 100 solicitudes válidas, al menos el 95% completa la actualización en 2 segundos o menos.
- **SC-005**: El 100% de las propiedades actualizadas quedan disponibles para consulta posterior en el catálogo sin intervención manual adicional.

## Suposiciones

- Ya existen la base de la solución, el modelo persistente, el listado de propiedades y el alta de propiedades definidos por las specs previas.
- La actualización reutiliza la arquitectura de Vertical Slice ya establecida en el backend.
- La convención vigente de publicación de imágenes de propiedades se conserva y la actualización no introduce una acción separada para eliminar la imagen previa.
- Cuando exista imagen nueva, el archivo se guardará con un nombre único para evitar colisiones entre propiedades.
- El alcance de esta spec se limita a actualizar propiedades existentes y no incluye creación, borrado ni edición de otros recursos relacionados.
