# Especificación de la funcionalidad: Consulta de propiedad por id

**Feature Branch**: `007-properties-get-by-id`

**Created**: 2026-07-22

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Definir e implementar un endpoint público de consulta por id que devuelva una única propiedad: GET /api/properties/{id}. Basarse explícitamente en la spec 004-properties-list-pagination y mantener los mismos principios de contrato de respuesta. Reutilizar el mismo criterio de exposición de imagen al cliente: imageUrl debe ser URL absoluta y pública, construida con el esquema y host de la request actual, con patrón http(s)://{host}/assets/properties/{fileName}, sin rutas físicas internas ni URLs relativas. Mantener arquitectura Vertical Slice + Minimal APIs existente, auto-registro por ISlice/IHandler y sin controllers. Mantener cancelación con CancellationToken y contrato público explícito sin exponer entidades EF directamente. Mantener gobernanza Spec-Driven del repositorio y dependencias con specs previas (foundation, persistencia, listado, alta, actualización). Resultado funcional esperado: cuando el id existe, devolver una respuesta 200 con los campos públicos de propiedad usados en 004: id, title, description, address, price, status, bedroomCount, bathroomCount, areaSquareMeters, imageUrl. Cuando el id no existe, devolver 404 con ProblemDetails. Definir comportamiento ante id inválido según contrato público del proyecto (y documentarlo explícitamente en spec/contract). Requisitos de especificación: historias de usuario con prioridad P1/P2/P3 y pruebas independientes, escenarios de aceptación claros, casos límite (id inexistente, id inválido, propiedad sin imagen, host/esquema variable por entorno), requisitos funcionales testables (FR-xxx), criterios de éxito medibles (SC-xxx), suposiciones explícitas, contrato OpenAPI de la operación en contracts/backend-properties-get-by-id.openapi.yaml, data model y quickstart de validación manual. Criterios mínimos de response: imageUrl siempre consumible por cliente externo cuando exista imagen, contrato consistente con 004 para campos de propiedad, sin metadatos de paginación en esta operación por ser consulta unitaria."

## Clarifications

### Session 2026-07-22

- Q: Comportamiento ante un id con formato inválido → A: Responder `400 Bad Request` con `ProblemDetails`.
- Q: Representación de `imageUrl` cuando no existe imagen asociada → A: Devolver cadena vacía.

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Consultar una propiedad existente (Prioridad: P1)

Como consumidor de la API, necesito consultar una propiedad específica por su id para mostrar su información detallada en una interfaz o flujo de revisión.

**Por qué es prioritario**: Es el caso principal de consulta unitaria y reutiliza el mismo contrato de lectura pública definido para el catálogo.

**Prueba independiente**: Puede verificarse consultando `GET /api/properties/{id}` con un id existente y comprobando que devuelve un único recurso con los campos públicos esperados y una `imageUrl` absoluta cuando exista imagen.

**Escenarios de aceptación**:

1. **Dado** que el id existe, **cuando** consulto la propiedad por id, **entonces** recibo `200 OK` con `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
2. **Dado** que la propiedad tiene imagen persistida, **cuando** consulto la propiedad por id, **entonces** `imageUrl` se devuelve como una URL absoluta pública construida con el esquema y host actuales.
3. **Dado** que la propiedad no tiene imagen asociada, **cuando** consulto la propiedad por id, **entonces** `imageUrl` se devuelve sin exponer rutas físicas internas.

---

### Historia de usuario 2 - Recibir error cuando la propiedad no existe (Prioridad: P2)

Como consumidor de la API, necesito recibir una respuesta clara cuando solicito una propiedad inexistente para saber que el recurso no está disponible.

**Por qué es prioritario**: Evita ambigüedad al diferenciar entre una propiedad encontrada y una que no existe.

**Prueba independiente**: Puede verificarse consultando el endpoint con un id válido que no corresponda a ninguna propiedad persistida y confirmando una respuesta `404` con `ProblemDetails`.

**Escenarios de aceptación**:

1. **Dado** que el id no corresponde a ninguna propiedad persistida, **cuando** consulto la propiedad por id, **entonces** recibo `404 Not Found` con un contrato `ProblemDetails`.
2. **Dado** que el recurso no existe, **cuando** consulto el endpoint, **entonces** no se expone ninguna entidad de persistencia ni metadatos de paginación.

---

### Historia de usuario 3 - Rechazar ids inválidos de forma consistente (Prioridad: P3)

Como consumidor de la API, necesito que los ids con formato inválido se rechacen explícitamente para corregir la solicitud sin confundir un error de formato con un recurso inexistente.

**Por qué es prioritario**: Mantiene el contrato público claro y permite distinguir errores de validación de errores de búsqueda.

**Prueba independiente**: Puede verificarse enviando un valor de id con formato inválido y confirmando la respuesta definida por el contrato público del proyecto.

**Escenarios de aceptación**:

1. **Dado** que el id no tiene formato válido, **cuando** consulto la propiedad por id, **entonces** recibo la respuesta definida por el contrato público para un id inválido.
2. **Dado** que el formato del id es inválido, **cuando** consulto el endpoint, **entonces** el sistema responde de forma consistente y documentada sin exponer detalles internos.

### Casos borde

- ¿Qué ocurre cuando el id no existe? La operación debe responder `404 Not Found` con `ProblemDetails`.
- ¿Qué ocurre cuando el id tiene formato inválido? La operación debe responder `400 Bad Request` con `ProblemDetails` según el contrato público documentado.
- ¿Qué ocurre cuando la propiedad no tiene imagen asociada? La respuesta debe mantener el contrato público sin rutas físicas internas; `imageUrl` debe ser cadena vacía.
- ¿Qué ocurre cuando el host o el esquema cambian por entorno? La `imageUrl` debe construirse con el esquema y host de la solicitud actual, manteniendo una URL absoluta pública.
- ¿Qué ocurre si la solicitud no debe incluir paginación? La operación no debe devolver metadatos de paginación.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer un endpoint público `GET /api/properties/{id}` para consultar una sola propiedad por id sin requerir autenticación o autorización.
- **FR-002**: El sistema DEBE buscar la propiedad por id antes de construir la respuesta.
- **FR-003**: Si la propiedad no existe, el sistema DEBE responder `404 Not Found` con `ProblemDetails`.
- **FR-004**: Si el id tiene formato inválido, el sistema DEBE responder `400 Bad Request` con `ProblemDetails` según el contrato público del proyecto.
- **FR-005**: El sistema DEBE devolver los campos públicos `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **FR-006**: El sistema DEBE construir `imageUrl` como una URL absoluta pública usando el esquema y host de la solicitud actual.
- **FR-007**: La `imageUrl` DEBE seguir el patrón `http(s)://{host}/assets/properties/{fileName}` cuando exista una imagen persistida.
- **FR-008**: Si la propiedad no tiene imagen asociada, el sistema DEBE devolver `imageUrl` como cadena vacía y NO DEBE exponer rutas físicas internas ni URLs relativas privadas.
- **FR-009**: El sistema NO DEBE devolver metadatos de paginación en esta operación.
- **FR-010**: El sistema NO DEBE devolver entidades EF ni modelos de persistencia directamente.
- **FR-011**: El sistema DEBE mantener la lógica de consulta en un slice dedicado con handler, mapping y contrato explícitos.
- **FR-012**: El sistema DEBE propagar el `CancellationToken` durante toda la ejecución del caso de uso.
- **FR-013**: El sistema DEBE ser auto-descubrible mediante `ISlice` y `IHandler` sin controllers.
- **FR-014**: El sistema DEBE documentar la validación manual del endpoint en `quickstart.md` y el contrato HTTP en `contracts/backend-properties-get-by-id.openapi.yaml`.

### Entidades clave

- **Propiedad**: Representa un inmueble persistido que puede consultarse individualmente; incluye los campos públicos expuestos al consumidor.
- **Detalle de propiedad**: Representa la proyección de lectura devuelta por el endpoint unitario, separada del modelo de persistencia.
- **Solicitud de consulta por id**: Representa el identificador de ruta usado para recuperar una propiedad específica.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las consultas con id existente devuelven `200 OK` con los campos públicos esperados.
- **SC-002**: El 100% de las consultas con id inexistente devuelven `404 Not Found` con `ProblemDetails`.
- **SC-003**: El 100% de las consultas con id inválido devuelven la respuesta definida por el contrato público del proyecto y no exponen detalles internos.
- **SC-004**: El 100% de las respuestas que incluyen imagen devuelven una `imageUrl` pública absoluta consumible por clientes externos.
- **SC-005**: En una muestra funcional de 100 consultas válidas, al menos el 95% completa la respuesta en 1 segundo o menos.

## Suposiciones

- Ya existen propiedades persistidas y consultables en la base de datos.
- La convención pública de imágenes bajo `/assets/properties` ya está disponible por las specs previas.
- El contrato de respuesta reutiliza los mismos campos públicos usados por el listado 004, pero sin metadatos de paginación.
- La validación manual del comportamiento se documentará en `quickstart.md`.
- La operación se limita a la consulta unitaria por id y no incluye creación, edición, paginación ni eliminación.
- El comportamiento para ids inválidos se documenta explícitamente en el contrato de esta spec y se implementa de forma consistente con el contrato público del proyecto.
