# Investigación: Consulta de propiedad por id

## Decisión: Reutilizar el contrato de lectura pública de la spec 004

- **Enfoque elegido**: Exponer un endpoint unitario que devuelva exactamente los campos públicos de lectura usados en el listado 004, sin metadatos de paginación.
- **Razonamiento**: Mantiene consistencia entre el catálogo y el detalle, reduce duplicación de contratos y facilita el consumo por clientes externos.
- **Alternativas consideradas**:
  - Crear un contrato nuevo con campos adicionales: descartado porque introduce divergencia innecesaria.
  - Reutilizar la entidad EF directamente: descartado porque expone el modelo de persistencia.

## Decisión: Construir `imageUrl` como URL absoluta pública con esquema y host actuales

- **Enfoque elegido**: Formar `imageUrl` con `scheme://host/assets/properties/{fileName}` usando el esquema y host de la request actual.
- **Razonamiento**: Es el mismo criterio de exposición pública usado por el catálogo paginado y permite que clientes externos consuman la imagen sin conocer rutas internas.
- **Alternativas consideradas**:
  - Devolver rutas relativas: descartado porque rompe la consumibilidad externa.
  - Exponer rutas físicas del servidor: descartado por seguridad y acoplamiento a infraestructura.

## Decisión: Representar ausencia de imagen con cadena vacía

- **Enfoque elegido**: Cuando una propiedad no tenga imagen asociada, devolver `imageUrl` como cadena vacía.
- **Razonamiento**: Mantiene el contrato explícito del recurso unitario y evita confundir la ausencia de imagen con una ruta relativa o interna.
- **Alternativas consideradas**:
  - `null`: descartado porque el contrato debe ser explícito para clientes y OpenAPI.
  - Omitir el campo: descartado porque rompe la forma de respuesta consistente con 004.

## Decisión: Tratar el id inválido como error de validación pública

- **Enfoque elegido**: Responder `400 Bad Request` con `ProblemDetails` cuando el id de ruta no tenga formato válido.
- **Razonamiento**: Distingue un error de formato de un recurso inexistente y mantiene el contrato público documentado.
- **Alternativas consideradas**:
  - Responder `404 Not Found`: descartado porque mezcla validación con ausencia de recurso.
  - Responder `422 Unprocessable Entity`: descartado porque no coincide con el contrato público establecido por el proyecto.

## Decisión: Mantener una consulta unitaria sin paginación

- **Enfoque elegido**: No incluir metadatos de paginación ni wrappers adicionales en la respuesta.
- **Razonamiento**: La operación recupera un único recurso y debe ser más simple que el listado.
- **Alternativas consideradas**:
  - Reusar el envelope paginado del listado: descartado porque agrega ruido innecesario.
  - Agregar metadatos de navegación: descartado porque no aplica a una consulta unitaria.

## Decisión: Estructurar la feature como Vertical Slice

- **Enfoque elegido**: Implementar la consulta con slice, handler, mapping y validación explícita, auto-descubiertos por `ISlice` y `IHandler`.
- **Razonamiento**: Es la arquitectura canónica del backend y asegura coherencia con las specs previas.
- **Alternativas consideradas**:
  - Controllers: descartado por la constitución del proyecto.
  - Servicios globales: descartado por romper la organización por features.

## Decisión: No agregar migración de base de datos

- **Enfoque elegido**: Reutilizar el esquema existente de `Property`.
- **Razonamiento**: La consulta es de solo lectura y no requiere cambios de esquema.
- **Alternativas consideradas**:
  - Crear una migración nueva: descartado porque no agrega valor funcional.
