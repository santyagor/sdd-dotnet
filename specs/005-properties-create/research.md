# Investigación: Alta de propiedades

## Decisión: Usar `multipart/form-data` para la solicitud de alta

- **Enfoque elegido**: Recibir los datos de la propiedad y el archivo opcional en una sola solicitud `multipart/form-data`.
- **Razonamiento**: Este formato permite mezclar campos de texto y binarios sin codificar la imagen en base64, mantiene el payload eficiente y encaja de forma natural con la validación de tamaño y tipo.
- **Alternativas consideradas**:
  - `application/json` con base64: descartado porque incrementa el tamaño del payload y complica la validación.
  - Dos llamadas separadas: descartado porque rompe la simplicidad del caso de uso y deja la creación parcialmente distribuida.

## Decisión: Guardar la imagen en `wwwroot/assets/properties`

- **Enfoque elegido**: Persistir los archivos válidos en `app/backend/src/RealtorApi/wwwroot/assets/properties`.
- **Razonamiento**: La spec exige una ubicación pública dedicada y el catálogo posterior necesita una ruta estable para consumo web.
- **Alternativas consideradas**:
  - Reutilizar otra carpeta pública existente: descartado porque mezclará responsabilidades y dificultará la trazabilidad de activos.
  - Guardar en `support/seed-data`: descartado porque ese directorio es para datos de soporte, no para activos públicos del usuario.

## Decisión: Generar nombres únicos para evitar colisiones

- **Enfoque elegido**: Crear un nombre único nuevo para cada archivo, conservando la extensión original.
- **Razonamiento**: Evita sobrescrituras entre propiedades distintas y mantiene la compatibilidad con el tipo de archivo.
- **Alternativas consideradas**:
  - Conservar el nombre original: descartado por riesgo de colisión.
  - Usar nombres secuenciales: descartado porque requiere coordinación adicional y es menos robusto ante concurrencia.

## Decisión: Aceptar solo PNG/JPG y limitar a 5 MB

- **Enfoque elegido**: Validar extensiones y contenido esperado para PNG y JPG, y rechazar archivos mayores de 5 MB.
- **Razonamiento**: La especificación fija un formato controlado y un tamaño máximo claro para proteger almacenamiento y experiencia de consumo.
- **Alternativas consideradas**:
  - Permitir más formatos: descartado porque sale del alcance definido.
  - Solo validar extensión: descartado porque permite archivos corruptos o no compatibles.

## Decisión: Permitir `imageUrl` nula cuando no hay imagen

- **Enfoque elegido**: Hacer que la propiedad persistida acepte `imageUrl` nula si el usuario no adjunta imagen.
- **Razonamiento**: Es la regla explícita del caso de uso P1 y permite completar altas rápidas sin recurso visual.
- **Alternativas consideradas**:
  - Obligatoria siempre: descartado porque contradice la spec.
  - Rellenar con placeholder fijo: descartado porque introduce semántica no solicitada.

## Decisión: Responder con confirmación de creación

- **Enfoque elegido**: Devolver una respuesta de creación con la información esencial de la propiedad registrada.
- **Razonamiento**: El cliente necesita confirmar el alta y usar inmediatamente el identificador y la referencia de imagen si existe.
- **Alternativas consideradas**:
  - Responder solo con código vacío: descartado porque reduce utilidad para el consumidor.
  - Devolver el listado completo: descartado porque no forma parte del contrato de alta.
