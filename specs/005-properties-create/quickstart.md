# Quickstart de validación: Alta de propiedades

## Objetivo

Validar manualmente que `POST /api/properties` permite crear propiedades con y sin imagen, valida correctamente el archivo opcional y persiste la imagen en la ubicación pública prevista.

## Requisitos previos

- .NET 11 SDK instalado.
- PostgreSQL disponible y configurado para el backend.
- La solución debe compilar correctamente.
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
- El endpoint de alta queda disponible en `POST /api/properties`.

### 3. Crear una propiedad sin imagen

Enviar una solicitud `multipart/form-data` con todos los campos de la propiedad y sin archivo de imagen.

Resultado esperado:
- Respuesta exitosa.
- La propiedad se persiste correctamente.
- `imageUrl` queda en `null`.

### 4. Crear una propiedad con imagen válida

Enviar una solicitud `multipart/form-data` con los campos de la propiedad y una imagen PNG o JPG menor o igual a 5 MB.

Resultado esperado:
- Respuesta exitosa.
- La imagen se almacena en `app/backend/src/RealtorApi/wwwroot/assets/properties`.
- La propiedad persiste con una ruta pública/relativa de imagen.

### 5. Validar un archivo no permitido

Enviar una solicitud con una imagen de formato no admitido o con tamaño superior a 5 MB.

Resultado esperado:
- Respuesta `400 Bad Request`.
- La propiedad no se persiste.
- El archivo no se almacena.

### 6. Verificar la consistencia posterior

Consultar la base de datos o la respuesta de creación para confirmar que la propiedad creada es consumible por el catálogo posterior.

Resultado esperado:
- La propiedad queda lista para el listado.
- La referencia de imagen es compatible con el consumo posterior.

## Resultados esperados finales

- El alta sin imagen funciona correctamente.
- El alta con imagen válida funciona correctamente.
- Los archivos no permitidos se rechazan.
- La creación no deja estados parciales.
