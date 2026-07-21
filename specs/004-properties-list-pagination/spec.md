# Especificación de la funcionalidad: Paginación de listado de propiedades

**Feature Branch**: `004-properties-list-pagination`

**Created**: 2026-07-20

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Crear la especificación 004-properties-list-pagination."

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Listar propiedades paginadas (Prioridad: P1)

Como consumidor de la API, necesito consultar un listado paginado de propiedades para mostrar un catálogo navegable en una interfaz.

**Por qué es prioritario**: Es la capacidad principal del alcance y permite consumir el catálogo existente de propiedades de forma controlada.

**Prueba independiente**: Puede verificarse invocando el listado sin parámetros y confirmando que devuelve la primera página, ordenada alfabéticamente por título, con un máximo de seis elementos.

**Escenarios de aceptación**:

1. **Dado** que existen propiedades persistidas, **cuando** consulto `GET /api/properties` sin parámetros, **entonces** recibo la primera página con hasta seis propiedades ordenadas por título ascendente.
2. **Dado** que existen más de seis propiedades, **cuando** consulto `GET /api/properties?page=2&pageSize=6`, **entonces** recibo la segunda página con metadatos consistentes con el total disponible.

---

### Historia de usuario 2 - Validar parámetros de paginación (Prioridad: P2)

Como consumidor de la API, necesito recibir errores claros cuando envío parámetros de paginación inválidos para corregir la solicitud sin ambigüedad.

**Por qué es prioritario**: Evita resultados inconsistentes y permite que el contrato de consulta sea predecible y robusto.

**Prueba independiente**: Puede verificarse enviando valores inválidos en `page` o `pageSize` y confirmando una respuesta de validación con estado 400.

**Escenarios de aceptación**:

1. **Dado** que `page` es menor o igual a cero, **cuando** consulto el endpoint, **entonces** recibo una respuesta 400 con detalles de validación.
2. **Dado** que `pageSize` es menor o igual a cero, **cuando** consulto el endpoint, **entonces** recibo una respuesta 400 con detalles de validación.

---

### Historia de usuario 3 - Exponer imágenes públicas absolutas (Prioridad: P3)

Como consumidor de la API, necesito que cada propiedad incluya la dirección pública absoluta de su imagen para poder renderizarla directamente en una interfaz.

**Por qué es prioritario**: El listado debe ser utilizable de forma inmediata por clientes web sin exponer rutas internas del servidor.

**Prueba independiente**: Puede verificarse consultando el listado y comprobando que cada propiedad devuelve una URL absoluta pública para la imagen.

**Escenarios de aceptación**:

1. **Dado** que una propiedad tiene imagen persistida, **cuando** consulto el listado, **entonces** el campo `imageUrl` contiene una URL pública absoluta accesible por la API.
2. **Dado** un host local como `http://localhost:5023`, **cuando** consulto el listado, **entonces** la URL de imagen sigue el patrón `http://localhost:5023/assets/properties/{fileName}`.

### Casos borde

- ¿Qué ocurre cuando la página solicitada excede el total disponible? La respuesta debe ser consistente y devolver una colección vacía con metadatos correctos.
- ¿Qué ocurre cuando no existen propiedades persistidas? El endpoint debe responder con una página vacía y metadatos que reflejen cero elementos.
- ¿Qué ocurre cuando una propiedad no tiene imagen asociada? El contrato debe evitar rutas internas y mantener un valor público válido o una respuesta coherente según el modelo persistido.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer un listado paginado de propiedades mediante `GET /api/properties`.
- **FR-002**: El sistema DEBE aceptar los parámetros de consulta `page` y `pageSize`.
- **FR-003**: Si no se envían parámetros de paginación, el sistema DEBE usar `page = 1` y `pageSize = 6`.
- **FR-004**: El sistema DEBE ordenar los resultados por `title` en orden ascendente.
- **FR-005**: El sistema DEBE devolver, por cada propiedad, los campos `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **FR-006**: El sistema DEBE devolver metadatos de paginación con `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`.
- **FR-007**: El sistema DEBE validar que `page` y `pageSize` sean mayores que cero.
- **FR-008**: Si la validación falla, el sistema DEBE responder con estado 400 y un detalle de validación comprensible.
- **FR-009**: El sistema DEBE construir `imageUrl` como una URL pública absoluta basada en el host de la solicitud actual.
- **FR-010**: El sistema NO DEBE exponer rutas físicas internas del servidor en la respuesta del listado.
- **FR-011**: El sistema NO DEBE devolver entidades de persistencia directamente en la respuesta.
- **FR-012**: El sistema DEBE propagar el `CancellationToken` durante toda la ejecución del caso de uso.
- **FR-013**: El sistema DEBE mantener la lógica de consulta en un slice de features y no en controllers.
- **FR-014**: El sistema DEBE incluir pruebas manuales documentadas para `GET /api/properties` y `GET /api/properties?page=1&pageSize=6` en `RealtorApi.http`.

### Entidades clave

- **Propiedad**: Representa un inmueble disponible para consulta; incluye datos descriptivos, precio, estado e imagen pública.
- **Paginación de propiedades**: Representa el subconjunto de resultados devueltos para una página concreta junto con sus metadatos.
- **Elemento de respuesta de propiedad**: Representa la proyección de lectura expuesta al consumidor de la API, separada del modelo de persistencia.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las consultas sin parámetros devuelven la primera página con un máximo de seis elementos y orden consistente por título.
- **SC-002**: Al consultar una segunda página válida, los metadatos de paginación coinciden con el total disponible en al menos el 99% de los casos verificados.
- **SC-003**: El 100% de las solicitudes con parámetros inválidos reciben una respuesta 400 con detalles de validación.
- **SC-004**: El 100% de las propiedades devueltas incluyen una `imageUrl` pública absoluta utilizable por clientes web.
- **SC-005**: Al menos el 95% de los usuarios de prueba completan la consulta del catálogo sin necesitar consultar documentación adicional.

## Suposiciones

- Ya existen propiedades persistidas y disponibles para consulta.
- Las imágenes de propiedad se sirven públicamente bajo la ruta `/assets/properties`.
- El contrato de respuesta no incluirá campos adicionales no solicitados por este alcance.
- La funcionalidad se limita al listado paginado; la creación, edición y eliminación de propiedades quedan fuera de esta spec.
- El cliente consumidor construye la navegación usando los metadatos devueltos por la API.
