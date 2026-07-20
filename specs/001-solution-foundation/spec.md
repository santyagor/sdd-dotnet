# Especificación de Feature: Fundación de Solución Realtor

**Feature Branch**: `001-solution-foundation`

**Creada**: 2026-07-20

**Estado**: Draft

**Input**: Prompt instruction: `001-foundation-solution.prompt.md`

## Clarifications

### Session 2026-07-20

- Q: ¿Debemos configurar Serilog/ILogger base ahora en esta iniciativa? → A: No configurar logging; Program.cs mínimo posible, logging agregado solo cuando sea necesario.

## User Scenarios & Testing *(obligatorio)*

### User Story 1 - Developer configura la solución base (Prioridad: P1)

Un developer nuevo debe ser capaz de clonar el repositorio, restaurar dependencias, y compilar la solución completa sin errores en su máquina local dentro de 5 minutos.

**Por qué esta prioridad**: Es bloqueante para cualquier otra iniciativa. Sin una compilación exitosa, no se puede avanzar.

**Test Independiente**: Al ejecutar `dotnet build` desde la raíz con global.json vigente, la solución completa compila sin warnings de configuración ni errores.

**Acceptance Scenarios**:

1. **Given** un repositorio limpio clonado, **When** ejecuto `dotnet build` desde `app/`, **Then** compila exitosamente y genera artefactos en carpetas de output.
2. **Given** la solución compilada, **When** ejecuto `dotnet test` desde `app/`, **Then** todos los proyectos de test se descubren y los test placeholders corren sin error.
3. **Given** Program.cs base configurado, **When** verifico la estructura de carpetas, **Then** cada proyecto tiene Program.cs con servicios, middleware e inicialización de endpoints.

---

### User Story 2 - Developer entiende la estructura de la solución (Prioridad: P2)

Un developer debe entender de inmediato cómo está organizada la solución: dónde va el backend, frontend, tests, y qué convenciones se usan.

**Por qué esta prioridad**: Segunda: mejora la experiencia de incorporación y evita confusiones sobre dónde agregar código.

**Test Independiente**: Leer la estructura de carpetas respeta convenciones de Vertical Slice para backend y Blazor estándar para frontend. Documentación reflect esto.

**Acceptance Scenarios**:

1. **Given** la solución creada, **When** navego el árbol de carpetas, **Then** encuentro `app/backend/src/`, `app/backend/tests/`, `app/frontend/src/`, `app/frontend/test/` claramente.
2. **Given** el backend en vertical slice, **When** miro `RealtorApi/` csproj, **Then** NO hay carpeta `Controllers/` ni structuras genéricas de servicios globales.
3. **Given** el frontend Blazor, **When** reviso `RealtorWeb/` csproj, **Then** tiene estructura estándar Blazor: Components/, Pages/, Shared/, etc.

---

### User Story 3 - Tests de backend y frontend se ejecutan aisladamente (Prioridad: P3)

Los proyectos de test están separados, configurados y listos para recibir tests iniciales sin generar ruido en la compilación ni en la ejecución.

**Por qué esta prioridad**: Tercera: necesaria para conformidad con constitution pero no bloquea si falta. Se completa cuando se agrega primera feature que requiera tests.

**Test Independiente**: Proyectos de tests descubiertos por dotnet test y pueden contener test placeholders que pasan sin implementación.

**Acceptance Scenarios**:

1. **Given** la solución completa, **When** ejecuto `dotnet test` desde `app/`, **Then** se descubren ambos RealtorApiTests y RealtorWeb Test projects.
2. **Given** RealtorApiTests configurado, **When** miro el .csproj, **Then** tiene references a framework de test (xUnit) y a proyecto RealtorApi.
3. **Given** RealtorWeb Test configurado, **When** miro el .csproj, **Then** tiene referencias a Blazor testing utilities y al proyecto RealtorWeb.

---

### Edge Cases

- ¿Qué ocurre si global.json se actualiza tras la creación de la solución? La versión de SDK debe ser consistente en todo momento.
- ¿Qué ocurre si un developer clona sin restaurar NuGet? `dotnet build` debe fallar de forma clara reportando dependencias faltantes.

## Requirements *(obligatorio)*

### Requisitos Funcionales

- **FR-001**: La solución DEBE compilar sin errores cuando se ejecuta `dotnet build` desde la carpeta `app/` en máquina limpia.
- **FR-002**: DEBE existir archivo `app/Realtor.sln` que contenga referencias a todos los proyectos backend, frontend y tests.
- **FR-003**: Backend DEBE residir en `app/backend/src/RealtorApi/` como proyecto de clase ASP.NET Core (tipo Minimal API).
- **FR-004**: DEBE existir `app/backend/src/RealtorApi/Program.cs` configurado con servicios base, middleware y mapeo inicial de endpoints.
- **FR-005**: DEBE existir `app/backend/tests/RealtorApiTests/` como proyecto de test con framework xUnit.
- **FR-006**: Frontend DEBE residir en `app/frontend/src/RealtorWeb/` como Blazor Web App con Razor Components.
- **FR-007**: DEBE existir `app/frontend/test/RealtorWeb/` como carpeta de proyecto de test para componentes Blazor.
- **FR-008**: Estructura backend NO DEBE contener carpeta `Controllers/` ni servicios monolíticos genéricos.
- **FR-009**: Program.cs SOLO DEBE incluir configuración de servicios, middleware y mapeo inicial. NO debe contener lógica de negocio ni configuraciones de logging estructurado avanzadas no requeridas en esta fase.
- **FR-010**: Todos los proyectos DEBEN usar la versión de SDK declarada en `global.json`.
- **FR-011**: No se DEBE crear ni modificar `global.json` en esta iniciativa.

### Entidades Clave

- **Solución (Realtor.sln)**: Contenedor que agrupa todos los proyectos backend, frontend y tests.
- **Proyecto RealtorApi**: Backend con Minimal APIs y arquitectura Vertical Slice.
- **Proyecto RealtorApiTests**: Tests unitarios e integración para backend.
- **Proyecto RealtorWeb**: Frontend Blazor con componentes y páginas.
- **Proyecto RealtorWeb.Tests**: Tests para componentes y flujos Blazor.

## Success Criteria *(obligatorio)*

### Medidas de Éxito

- **SC-001**: `dotnet build` se completa en menos de 2 minutos en máquina de desarrollo estándar sin errores ni warnings críticos.
- **SC-002**: `dotnet test` descubre y ejecuta todos los proyectos de test (backend + frontend) sin fallos; test placeholders pasan.
- **SC-003**: Estructura de carpetas es clara y respeta convenciones: Vertical Slice para backend, Blazor estándar para frontend.
- **SC-004**: DevEx inicial es positiva: developer nuevo puede compilar y entender la estructura en < 10 minutos.
- **SC-005**: Solución está lista para recibir primeraeature sin cambios estructurales adicionales.
- **SC-006**: Documentación (README o Wiki) explica estructura y cómo ejecutar build/test.

## Assumptions

- `global.json` existe en raíz del repositorio y declara .NET 11.0.100-preview.6.26359.118.
- PostgreSQL será usado para persistencia (declarado en constitución), pero no se configura aún en esta iniciativa.
- Refit será usado para comunicación typed entre frontend y backend, pero integración ocurre en iniciativas posteriores.
- Developer local tiene .NET SDK 11 instalado (o tooling que lo maneja automáticamente).
- CI/CD pipeline se configura en iniciativas posteriores.
- Code review y compliance checks se aplican como parte del flujo spec-driven.
