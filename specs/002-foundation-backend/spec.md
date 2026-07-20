# Feature Specification: Backend Foundation

**Feature Branch**: `002-foundation-backend`

**Created**: 2026-07-20

**Estado**: En implementación

- Estados canónicos: Borrador, Aprobada, En implementación, Implementada

**Input**: User description: "Crear la especificación 002-foundation-backend.

Objetivo: establecer la estructura base interna del backend RealtorApi y los
cross-cutting concerns transversales, sin crear entidades de dominio ni features
funcionales de producto. Esta iniciativa parte de la base ya creada en la spec 001
(solución app/Realtor.sln y proyecto app/backend/src/RealtorApi ya existen) y NO
debe recrear la solución ni los proyectos.

Alcance incluido:

1. Estructura interna de carpetas del backend siguiendo Vertical Slice Architecture,
   preparada para features futuras pero sin ninguna feature todavía. Debe existir la
   organización transversal en Infrastructure (por ejemplo Endpoints, Validation,
   Handlers) según backend.instructions.md, sin implementar casos de uso de negocio.

2. Infraestructura de descubrimiento y mapeo de endpoints Minimal API mediante el
   patrón ISlice con reflection:
   - Contrato ISlice.
   - RegisterSlices por assembly.
   - MapSliceEndpoints centralizado.
   - Agregar un endpoint no debe requerir modificar Program.cs.
   - Sin registries manuales ni descubrimiento por nombre de clase.

3. Infraestructura de auto-registro de handlers de casos de uso mediante el marker
   interface IHandler y assembly scanning, sin implementar ningún handler concreto.

4. Validación automática de Minimal APIs con FluentValidation:
   - Registro de validators por assembly scanning.
   - ValidationFilterFactory basada en convención que detecta IValidator<T>.
   - Ausencia de validator no produce error (pass-through).
   - Errores de validación devuelven ValidationProblemDetails con HTTP 400.
   - Sin registrar validators, filtros ni handlers de forma individual.

5. Manejo consistente de errores esperados con Result y conversión centralizada a
   ProblemDetails, y logging estructurado mediante ILogger, como capacidades base
   disponibles para futuras features.

6. Migrar el endpoint /health existente al patrón ISlice, como única sonda de
   infraestructura de extremo a extremo. Es infraestructura, no una feature de
   negocio: no introduce entidades, base de datos ni lógica de dominio. Sirve para
   demostrar que el descubrimiento y el mapeo centralizado funcionan sin tocar
   Program.cs.

7. Program.cs debe limitarse a: configurar servicios, middleware, registrar la
   infraestructura anterior y mapear endpoints mediante el mecanismo centralizado.

Pruebas requeridas (unit tests, en app/backend/tests/RealtorApiTests):

- RegisterSlices descubre una clase ISlice de prueba definida en el proyecto de
  tests y la registra bajo el contrato ISlice, sin duplicados.
- ValidationFilterFactory detecta IValidator<T> cuando existe y ejecuta la
  validación; cuando no existe validator, hace pass-through sin error.
- Un fallo de validación produce ValidationProblemDetails con HTTP 400.
- El mapeo de Result de error a ProblemDetails produce el status code y el payload
  correctos.
- El endpoint /health migrado a ISlice responde correctamente su estado.

Restricciones y exclusiones explícitas:

- NO crear entidades de dominio.
- NO crear AppDbContext, configuraciones EF Core, migraciones ni seeders.
- NO crear conexión a base de datos.
- NO crear ningún endpoint, handler, validator ni mapping de negocio.
- NO introducir lógica de negocio ni features de producto.
- NO usar controllers.
- Las clases ISlice y validators de prueba usados para verificar la infraestructura
  DEBEN vivir en el proyecto de tests, nunca en el proyecto de producción.
- La versión de plataforma se deriva de global.json existente, sin modificarlo.

Criterios de éxito verificables:

- El proyecto compila con la infraestructura transversal registrada.
- El endpoint /health queda implementado como ISlice y es descubierto y mapeado sin
  registro manual en Program.cs.
- Un request con IValidator<T> registrado sería validado automáticamente; sin
  validator, pasa sin error.
- Los unit tests de la infraestructura transversal pasan.
- No existe ninguna entidad, base de datos, feature ni caso de uso de negocio en el
  backend.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Registrar slices automáticamente (Priority: P1)

Un desarrollador de backend quiere agregar nuevos endpoints al proyecto sin tener
que modificar `Program.cs` cada vez. Al crear una clase que implementa `ISlice`
la infraestructura la descubre y la mapea automáticamente.

**Why this priority**: Esta es la capacidad central que habilita la arquitectura
base y reduce el acoplamiento de la infraestructura de endpoints.

**Independent Test**: Definir una clase `ISlice` de prueba en el proyecto de tests,
arrancar la aplicación y verificar que su endpoint se ha registrado.

**Acceptance Scenarios**:

1. **Given** la aplicación está configurada y cargada,
   **When** existe una clase `ISlice` en el assembly de backend o tests,
   **Then** `RegisterSlices` la descubre y la registra como un slice único.

2. **Given** varias clases `ISlice` en el mismo assembly,
   **When** se ejecuta el registro,
   **Then** ninguna clase se registra dos veces.

---

### User Story 2 - Validación automática de requests (Priority: P2)

Un desarrollador de backend quiere que los validators de FluentValidation se
apliquen de forma automática cuando existen y se ignoren cuando no existen,
para que no haya necesidad de registrar cada validator individualmente.

**Why this priority**: Evita registros manuales y asegura que la validación es una
capacidad transversal disponible desde el inicio.

**Independent Test**: Implementar un validator de prueba en el proyecto de tests,
enviar una request inválida y comprobar que la respuesta es `400 ValidationProblemDetails`.

**Acceptance Scenarios**:

1. **Given** un endpoint minimal API que acepta un request body,
   **When** existe un `IValidator<T>` para el tipo del body,
   **Then** el request válido continúa y el request inválido devuelve `400` con `ValidationProblemDetails`.

2. **Given** un endpoint minimal API que acepta un request body,
   **When** no existe un `IValidator<T>` para el tipo del body,
   **Then** el request continúa sin error de validación.

---

### User Story 3 - Manejo consistente de errores esperados (Priority: P3)

Un desarrollador de backend quiere que la aplicación traduzca errores esperados a
`ProblemDetails` y registre los eventos de error de forma estructurada, para que
la experiencia de error sea uniforme desde el inicio.

**Why this priority**: Establece una base sólida de observabilidad y manejo de
errores sin depender de comportamiento ad hoc en cada endpoint.

**Independent Test**: Simular un resultado de error en la infraestructura de manejo
centralizada y verificar el código de estado y el cuerpo de `ProblemDetails`.

**Acceptance Scenarios**:

1. **Given** un resultado de error esperado desde la lógica de infraestructura,
   **When** el endpoint lo devuelve,
   **Then** el middleware central convierte ese error en `ProblemDetails` con el estado adecuado.

2. **Given** un error inesperado no previsto por los resultados esperados,
   **When** ocurre durante el procesamiento de un request,
   **Then** el middleware produce una respuesta clara y registra el evento.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El backend debe exponer un contrato `ISlice` que represente un slice
  de endpoints Minimal API.
- **FR-002**: El backend debe descubrir e instanciar todas las implementaciones de
  `ISlice` por scanning de assemblies, sin registries manuales ni nombres de clase.
- **FR-003**: El backend debe registrar los endpoints de cada `ISlice` a través de un
  único punto central de mapeo en `Program.cs`.
- **FR-004**: El backend debe exponer un marker interface `IHandler` y registrar sus
  implementaciones automáticamente por scanning de assembly.
- **FR-005**: El backend debe registrar validators de FluentValidation mediante
  scanning de assembly y aplicar validación automática por convención.
- **FR-006**: Si no existe un `IValidator<T>` para un tipo de request, el pipeline debe
  continuar sin error de validación.
- **FR-007**: Si existe un `IValidator<T>` y la validación falla, la respuesta debe ser
  `400 ValidationProblemDetails`.
- **FR-008**: El backend debe exponer un mapeo centralizado de `Result` de error a
  `ProblemDetails` con el status code apropiado.
- **FR-009**: El backend debe registrar el endpoint `/health` como una implementación
  de `ISlice` y mapearlo automáticamente sin modificaciones adicionales en `Program.cs`.
- **FR-010**: El backend debe usar `ILogger` para logging estructurado de errores y
  eventos cruciales de la infraestructura.
- **FR-011**: No se debe crear ninguna entidad de dominio, `AppDbContext`, conexión de
  base de datos, migración o lógica de negocio de producto.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El backend compila correctamente con la infraestructura transversal
  registrada y sin implementaciones de negocio.
- **SC-002**: El endpoint `/health` responde correctamente cuando se expone como
  `ISlice` y se mapea sin registrar manualmente en `Program.cs`.
- **SC-003**: Una petición a un endpoint con validator devuelve `400` y
  `ValidationProblemDetails` cuando la validación falla.
- **SC-004**: Una petición al mismo endpoint sin validator continúa sin error de
  validación.
- **SC-005**: Los tests unitarios de la infraestructura transversal ejecutados en
  `app/backend/tests/RealtorApiTests` pasan.

## Assumptions

- El proyecto ya existente en `app/backend/src/RealtorApi` es la base sobre la que se
  construye la infraestructura, y no debe recrearse.
- Esta especificación solo cubre infraestructura transversal y no introduce ningún
  caso de uso de negocio real.
- El endpoint `/health` se usa únicamente como sonda de infraestructura para validar
  el descubrimiento de `ISlice`.
- Los artefactos de prueba necesarios para validar el registro de `ISlice` y
  validación pueden residir en el proyecto de tests.
- No se requiere soporte de base de datos ni persistencia adicional para esta fase.
