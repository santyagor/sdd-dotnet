# quickstart.md

## Objetivo

Validar rápidamente el diseño de la persistencia y el seeding de propiedades una vez implementado.

## Requisitos previos

- .NET 11 SDK instalado.
- PostgreSQL disponible y accesible.
- Variables de configuración de conexión definidas en `app/backend/src/RealtorApi/appsettings.json` o `appsettings.Development.json`.
- El proyecto `app/backend/src/RealtorApi` debe estar en la solución existente.

## Pasos de validación

1. Construir el backend:

```powershell
cd d:\ProyGitPersonal\sdd\app\backend\src\RealtorApi
dotnet build
```

2. Verificar que el plan y los artefactos de diseño existen:

```powershell
Get-ChildItem ..\..\..\specs\003-properties-persistence-seeding\
```

Debe incluir:
- `plan.md`
- `research.md`
- `data-model.md`
- `quickstart.md`
- `spec.md`

3. Ejecutar las pruebas de diseño y persistencia:

```powershell
cd d:\ProyGitPersonal\sdd
dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj --no-restore
```

Resultado esperado:
- Todas las pruebas del backend pasan.
- No hay errores de compilación.

4. Validar la integración de los archivos de seed:

- `support/seed-data/properties.json`
- `support/seed-data/properties-statuses.json`
- `support/seed-data/seed-manifest.json`
- `support/seed-data/properties/` (imágenes)

5. Validar el comportamiento de arranque:

- Ejecutar la aplicación:

```powershell
cd d:\ProyGitPersonal\sdd\app\backend\src\RealtorApi
dotnet run
```

- Verificar en logs que `MigrateAsync` y el seeding async se ejecutan antes de `app.Run()`.
- Confirmar que el mensaje de arranque no muestra errores de migración o seeding.

6. Validar la ruta pública de imagen después de la copia de assets:

- Confirmar que las imágenes del manifiesto están disponibles en el destino público bajo `wwwroot/images/properties` o la ruta configurada.
- La URL pública en `Property.ImageUrl` debe tener formato `/images/properties/{filename}`.

## Resultados esperados

- El backend compila y arranca correctamente.
- El seeding lee los JSON y el manifiesto desde `support/seed-data`.
- Las imágenes se copian o sincronizan al destino público final.
- `ImageUrl` persiste como ruta pública de API.
- Repetir el arranque no duplica datos.
