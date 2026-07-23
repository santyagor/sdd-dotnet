# Guía de validación rápida: Documento OpenAPI v1 de RealtorApi

## Objetivo

Verificar que el contrato OpenAPI v1 se genera desde la implementación real, se publica como archivo estático y pasa las validaciones de drift y de herramientas externas.

## Prerrequisitos

Antes de continuar, confirmar que la máquina de desarrollo dispone de:

- Node.js 20 o superior
- npm disponible en PATH
- npx disponible
- NSwag ConsoleCore restaurable con `dotnet tool restore`
- `.redocly.yaml` presente en la raíz del repositorio

## Validación previa de herramientas

1. Confirmar versión de Node.js:
   - `node --version`
2. Confirmar npm:
   - `npm --version`
3. Confirmar Redocly CLI:
   - `npx @redocly/cli --version`
4. Restaurar herramientas .NET:
   - `dotnet tool restore`
5. Confirmar que NSwag está disponible:
   - `dotnet tool list`

**Resultado esperado**: las herramientas deben estar presentes antes de validar la feature.

## Regeneración reproducible del contrato

Ejecutar el script reproducible de la feature:

- `support/scripts/generate-openapi-v1.ps1`

**Resultado esperado**:
- el backend compila
- el documento se genera en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`
- Redocly lint pasa sin errores bloqueantes
- NSwag genera el cliente tipado de smoke test
- el script informa éxito con la ruta del documento

## Verificación funcional del documento publicado

1. Iniciar la aplicación backend.
2. Solicitar `GET /openapi/v1.json`.
3. Confirmar que la respuesta es `200 OK`.
4. Confirmar que el `Content-Type` es `application/json`.

**Resultado esperado**: el archivo estático publicado responde correctamente y coincide con el documento generado.

## Verificación de drift

Ejecutar la suite de pruebas xUnit de contrato una vez implementada la feature.

**Resultado esperado**:
- si se altera un endpoint real sin actualizar el contrato, la prueba falla
- si el contrato y el backend coinciden, la prueba pasa
- la normalización de constraints trata como equivalentes `{id:guid}` y `{id}`

## Flujo de aceptación recomendado

1. Validar prerequisitos.
2. Ejecutar el script reproducible.
3. Confirmar la publicación estática en `GET /openapi/v1.json`.
4. Ejecutar las pruebas de drift del backend.
5. Revisar que no existan errores de Redocly ni de NSwag.

## Evidencia de éxito

La feature se considera validada cuando:

- el documento se genera sin edición manual
- el lint de Redocly no reporta errores bloqueantes
- NSwag genera un cliente consumible
- las pruebas de drift detectan cualquier desalineación entre implementación y contrato
