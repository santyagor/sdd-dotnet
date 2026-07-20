# Investigación: Fundación de Solución Realtor

## Decisiones

- Se mantiene la arquitectura establecida en la constitución: backend ASP.NET Core Minimal API y frontend Blazor Web App.
- Se usa la versión de SDK declarada en `global.json` (.NET 11.0.100-preview.6.26359.118) como fuente de verdad.
- Se crea una solución única `app/Realtor.sln` que agrupa backend, frontend y proyectos de test.
- Se aplazan las integraciones de persistencia (EF Core/Npgsql), validación avanzada (FluentValidation) y comunicación tipada (Refit) a iniciativas posteriores.

## Racional

- El feature es fundacional; el objetivo es garantizar que la solución compila y soporta la estructura de proyectos antes de añadir lógica de negocio.
- Mantener el alcance mínimo evita introducir dependencias o complejidad prematura que no está requerida por la spec.
- El uso de una sola solución cumple con la constitución de Solución Única y facilita la ejecución de `dotnet build` y `dotnet test` desde `app/`.

## Alternativas consideradas

- Usar un proyecto separado para backend y frontend sin solución común: descartado por la regla de solución única.
- Empezar con Controllers/MVC en backend: descartado porque la constitución exige Minimal APIs y Vertical Slice.
- Configurar persistencia y dominio en esta fase: descartado para mantener la fase fundacional simple y evitar mezcla de responsabilidades.

## Clarificaciones resueltas

- No hay decisiones abiertas sobre la estructura básica: la spec ya define claramente la ubicación de backend y frontend, los tipos de proyectos y la obligación de tests placeholder.
- Se confirma que no se debe modificar `global.json` en esta iniciativa.
