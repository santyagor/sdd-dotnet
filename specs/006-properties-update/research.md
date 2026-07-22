# Investigación: Actualización de propiedades

## Decisión: Usar `PUT /api/properties/{id}` con `multipart/form-data`

- **Enfoque elegido**: Exponer la actualización como un `PUT` por id y recibir los campos editables junto con un archivo opcional en `multipart/form-data`.
- **Razonamiento**: `PUT` expresa reemplazo completo del recurso editable y el formato multipart permite enviar texto y binario en una sola solicitud, alineado con la solución creada en 005.
- **Alternativas consideradas**:
  - `PATCH` con campos opcionales: descartado porque la spec exige actualización completa de campos editables.
  - `application/json` + base64: descartado porque complica el manejo de imagen y aumenta el payload.
  - Dos solicitudes separadas: descartado porque rompe la atomicidad funcional del caso de uso.

## Decisión: Tratar la validación de negocio y la validación de imagen por separado

- **Enfoque elegido**: Validar los campos editables obligatorios con el validator del slice y dejar la validación específica de imagen en el flujo del handler para devolver los códigos de estado requeridos.
- **Razonamiento**: La validación global existente del proyecto devuelve `400`, pero la spec exige `415`, `413` y `500` para errores de archivo; separar responsabilidades permite mantener la semántica de errores sin alterar el contrato base de validación.
- **Alternativas consideradas**:
  - Extender el pipeline global de validación para múltiples códigos: descartado por impacto transversal excesivo para una sola feature.
  - Hacer toda la validación en el endpoint: descartado porque debilita la arquitectura de slice + validator.

## Decisión: Reutilizar la ruta pública de imágenes en `wwwroot/assets/properties`

- **Enfoque elegido**: Guardar la nueva imagen en `app/backend/src/RealtorApi/wwwroot/assets/properties` y persistir una URL pública/relativa bajo `/assets/properties`.
- **Razonamiento**: Es la convención ya usada por el alta de propiedades, facilita el consumo posterior por el listado y evita duplicar reglas de publicación de activos.
- **Alternativas consideradas**:
  - Crear una ruta distinta para update: descartado porque fragmentaría la convención de activos públicos.
  - Escribir en `support/seed-data`: descartado porque ese directorio no es para archivos de usuario.

## Decisión: Generar nombres internos únicos con UUID y conservar la extensión

- **Enfoque elegido**: Crear un nombre interno nuevo con UUID y conservar la extensión válida original de la imagen.
- **Razonamiento**: Evita colisiones, simplifica concurrencia y mantiene compatibilidad con clientes y navegadores.
- **Alternativas consideradas**:
  - Reusar el nombre original: descartado por riesgo de sobrescritura.
  - Usar nombres secuenciales: descartado por complejidad de coordinación.

## Decisión: Eliminar la imagen anterior tras una actualización exitosa

- **Enfoque elegido**: Guardar la nueva imagen, persistir la actualización en la base de datos y luego eliminar el archivo anterior.
- **Razonamiento**: Mantiene la actualización coherente para el consumidor y evita archivos huérfanos después de reemplazos repetidos.
- **Alternativas consideradas**:
  - Conservar todas las imágenes anteriores: descartado porque acumula archivos huérfanos.
  - Reemplazar el archivo en el mismo nombre físico: descartado porque rompe la inmutabilidad del nombre interno único.

## Decisión: Responder con `200 OK` y el recurso actualizado

- **Enfoque elegido**: Devolver el recurso actualizado con el estado final persistido.
- **Razonamiento**: El consumidor necesita confirmar el resultado y leer la nueva `imageUrl` cuando cambia, o verificar que se mantuvo la anterior cuando no se envía imagen.
- **Alternativas consideradas**:
  - `204 No Content`: descartado porque reduce visibilidad del estado final.
  - `201 Created`: descartado porque la operación es una actualización, no una creación.

## Decisión: No agregar migración de base de datos

- **Enfoque elegido**: Reutilizar el esquema existente de `Property` sin cambios de base de datos.
- **Razonamiento**: La actualización no introduce nuevas columnas ni relaciones; el modelo persistente ya soporta `ImageUrl` nula y el resto de campos editables ya existen.
- **Alternativas consideradas**:
  - Crear una migración nueva: descartado porque no aporta valor y complicaría el historial sin cambios de esquema.
