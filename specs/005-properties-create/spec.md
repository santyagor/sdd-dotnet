# Especificación de la funcionalidad: Alta de propiedades

**Feature Branch**: `005-properties-create`

**Created**: 2026-07-20

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Crear la especificación 005-properties-create para definir un nuevo caso de uso de alta de propiedades mediante un slice Minimal API en backend. La especificación debe cubrir un endpoint público de creación que reciba los datos de la propiedad y permita de forma opcional el upload de imagen. Si no se envía imagen, la propiedad debe persistirse correctamente con imageUrl en null. Si se envía imagen válida, debe almacenarse en app/backend/src/RealtorApi/wwwroot/assets/properties y persistirse la URL pública/relativa correspondiente en la propiedad para su consumo posterior por clientes. Incluir criterios de validación funcional del upload (formatos PNG/JPG y tamaño máximo 5MB), reglas de respuesta y errores esperados, escenarios independientes por prioridad (P1/P2/P3), requisitos funcionales testables y criterios de éxito medibles. Mantener alineación con la arquitectura existente de Vertical Slice (slice + handler + mapping + validator), auto-registro por IHandler e ISlice, y mapeos explícitos por slice sin lógica de negocio en mapping. Respetar gobernanza Spec-Driven del repositorio y dependencias con specs previas de foundation, persistencia y listado."

## Clarificaciones

### Session 2026-07-20

- Q: Formato de la solicitud de alta → A: `multipart/form-data` con campos de texto y archivo opcional.
- Q: Acceso al endpoint de alta → A: Sin autenticación; el endpoint permanece público.
- Q: Nombre del archivo de imagen → A: Generar un nombre único nuevo y conservar la extensión original.

## Escenarios de usuario y pruebas *(obligatorio)*

### Historia de usuario 1 - Crear una propiedad sin imagen (Prioridad: P1)

Como usuario del sistema, necesito registrar una nueva propiedad mediante `POST /api/properties` sin adjuntar imagen para completar altas rápidas cuando todavía no dispongo del recurso visual.

**Por qué es prioritario**: Es el flujo mínimo de valor y debe funcionar aunque el archivo de imagen no esté disponible.

**Prueba independiente**: Puede verificarse creando una propiedad con todos los datos requeridos y sin imagen; el sistema debe confirmar la creación y persistir la propiedad con `imageUrl` en null.

**Escenarios de aceptación**:

1. **Dado** que envío un conjunto válido de datos de propiedad sin imagen, **cuando** confirmo el alta, **entonces** la propiedad se persiste correctamente, `imageUrl` queda en null y la respuesta confirma la creación.
2. **Dado** que la solicitud contiene todos los campos obligatorios, **cuando** el sistema responde, **entonces** recibo una confirmación de creación con estado exitoso y los datos principales de la propiedad.

---

### Historia de usuario 2 - Crear una propiedad con imagen válida (Prioridad: P2)

Como usuario del sistema, necesito registrar una propiedad mediante `POST /api/properties` con su imagen para que el catálogo pueda mostrarse completo desde el primer momento.

**Por qué es prioritario**: La imagen es una parte importante de la presentación del catálogo, pero no bloquea el alta básica.

**Prueba independiente**: Puede verificarse creando una propiedad con una imagen válida en formato admitido y tamaño permitido; el sistema debe guardar el archivo y persistir la ruta pública/relativa correspondiente.

**Escenarios de aceptación**:

1. **Dado** que envío datos válidos y una imagen PNG o JPG dentro del tamaño permitido, **cuando** confirmo el alta, **entonces** la imagen se almacena en la ubicación pública prevista y la propiedad persiste con la referencia correspondiente.
2. **Dado** que la creación con imagen finaliza correctamente, **cuando** consulto la propiedad creada posteriormente, **entonces** la referencia de imagen permite su consumo público por clientes.

---

### Historia de usuario 3 - Rechazar altas inválidas y archivos no permitidos (Prioridad: P3)

Como usuario del sistema, necesito recibir errores claros cuando envío `POST /api/properties` con datos incompletos o imágenes no válidas para corregir la solicitud sin crear registros defectuosos.

**Por qué es prioritario**: Evita datos inconsistentes y protege el catálogo de archivos no soportados.

**Prueba independiente**: Puede verificarse enviando solicitudes con campos obligatorios faltantes, archivos no permitidos o imágenes que superan el tamaño máximo; el sistema debe rechazar la creación con errores comprensibles.

**Escenarios de aceptación**:

1. **Dado** que falta algún dato obligatorio de la propiedad, **cuando** intento crearla, **entonces** recibo un error de validación y no se persiste ningún registro.
2. **Dado** que envío una imagen con formato no admitido o superior a 5 MB, **cuando** intento crear la propiedad, **entonces** recibo un error de validación y no se almacena el archivo.

### Casos borde

- ¿Qué ocurre si no se envía ningún archivo de imagen? La creación debe completarse correctamente y la referencia de imagen debe permanecer nula.
- ¿Qué ocurre si la imagen tiene extensión admitida pero contenido no válido? La solicitud debe rechazarse con un error claro y no debe persistirse el archivo.
- ¿Qué ocurre si la carga de imagen supera el tamaño máximo? La solicitud debe rechazarse antes de completar la creación.
- ¿Qué ocurre si la ruta de almacenamiento no está disponible temporalmente? El sistema debe responder con un error controlado y no dejar la propiedad en un estado parcial.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **FR-001**: El sistema DEBE exponer un endpoint público `POST /api/properties` para crear propiedades sin requerir autenticación o autorización.
- **FR-002**: El sistema DEBE aceptar la solicitud de alta mediante `multipart/form-data` con campos de texto y archivo opcional.
- **FR-003**: El sistema DEBE permitir crear una propiedad sin imagen y persistir `imageUrl` en null.
- **FR-004**: El sistema DEBE permitir adjuntar una imagen opcional al alta de la propiedad.
- **FR-005**: El sistema DEBE aceptar únicamente imágenes cuya validación confirme que el contenido corresponde a PNG o JPG.
- **FR-006**: El sistema DEBE rechazar imágenes cuyo tamaño supere 5 MB.
- **FR-007**: Si la imagen es válida, el sistema DEBE almacenar el archivo en una ubicación pública dedicada para propiedades usando un nombre único y conservando la extensión original.
- **FR-008**: Si la imagen es válida, el sistema DEBE persistir en la propiedad la ruta pública/relativa correspondiente al archivo almacenado.
- **FR-009**: El sistema DEBE responder con una confirmación de creación con estado exitoso cuando la solicitud sea válida.
- **FR-010**: El sistema DEBE devolver errores de validación claros cuando falten datos obligatorios o cuando el archivo no cumpla las reglas.
- **FR-011**: El sistema NO DEBE persistir una propiedad parcial cuando la validación falle.
- **FR-012**: El sistema NO DEBE almacenar archivos no permitidos.
- **FR-013**: El sistema DEBE mantener la lógica de creación en un slice dedicado con handler, validator y mapping explícitos.
- **FR-014**: El sistema DEBE integrar el nuevo alta con el modelo de datos existente para que la propiedad quede disponible para consumo posterior en el catálogo.

### Entidades clave

- **Propiedad**: Representa un inmueble registrado por el sistema; incluye título, descripción, dirección, precio, estado, dimensiones y referencia de imagen.
- **Solicitud de alta de propiedad**: Representa los datos que un usuario envía para crear una nueva propiedad, con una imagen opcional.
- **Imagen de propiedad**: Representa el archivo cargado para enriquecer la presentación pública de la propiedad.
- **Confirmación de creación**: Representa la respuesta de éxito que informa al consumidor que la propiedad quedó registrada correctamente.

## Criterios de éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las altas válidas sin imagen completan la creación con una confirmación exitosa y dejan la referencia de imagen en null.
- **SC-002**: El 100% de las altas válidas con imagen PNG o JPG de hasta 5 MB almacenan el archivo y dejan una referencia pública/relativa utilizable.
- **SC-003**: El 100% de las solicitudes inválidas reciben un error comprensible y no crean registros parciales.
- **SC-004**: Al menos el 95% de las altas válidas pueden completarse en una sola solicitud sin correcciones posteriores por validaciones previsibles.
- **SC-005**: El 100% de las propiedades creadas quedan disponibles para consulta posterior en el catálogo sin intervención manual adicional.

## Suposiciones

- Ya existen la base de la solución, el modelo persistente y el catálogo de listado de propiedades definidos por las specs previas.
- La nueva funcionalidad reutiliza la arquitectura de Vertical Slice ya establecida en el backend.
- La referencia persistida de imagen será una ruta pública/relativa consumible por el catálogo posterior.
- Cuando exista imagen, el archivo se guardará con un nombre único para evitar colisiones entre propiedades.
- La imagen no es obligatoria para completar el alta.
- El alcance de esta spec se limita al alta de propiedades y no incluye edición, borrado ni cambios en el catálogo más allá de la visibilidad posterior de los datos creados.
