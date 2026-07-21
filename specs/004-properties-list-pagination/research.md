# Investigación: Paginación de listado de propiedades

## Decisión: Implementar el caso de uso como un slice dedicado

- **Enfoque elegido**: Crear `Features/Properties/ListProperties` con un slice concreto para el endpoint `GET /api/properties`.
- **Razonamiento**: El repositorio ya usa descubrimiento centralizado de `ISlice`, por lo que el caso de uso encaja naturalmente en una feature aislada y no requiere controllers ni registros manuales.
- **Alternativas consideradas**:
  - Endpoint manual en `Program.cs`: descartado porque rompe el patrón de slices y centralización.
  - Agrupar varios casos de uso de propiedades en un único archivo genérico: descartado porque reduce claridad y dificulta el mantenimiento por caso de uso.

## Decisión: Validar `page` y `pageSize` con FluentValidation

- **Enfoque elegido**: Definir un request DTO de consulta y un validator específico para garantizar que `page` y `pageSize` sean mayores que cero.
- **Razonamiento**: La base del backend ya incluye validación automática con `ValidationFilterFactory`, así que la forma más consistente es aprovechar el mecanismo existente y no validar manualmente dentro del handler.
- **Alternativas consideradas**:
  - Validación manual dentro del handler: descartada porque duplica lógica y contradice la infraestructura de validación automática.
  - Reglas ad hoc sin validator: descartado porque impide reutilizar el pipeline estándar de `ValidationProblemDetails`.

## Decisión: Leer con EF Core y proyectar al contrato de respuesta

- **Enfoque elegido**: Consultar `AppDbContext.Properties` con `AsNoTracking()`, aplicar `OrderBy(p => p.Title)`, ejecutar `CountAsync()` para el total y luego materializar la página mediante `Skip()` y `Take()`.
- **Razonamiento**: La spec pide una lectura eficiente y explícita sin devolver entidades de persistencia; la proyección directa al contrato minimiza transferencia y evita exponer el modelo interno.
- **Alternativas consideradas**:
  - Cargar todas las propiedades y paginar en memoria: descartado por ineficiente y poco escalable.
  - Devolver entidades EF y mapear en el cliente: descartado porque viola el contrato de separación entre dominio y response.

## Decisión: Construir `imageUrl` absoluta con base en la solicitud actual

- **Enfoque elegido**: Tomar el valor persistido de la imagen, extraer el nombre de archivo y construir la URL como `scheme://host/assets/properties/{fileName}`.
- **Razonamiento**: La spec exige una URL pública absoluta y prohíbe rutas físicas o relativas; derivarla desde el host del request asegura que funcione en local, staging y producción sin hardcodear dominios.
- **Alternativas consideradas**:
  - Usar una URL fija en configuración: descartado porque no responde al host actual y complica despliegues.
  - Devolver la ruta relativa almacenada en base de datos: descartado por prohibición explícita de la spec.

## Decisión: Servir imágenes bajo `/assets/properties`

- **Enfoque elegido**: Exponer el directorio físico `wwwroot/images/properties` con una ruta pública `/assets/properties`.
- **Razonamiento**: La funcionalidad de lectura requiere una URL pública estable bajo un prefijo claro, y esta ruta cumple exactamente el contrato esperado sin cambiar la persistencia existente.
- **Alternativas consideradas**:
  - Mantener `/images/properties`: descartado porque la spec fija el formato esperado como `/assets/properties/{fileName}`.
  - Exponer rutas físicas de `support/seed-data`: descartado por ser información interna del servidor.

## Decisión: Mantener el alcance solo en lectura paginada

- **Enfoque elegido**: Limitar la implementación al listado paginado y a sus contratos asociados.
- **Razonamiento**: La spec prohíbe introducir funcionalidades fuera del listado paginado y no solicita CRUD ni otras operaciones.
- **Alternativas consideradas**:
  - Añadir filtros, búsqueda o ordenación adicional: descartado porque no forman parte del alcance.
  - Introducir endpoints extra de detalle: descartado porque pertenece a otra spec futura.
