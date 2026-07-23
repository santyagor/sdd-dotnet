# Contrato de publicación OpenAPI v1 de RealtorApi

## Objetivo

Publicar un único documento OpenAPI versionado para la API pública de `RealtorApi`, accesible como archivo estático en `GET /openapi/v1.json` y generado desde la implementación real del backend.

## Alcance funcional

El documento debe cubrir el 100% de los endpoints públicos implementados en esta versión:

- `GET /health`
- `GET /api/properties`
- `GET /api/properties/{id}`
- `POST /api/properties`
- `PUT /api/properties/{id}`

## Reglas del contrato

1. El documento debe generarse a partir del backend Minimal API real.
2. No se permite edición manual del JSON publicado.
3. El archivo versionado debe residir en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.
4. El documento debe ser servido por archivos estáticos como recurso estable.
5. No se publica Swagger UI, ReDoc UI ni ninguna interfaz visual de exploración.
6. Las herramientas `@redocly/cli` y NSwag se consideran utilidades de desarrollo fuera de runtime.

## Esquemas reutilizables obligatorios

El documento debe exponer en `components/schemas` los tipos compartidos siguientes:

- `PropertyStatus`
- `ProblemDetails`
- `HttpValidationProblemDetails`

Estos esquemas deben referenciarse mediante `$ref` cuando corresponda para evitar duplicación inline.

## Respuestas esperadas

Cada operación debe documentar:

- método HTTP
- ruta canónica
- parámetros de ruta y query
- request body cuando aplique
- responses de éxito
- responses de error basadas en `ProblemDetails` o `HttpValidationProblemDetails`

## Normalización de rutas

Para la validación de drift, las constraints de ASP.NET Core son equivalentes a su forma canónica sin constraint.

Ejemplo:

- `GET /api/properties/{id:guid}`
- `GET /api/properties/{id}`

Ambas formas deben considerarse equivalentes al comparar la implementación con el documento.

## Validación obligatoria

La validez del contrato depende de tres verificaciones complementarias:

1. `dotnet build` del backend genera el documento.
2. `npx @redocly/cli lint` valida el JSON con `.redocly.yaml`.
3. `dotnet tool run nswag openapi2csclient` genera un cliente tipado de smoke test.

## Artefactos relacionados

- Plan: [../plan.md](../plan.md)
- Modelo de diseño: [../data-model.md](../data-model.md)
- Guía de validación: [../quickstart.md](../quickstart.md)
- Script reproducible: `support/scripts/generate-openapi-v1.ps1`
