# Feature Specification: Documento OpenAPI v1 de RealtorApi

**Feature Branch**: `[008-open-api]`

**Created**: 2026-07-22

**Estado**: Implementada

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Generar un único documento OpenAPI versionado (v1) del contrato HTTP público de RealtorApi, publicado como openapi/v1.json y accesible por una ruta HTTP estable GET /openapi/v1.json. El documento debe generarse a partir de la implementación real del backend (Minimal APIs), nunca por edición manual."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Contrato público versionado (Priority: P1)

Como equipo de desarrollo, se necesita un único contrato HTTP oficial para RealtorApi que refleje exactamente los endpoints públicos implementados y pueda consumirse desde una ruta estable.

**Why this priority**: Es la base para compartir el contrato con consumidores, automatizar validaciones y evitar documentación desalineada.

**Independent Test**: Verificar que el documento versionado existe, se publica en una ruta HTTP fija y describe todos los endpoints públicos definidos.

**Acceptance Scenarios**:

1. **Given** que la aplicación está ejecutándose, **When** se solicita GET /openapi/v1.json, **Then** se devuelve el documento OpenAPI v1 en formato JSON.
2. **Given** que existen los endpoints públicos implementados, **When** se inspecciona el documento generado, **Then** aparecen todos los endpoints en alcance sin omisiones.

---

### User Story 2 - Validación de alineación automática (Priority: P1)

Como equipo de calidad, se necesita detectar cualquier desalineación entre el backend real y el contrato publicado antes de entregar cambios.

**Why this priority**: Evita que consumidores dependan de un contrato incorrecto o incompleto.

**Independent Test**: Ejecutar una verificación automatizada que compare los endpoints reales con el documento generado y falle ante cualquier diferencia.

**Acceptance Scenarios**:

1. **Given** que un endpoint real cambia sin actualizar el documento, **When** se ejecutan las pruebas de drift, **Then** la validación falla.
2. **Given** que el documento coincide con la implementación real, **When** se ejecutan las pruebas de drift, **Then** la validación pasa.

---

### User Story 3 - Consumo externo confiable (Priority: P2)

Como consumidor externo del contrato, se necesita poder reutilizar esquemas compartidos y generar un cliente tipado de prueba sin correcciones manuales.

**Why this priority**: Confirma que el documento no solo existe, sino que también es consumible por herramientas de integración.

**Independent Test**: Validar que los tipos compartidos están centralizados y que un cliente tipado puede generarse automáticamente a partir del documento.

**Acceptance Scenarios**:

1. **Given** que el documento está disponible, **When** se procesa con una herramienta de validación de contrato, **Then** no se requieren ediciones manuales para interpretarlo.
2. **Given** que el documento expone tipos compartidos, **When** se revisan sus componentes, **Then** los esquemas comunes se reutilizan mediante referencias.

---

### Edge Cases

- La normalización debe tratar como equivalentes las rutas con restricciones del router y las rutas canónicas sin restricciones.
- Si el documento no se genera, la verificación reproducible debe fallar de forma explícita.
- Si falta alguna herramienta requerida en la máquina de desarrollo, la especificación debe considerarlo un bloqueo previo a la validación.
- Si un endpoint devuelve error, el contrato debe representar de forma consistente el formato ProblemDetails esperado.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema MUST publicar un único documento OpenAPI v1 del contrato HTTP público de RealtorApi.
- **FR-002**: El sistema MUST exponer el documento publicado mediante GET /openapi/v1.json como un recurso HTTP estable.
- **FR-003**: El sistema MUST almacenar el documento versionado en app/backend/src/RealtorApi/wwwroot/openapi/v1.json.
- **FR-004**: El documento MUST describir el 100% de los endpoints públicos implementados en alcance: GET /health, GET /api/properties, GET /api/properties/{id}, POST /api/properties y PUT /api/properties/{id}.
- **FR-005**: El documento MUST incluir método, ruta, parámetros de ruta y query, request body cuando aplique, y responses de éxito y de error para cada endpoint en alcance.
- **FR-006**: El documento MUST representar de forma consistente los errores basados en ProblemDetails y HttpValidationProblemDetails.
- **FR-007**: El documento MUST exponer PropertyStatus, ProblemDetails y HttpValidationProblemDetails como componentes reutilizables en components/schemas, referenciados mediante $ref cuando corresponda.
- **FR-008**: El documento MUST generarse a partir de la implementación real del backend y no mediante edición manual del archivo JSON.
- **FR-009**: El sistema MUST incluir una verificación automatizada de drift que falle cuando exista desalineación entre los endpoints reales del backend y el documento publicado.
- **FR-010**: La verificación automatizada MUST comparar el inventario de endpoints en tiempo de ejecución contra los paths documentados, aplicando la normalización de restricciones del router como equivalentes a su forma canónica.
- **FR-011**: El sistema MUST permitir validar el documento con una herramienta externa de linting de OpenAPI usando la configuración de la raíz del repositorio.
- **FR-012**: El sistema MUST incluir .redocly.yaml en la raíz del repositorio como artefacto obligatorio de esta feature.
- **FR-013**: La configuración .redocly.yaml MUST deshabilitar la regla security-defined y tratar operation-4xx-response como warning.
- **FR-014**: El sistema MUST incluir NSwag ConsoleCore en dotnet-tools.json y permitir su restauración y ejecución como parte de una verificación de consumo del contrato.
- **FR-015**: La validación automatizada MUST generar un cliente tipado de prueba con NSwag como smoke test y fallar si el documento no puede consumirse sin correcciones manuales.
- **FR-016**: El sistema MUST incluir support/scripts/generate-openapi-v1.ps1 como script reproducible para regenerar y validar el documento en una secuencia determinista.
- **FR-017**: El script reproducible MUST ejecutar, en orden, la generación del documento desde el build del backend, la validación con el lint externo, la generación del cliente tipado de prueba y el reporte de éxito final.
- **FR-018**: El script reproducible MUST fallar explícitamente si el documento no existe, si no se genera correctamente o si cualquiera de las validaciones falla.
- **FR-019**: La solución MUST excluir Swagger UI, ReDoc UI y cualquier interfaz visual de exploración interactiva en runtime.
- **FR-020**: La especificación MUST documentar que las herramientas @redocly/cli y NSwag son utilidades CLI offline de desarrollo y validación, no superficie funcional de la aplicación.
- **FR-021**: La especificación MUST incluir una lista previa de verificación de herramientas de desarrollo para Node.js, npm, npx, dotnet tool restore y disponibilidad de NSwag.
- **FR-022**: La solución MUST mantener la única versión de contrato en v1, sin introducir versionado paralelo para esta feature.

### Requisitos técnicos y herramientas externas

Esta sección documenta los requisitos que deben estar disponibles en la máquina de desarrollo antes de ejecutar la validación de esta feature.

#### Requisitos de sistema

- Node.js 20 o superior disponible en PATH.
- npm disponible en PATH.
- npx disponible para ejecutar @redocly/cli.

#### Herramientas NPM

- @redocly/cli debe poder ejecutarse mediante npx @redocly/cli --version.

#### Herramientas .NET locales

- NSwag.ConsoleCore debe estar registrado en dotnet-tools.json.
- La herramienta debe poder restaurarse con dotnet tool restore.
- La herramienta debe poder ejecutarse con dotnet tool run nswag.

#### Archivo de configuración requerido

- .redocly.yaml debe existir en la raíz del repositorio.
- La configuración debe permitir validar una API pública sin autenticación sin producir un bloqueo por la regla security-defined.
- La configuración debe degradar operation-4xx-response a warning.

#### Verificación previa de herramientas

Antes de implementar o validar, se debe comprobar lo siguiente:

- node --version >= 20
- npm --version
- npx @redocly/cli --version
- dotnet tool restore
- dotnet tool list debe mostrar NSwag disponible

### Key Entities *(include if feature involves data)*

- **Documento OpenAPI v1**: Contrato versionado que describe la superficie HTTP pública de RealtorApi.
- **Esquema compartido**: Tipo reutilizable expuesto en components/schemas para evitar duplicación de contratos.
- **Reporte de validación**: Resultado observable de linting, generación de cliente y verificación de drift.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de los endpoints públicos en alcance aparece representado en v1.json, incluyendo GET /health.
- **SC-002**: GET /openapi/v1.json responde 200 OK con contenido JSON en todas las validaciones funcionales ejecutadas.
- **SC-003**: La validación externa del documento no reporta errores en el 100% de las ejecuciones exitosas previstas.
- **SC-004**: La verificación de drift falla en el 100% de los casos donde se introduce una desalineación entre implementación real y documento.
- **SC-005**: Un consumidor puede generar un cliente tipado de prueba sin realizar correcciones manuales al contrato.
- **SC-006**: El script de regeneración y validación completa el flujo sin errores cuando el entorno de desarrollo cumple los requisitos previos.
- **SC-007**: La configuración obligatoria de validación existe y es aceptada por las herramientas de control del contrato.

## Assumptions

- La implementación real del backend es la fuente de verdad para la publicación del contrato.
- La feature se limita a una sola versión de contrato: v1.
- La publicación del documento no modifica el comportamiento funcional de los endpoints existentes.
- El servidor puede servir el archivo JSON estático desde wwwroot sin requerir una UI de documentación.
- Las herramientas externas requeridas están instaladas y accesibles en la máquina de desarrollo antes de ejecutar las validaciones.
- La validación de drift y el smoke test de cliente tipado se ejecutan como parte de una verificación reproducible de desarrollo, no como funcionalidad en runtime.
