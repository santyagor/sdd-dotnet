# Contrato: Backend Minimal API Infrastructure

## Propósito

Definir el contrato público de la infraestructura base del backend `RealtorApi` para futuros slices, validación y manejo de errores.

## Contratos principales

### `ISlice`

- Una implementación de `ISlice` debe exponer un método que reciba `IEndpointRouteBuilder` y registre sus rutas.
- El backend descubre todas las implementaciones de `ISlice` por scanning de assemblies y las mapea automáticamente.
- `Program.cs` no debe conocer implementaciones concretas de `ISlice`.

### `IHandler`

- Marker interface para handlers de caso de uso.
- El contenedor de dependencias registra implementaciones de `IHandler` automáticamente por assembly scanning.
- Esta iniciativa no crea handlers concretos; el contrato existe solo para infraestructura.

### Validación por convención

- La aplicación debe registrar validators de FluentValidation mediante scanning de assemblies.
- La convención es: si existe `IValidator<T>` para un tipo de request, ese validator se ejecuta automáticamente antes de que el endpoint procese la request.
- Si no existe validator para un tipo, el pipeline debe continuar sin error.
- En validación fallida, la respuesta debe ser `400 ValidationProblemDetails`.

### Manejo de errores esperados

- El backend debe exponer un mapeo centralizado de errores esperados a `ProblemDetails`.
- El contrato define respuestas uniformes y status codes determinados por el tipo de error.

## Endpoint de prueba

### `/health`

- Debe ser implementado como una clase que respete `ISlice`.
- Debe mapearse automáticamente sin referencias manuales en `Program.cs`.
- Respuesta esperada: `200 OK` con un payload simple que indique estado saludable.

## Invariantes del contrato

- No se permitirá la introducción de controllers en esta infraestructura base.
- La infraestructura debe ser ampliable: agregar nuevos slices no debe requerir modificación en `Program.cs`.
- La existencia de validators debe ser opcional y no causará errores si faltan.
