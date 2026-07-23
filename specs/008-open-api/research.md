# Investigación: Documento OpenAPI v1 de RealtorApi

## 1) Generación del documento desde la implementación real

**Decisión**: Usar la compatibilidad nativa de OpenAPI en ASP.NET Core con generación en build para emitir el documento versionado `v1` desde la propia implementación Minimal API.

**Racional**:
- Mantiene la implementación y el contrato bajo una única fuente de verdad.
- Permite que `dotnet build` produzca el documento sin edición manual.
- Encaja con el stack obligatorio del proyecto y evita dependencias externas de UI o scaffolding.

**Alternativas consideradas**:
- **Edición manual de JSON**: rechazada porque rompe la trazabilidad y el requisito de generación desde la implementación real.
- **Swashbuckle**: rechazada por la constitución y por no ser necesaria en .NET 11 con soporte nativo.
- **Documento mantenido aparte del backend**: rechazada por riesgo alto de drift.

## 2) Publicación del contrato como archivo estático

**Decisión**: Servir `app/backend/src/RealtorApi/wwwroot/openapi/v1.json` con archivos estáticos y no exponer una UI de documentación en runtime.

**Racional**:
- La URL pública debe ser estable y simple: `GET /openapi/v1.json`.
- La publicación como archivo estático garantiza que el contrato servido es exactamente el artefacto versionado.
- Cumple la exclusión explícita de Swagger UI, ReDoc UI y cualquier interfaz interactiva.

**Alternativas consideradas**:
- **MapOpenApi en runtime**: descartado como superficie principal porque el requisito pide archivo estático versionado y no UI.
- **Endpoint dinámico de documentación**: descartado por no cumplir la intención de estabilidad del archivo versionado.

## 3) Metadatos y componentes reutilizables

**Decisión**: Anotar cada slice con metadatos explícitos de OpenAPI y respuestas HTTP para que el generador produzca parámetros, request bodies, responses y componentes reutilizables como `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails`.

**Racional**:
- Reduce esquemas inline y mejora la reutilización de componentes.
- Hace el contrato más consumible por herramientas externas como NSwag.
- Mantiene la documentación alineada con las firmas reales de los endpoints.

**Alternativas consideradas**:
- **Inferencia implícita sin metadatos**: descartada porque genera documentos más pobres y aumenta el riesgo de respuestas incompletas.
- **Esquemas inline duplicados**: descartados por el requisito explícito de evitar duplicación.

## 4) Detección de drift entre endpoints y contrato

**Decisión**: Comparar el inventario en runtime de `EndpointDataSource` con los paths documentados, normalizando las constraints de ruta a su forma canónica.

**Racional**:
- La normalización evita falsos positivos entre `{id:guid}` y `{id}`.
- `EndpointDataSource` refleja la realidad del backend desplegado, no una copia estática.
- Permite una prueba de drift útil y accionable cuando el contrato y la implementación se desalinean.

**Alternativas consideradas**:
- **Comparación textual exacta de rutas**: rechazada porque falla en rutas equivalentes con constraints diferentes.
- **Validación solo por snapshot del JSON**: rechazada porque no detecta de forma fiable cambios en la implementación real.

## 5) Validación externa con Redocly y NSwag

**Decisión**: Ejecutar `npx @redocly/cli lint` sobre el documento generado y `dotnet tool run nswag openapi2csclient` como smoke test de consumo del contrato.

**Racional**:
- Redocly valida estructura y reglas del contrato OpenAPI.
- NSwag comprueba que un consumidor real puede generar un cliente tipado sin correcciones manuales.
- Ambas validaciones completan la verificación reproducible pedida por la spec.

**Alternativas consideradas**:
- **Solo pruebas unitarias internas**: insuficientes para validar consumibilidad del contrato.
- **Herramientas de UI/documentación**: descartadas porque la feature excluye UI interactiva.

## 6) Configuración obligatoria de Redocly y herramientas locales

**Decisión**: Mantener `.redocly.yaml` en la raíz con reglas mínimas para una API pública sin autenticación y registrar NSwag en `dotnet-tools.json`.

**Racional**:
- Redocly puede ser estricto por defecto y bloquear una API pública sin security scheme.
- NSwag debe poder restaurarse de forma reproducible con `dotnet tool restore`.
- La validación depende de prerequisitos explícitos y documentados.

**Alternativas consideradas**:
- **Configurar Redocly solo por línea de comandos**: descartado porque la spec exige el archivo raíz como artefacto.
- **Instalación manual de NSwag**: descartada porque rompe la reproducibilidad y la trazabilidad.

## 7) Estructura del script reproducible

**Decisión**: Mantener un script PowerShell en `support/scripts/generate-openapi-v1.ps1` que ejecute en orden: build, lint, smoke test y resumen final.

**Racional**:
- Centraliza la validación en un único flujo reproducible.
- Facilita la verificación manual y la automatización local.
- Permite fallar explícitamente si falta el documento o si una validación no pasa.

**Alternativas consideradas**:
- **Múltiples scripts sueltos**: descartado porque fragmenta la validación.
- **Configuración adicional obligatoria para NSwag**: descartada como primera opción; el flujo debe preferir el comando CLI directo para minimizar artefactos extra.
