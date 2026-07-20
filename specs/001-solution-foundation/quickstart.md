# Quickstart: Validación de la Fundación de Solución Realtor

## Prerrequisitos

- .NET 11 SDK instalado en la máquina.
- Acceso al repositorio clonado en `d:\ProyGitPersonal\sdd`.
- No se requiere base de datos ni servicios externos en esta fase.

## Pasos de validación

1. Abrir una terminal en la raíz del repositorio:
   ```powershell
   cd /d d:\ProyGitPersonal\sdd
   ```
2. Ejecutar la compilación de la solución desde `app/`:
   ```powershell
   dotnet build app/
   ```
   - Resultado esperado: compilación exitosa sin errores.

3. Ejecutar los tests desde `app/`:
   ```powershell
   dotnet test app/
   ```
   - Resultado esperado: todos los proyectos de test descubiertos y los placeholders pasan.

4. Abrir la solución en Visual Studio o Visual Studio Code:
   - Verificar que `app/Realtor.sln` contiene:
     - `app/backend/src/RealtorApi/RealtorApi.csproj`
     - `app/backend/tests/RealtorApiTests/RealtorApiTests.csproj`
     - `app/frontend/src/RealtorWeb/RealtorWeb.csproj`
     - `app/frontend/test/RealtorWeb/RealtorWeb.Tests.csproj`

5. Revisar la estructura de folders:
   - `app/backend/src/RealtorApi/`
   - `app/backend/tests/RealtorApiTests/`
   - `app/frontend/src/RealtorWeb/`
   - `app/frontend/test/RealtorWeb/`

## Qué validar

- La solución usa la versión de SDK de `global.json`.
- El backend es un Minimal API sin carpeta `Controllers/`.
- El frontend es un proyecto Blazor Web App con `wwwroot/app.css` como CSS principal.
- Los tests placeholder existen y se ejecutan.

## Referencias

- `specs/001-solution-foundation/spec.md`
- `specs/001-solution-foundation/plan.md`
- `specs/001-solution-foundation/tasks.md`
