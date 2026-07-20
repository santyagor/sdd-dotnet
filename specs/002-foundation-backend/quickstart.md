# Quickstart: Validación de la Fundación del Backend RealtorApi

## Prerrequisitos

- .NET 11 SDK instalado.
- Repositorio clonado en `d:\ProyGitPersonal\sdd`.
- No se requiere base de datos ni servicios externos.

## Pasos de validación

1. Abrir terminal en la raíz del repositorio:
   ```powershell
   cd /d d:\ProyGitPersonal\sdd
   ```
2. Ejecutar compilación de la solución desde `app/`:
   ```powershell
   dotnet build app/
   ```
   - Resultado esperado: compilación exitosa sin errores.

3. Ejecutar los tests del backend:
   ```powershell
   dotnet test app/backend/tests/RealtorApiTests/RealtorApiTests.csproj
   ```
   - Resultado esperado: los tests de infraestructura pasan.

4. Ejecutar la aplicación backend:
   ```powershell
   dotnet run --project app/backend/src/RealtorApi/RealtorApi.csproj --urls http://localhost:5000
   ```

5. Verificar el endpoint `/health`:
   ```powershell
   curl http://localhost:5000/health
   ```
   - Resultado esperado: respuesta HTTP 200 con payload de estado.

## Qué validar

- El proyecto backend compila exitosamente.
- `Program.cs` no contiene registros manuales de slices ni controllers.
- El endpoint `/health` se expone como parte del mapeo automático de `ISlice`.
- Los tests de infraestructura se ejecutan y pasan.

## Artefactos relevantes

- `specs/002-foundation-backend/spec.md`
- `specs/002-foundation-backend/plan.md`
- `specs/002-foundation-backend/research.md`
- `specs/002-foundation-backend/data-model.md`
- `specs/002-foundation-backend/contracts/backend-minimal-api.md`
