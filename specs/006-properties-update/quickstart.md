# Quickstart de validación: Actualización de propiedades

## Objetivo

Validar manualmente que `PUT /api/properties/{id}` permite actualizar propiedades con y sin imagen, preserva la imagen existente cuando no se envía archivo y responde con los códigos HTTP definidos para errores de validación, formato, tamaño y almacenamiento.

## Requisitos previos

- .NET 11 SDK instalado.
- PostgreSQL disponible y configurado para el backend.
- La solución debe compilar correctamente.
- Debe existir una propiedad previa creada para poder probar la actualización.
- Debe existir acceso de escritura al directorio `app/backend/src/RealtorApi/wwwroot/assets/properties`.

## Pasos de validación

### 1. Construir el backend

```powershell
cd d:\ProyGitPersonal\sdd\app\backend\src\RealtorApi
dotnet build
```

Resultado esperado:
- La compilación finaliza sin errores.

### 2. Ejecutar la aplicación

```powershell
cd d:\ProyGitPersonal\sdd\app\backend\src\RealtorApi
dotnet run
```

Resultado esperado:
- La aplicación arranca correctamente.
- El endpoint de actualización queda disponible en `PUT /api/properties/{id}`.

### 3. Preparar una propiedad existente

Crear o identificar una propiedad existente mediante el flujo de alta previo y anotar su `id`.

Resultado esperado:
- Existe un `id` válido para realizar la prueba de actualización.

### 4. Actualizar una propiedad sin imagen nueva

Enviar una solicitud `multipart/form-data` con todos los campos editables válidos y sin archivo de imagen.

Resultado esperado:
- Respuesta exitosa `200 OK`.
- Los campos de negocio se actualizan.
- La `imageUrl` anterior permanece sin cambios.

### 5. Actualizar una propiedad con imagen válida

Enviar una solicitud `multipart/form-data` con todos los campos editables válidos y una imagen PNG o JPG menor o igual a 5 MB.

Resultado esperado:
- Respuesta exitosa `200 OK`.
- La imagen se almacena en `app/backend/src/RealtorApi/wwwroot/assets/properties`.
- La propiedad persiste con una nueva `imageUrl` pública/relativa.
- La imagen anterior se elimina tras la actualización exitosa.

### 6. Validar una propiedad inexistente

Enviar una solicitud con un `id` que no exista.

Resultado esperado:
- Respuesta `404 Not Found`.
- No se modifica ningún dato.

### 7. Validar un archivo no permitido o vacío

Enviar una solicitud con una imagen vacía, con formato no admitido o con contenido real inválido.

Resultado esperado:
- Respuesta `415 Unsupported Media Type`.
- La actualización no se persiste.
- No quedan archivos parciales.

### 8. Validar una imagen demasiado grande

Enviar una solicitud con una imagen que supere 5 MB.

Resultado esperado:
- Respuesta `413 Payload Too Large`.
- La actualización no se persiste.

### 9. Validar un fallo interno de almacenamiento

Provocar un fallo de escritura en el directorio de imágenes mediante un entorno sin permisos o una ruta temporal no escribible.

Resultado esperado:
- Respuesta `500 Internal Server Error`.
- No queda una actualización parcial.

## Validación funcional adicional

- Verificar que la referencia de imagen no cambia cuando no se envía archivo.
- Verificar que la referencia de imagen sí cambia cuando se envía una nueva imagen válida.
- Verificar que las actualizaciones válidas siguen siendo visibles desde el listado posterior.

## Validación de rendimiento

- Ejecutar una muestra funcional de 100 solicitudes válidas de actualización sobre propiedades existentes.
- Confirmar que la latencia p95 se mantiene dentro de 2 segundos o menos.

## Resultados esperados finales

- La actualización sin imagen conserva la `imageUrl` previa.
- La actualización con imagen válida reemplaza la imagen y persiste la nueva referencia.
- Los errores de id inexistente, formato, tamaño y almacenamiento responden con el código esperado.
- La operación no deja estados parciales.
