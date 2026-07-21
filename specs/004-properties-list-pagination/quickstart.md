# Quickstart de validación: Paginación de listado de propiedades

## Objetivo

Validar manualmente que el endpoint `GET /api/properties` devuelve propiedades paginadas, ordenadas por `title` y con `imageUrl` absoluta pública.

## Requisitos previos

- .NET 11 SDK instalado.
- PostgreSQL disponible y configurado para el backend.
- Las propiedades seed de la spec 003 deben existir en la base de datos.
- El backend `app/backend/src/RealtorApi` debe compilar correctamente.

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
- La aplicación arranca sin errores.
- Las imágenes públicas quedan disponibles bajo `/assets/properties`.

### 3. Probar el listado por defecto

Abrir `app/backend/src/RealtorApi/RealtorApi.http` y ejecutar:

- `GET /api/properties`

Resultado esperado:
- Respuesta `200 OK`.
- `page = 1`.
- `pageSize = 6`.
- Los resultados vienen ordenados por `title` ascendente.
- Cada elemento incluye `imageUrl` absoluta.

### 4. Probar una página explícita

Ejecutar:

- `GET /api/properties?page=1&pageSize=6`
- `GET /api/properties?page=2&pageSize=6`

Resultado esperado:
- Respuesta `200 OK`.
- Los metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious` son coherentes.

### 5. Probar validación de parámetros

Ejecutar una request inválida, por ejemplo:

- `GET /api/properties?page=0&pageSize=6`
- `GET /api/properties?page=1&pageSize=0`

Resultado esperado:
- Respuesta `400 Bad Request`.
- El payload contiene `ValidationProblemDetails` o un contrato equivalente de validación.

### 6. Verificar la URL pública de imagen

Comprobar que la respuesta contiene URLs con este formato:

- `http://localhost:5023/assets/properties/{fileName}`

Resultado esperado:
- La URL es absoluta.
- No aparecen rutas físicas del servidor.
- No aparecen rutas relativas en la respuesta final.

## Resultados esperados finales

- El endpoint expone propiedades paginadas correctamente.
- La paginación respeta el tamaño por defecto y el orden por título.
- La validación automática responde con 400 para parámetros inválidos.
- Cada propiedad devuelve una imagen pública absoluta servible por la API.
