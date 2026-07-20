# Modelo de Datos y Estructura Arquitectural

## Propósito

Esta fase no introduce entidades de dominio ni modelos de persistencia. El modelo de datos es principalmente arquitectural y describe cómo se integran los componentes transversales del backend.

## Componentes principales

### `ISlice`

- Representa un contenedor de endpoints Minimal API.
- Cada implementación de `ISlice` expone un método para mapear rutas sobre `IEndpointRouteBuilder`.
- El backend descubre todas las implementaciones de `ISlice` por scanning de assemblies.

### `IHandler`

- Marker interface para handlers de casos de uso futuros.
- Permite el auto-registro automático de servicios por convención sin instancias concretas en esta fase.
- No se crean handlers concretos en esta iniciativa.

### Validación automática

- Una fábrica central (`ValidationFilterFactory`) detecta `IValidator<T>` en los assemblies registrados.
- Si existe un validator para el tipo de request, se ejecuta antes de que el endpoint procese la solicitud.
- Si no existe validator, el request pasa sin error.
- Los errores de validación se traducen a `ValidationProblemDetails` con HTTP 400.

### Manejo de errores esperados

- El backend implementa un mapeo centralizado de `Result` de error a `ProblemDetails`.
- Los resultados de error esperados se convierten en respuestas HTTP con estado adecuado.
- Se mantiene registro estructurado con `ILogger` para eventos de error y de infraestructura.

### Sonda de infraestructura `/health`

- Implementada como una clase que cumple `ISlice`.
- Su único propósito es validar que el mecanismo de descubrimiento y mapeo de slices funciona.
- No depende de base de datos ni lógica de negocio.

## Relaciones y flujo

1. `Program.cs` configura servicios y middleware.
2. La aplicación registra scanning de assemblies para:
   - descubrir `ISlice`
   - registrar `IHandler`
   - registrar validators de FluentValidation
3. El punto de mapeo central llama a cada slice descubierto para exponer sus rutas.
4. El pipeline de request aplica validación automática si existe un `IValidator<T>`.
5. Las respuestas de error esperadas se convierten en `ProblemDetails` uniformes.

## Nota

- No hay entidad persistente ni modelo `DbContext` en este plan.
- La arquitectura está preparada para incorporar dominio y persistencia en una fase posterior.
