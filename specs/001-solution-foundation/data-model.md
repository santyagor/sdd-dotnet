# Modelo de Datos y Estructura de la Solución

## Entidades clave

### Solución Realtor
- Tipo: Contenedor de proyectos.
- Campos:
  - `Realtor.sln`: referencia a todos los proyectos backend, frontend y tests.
  - `global.json`: versión de SDK obligatoria.
- Relaciones:
  - Incluye `RealtorApi`, `RealtorApiTests`, `RealtorWeb` y `RealtorWeb.Tests`.

### Proyecto RealtorApi
- Tipo: Backend ASP.NET Core Minimal API.
- Campos:
  - `Program.cs`: configuración mínima de servicios y middleware.
  - `Features/`: carpeta para futuras slices de Vertical Slice.
  - `appsettings.json`: configuración base de logging y hosts.
- Relaciones:
  - Referenciado por `RealtorApiTests`.

### Proyecto RealtorApiTests
- Tipo: Test project xUnit.
- Campos:
  - `Usings.cs`: directivas globales.
  - `UnitTests/`, `IntegrationTests/`: carpetas para pruebas placeholder.
- Relaciones:
  - Referencia directa a `RealtorApi`.

### Proyecto RealtorWeb
- Tipo: Blazor Web App.
- Campos:
  - `Program.cs`: configuración base de Blazor y routing.
  - `wwwroot/app.css`: CSS propio centralizado.
  - `Pages/`, `Shared/`, `Components/`: estructura estándar.
- Relaciones:
  - Referenciado por `RealtorWeb.Tests`.

### Proyecto RealtorWeb.Tests
- Tipo: Test project xUnit con Bunit.
- Campos:
  - `Usings.cs`: directivas globales.
  - `Components/`: carpeta para pruebas de componentes.
- Relaciones:
  - Referencia directa a `RealtorWeb`.

## Reglas de validación

- `dotnet build app/` debe compilar todos los proyectos sin errores.
- `dotnet test app/` debe descubrir los proyectos de prueba y ejecutar los placeholders.
- Backend no debe contener `Controllers/`.
- `Program.cs` solo debe incluir configuración de servicios/middleware/endpoints mínimos.
- `global.json` no puede ser modificado en esta iniciativa.
- `wwwroot/app.css` debe ser el único archivo CSS global del frontend.

## Nota sobre dominio

En esta fase no se introducen entidades de dominio ni modelos de persistencia reales. El modelo de datos es principalmente estructural, orientado a la organización de la solución y preparación de la arquitectura para futuras features.
