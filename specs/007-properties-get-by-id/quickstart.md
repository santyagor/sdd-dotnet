# Quickstart de validación: Consulta de propiedad por id

## Objetivo

Validar manualmente que `GET /api/properties/{id}` devuelve una única propiedad con los campos públicos esperados, construye una `imageUrl` absoluta y pública cuando existe imagen, y responde con los códigos definidos para ids inexistentes o inválidos.

## Requisitos previos

- .NET 11 SDK instalado.
- PostgreSQL disponible y configurado para el backend.
- La solución debe compilar correctamente.
- Debe existir al menos una propiedad persistida para probar la consulta por id.
- Debe existir acceso a la ruta pública de imágenes `app/backend/src/RealtorApi/wwwroot/assets/properties`.

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
- El endpoint de consulta queda disponible en `GET /api/properties/{id}`.

### 3. Obtener un id existente

Usar una propiedad existente creada previamente o consultarla desde el listado para copiar su `id`.

Resultado esperado:
- Se dispone de un `id` válido para probar la consulta unitaria.

### 4. Consultar una propiedad existente

Enviar una solicitud `GET /api/properties/{id}` usando un id válido.

Resultado esperado:
- Respuesta `200 OK`.
- Se devuelven `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- Si existe imagen persistida, `imageUrl` es absoluta y pública.

### 5. Consultar una propiedad sin imagen

Usar un id de una propiedad que no tenga imagen asociada.

Resultado esperado:
- Respuesta `200 OK`.
- El contrato se mantiene sin exponer rutas físicas internas.
- `imageUrl` queda como cadena vacía.

### 6. Consultar una propiedad inexistente

Enviar una solicitud con un id válido que no exista en la base de datos.

Resultado esperado:
- Respuesta `404 Not Found`.
- La respuesta sigue el contrato `ProblemDetails`.

### 7. Consultar con un id inválido

Enviar una solicitud con un valor de id con formato inválido según el contrato público.

Resultado esperado:
- Respuesta `400 Bad Request`.
- La respuesta sigue el contrato documentado para ids inválidos.

## Validación funcional adicional

- Verificar que no se devuelven metadatos de paginación.
- Verificar que el valor de `imageUrl` usa el esquema y host de la request actual.
- Verificar que la respuesta coincide con los mismos campos públicos usados por el listado 004.

## Validación de rendimiento

- Ejecutar una muestra funcional de 100 consultas válidas por id.
- Confirmar que al menos el 95% se resuelve en 1 segundo o menos.

## Resultados esperados finales

- La consulta unitaria devuelve una única propiedad con los campos públicos correctos.
- La imagen, cuando existe, se expone como URL pública absoluta.
- Los ids inexistentes e inválidos responden con el contrato definido.
- La operación no expone metadatos de paginación ni detalles internos de persistencia.
