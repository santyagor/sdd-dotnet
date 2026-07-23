# Modelo de diseño: Documento OpenAPI v1 de RealtorApi

## 1) `DocumentoOpenApiV1`

Representa el artefacto versionado que se publica en `app/backend/src/RealtorApi/wwwroot/openapi/v1.json`.

| Campo | Tipo | Regla |
|---|---|---|
| `version` | string | Debe ser `v1` como nombre lógico del documento. |
| `outputPath` | string | Debe apuntar a `wwwroot/openapi/v1.json`. |
| `sourceOfTruth` | string | Debe ser la implementación real del backend Minimal API. |
| `format` | string | JSON OpenAPI generado, no editado manualmente. |
| `publicAccessPath` | string | Debe ser `GET /openapi/v1.json`. |

**Relaciones**:
- Contiene el inventario de endpoints documentados.
- Contiene componentes reutilizables para esquemas compartidos.
- Es validado por Redocly, NSwag y las pruebas de drift.

## 2) `InventarioEndpoint`

Representa cada operación HTTP que debe aparecer en el documento.

| Campo | Tipo | Regla |
|---|---|---|
| `method` | string | GET, POST o PUT según la implementación. |
| `route` | string | Ruta canónica normalizada sin constraint de ruta. |
| `rawRoute` | string | Ruta real del endpoint; se usa solo para verificación. |
| `requestModel` | string? | Modelo de entrada cuando aplique. |
| `responseModel` | string? | Modelo de salida principal cuando aplique. |
| `successStatusCodes` | colección | Debe incluir los códigos esperados del contrato. |
| `errorStatusCodes` | colección | Debe incluir los errores documentables del endpoint. |

**Endpoints en alcance**:
- `GET /health`
- `GET /api/properties`
- `GET /api/properties/{id}`
- `POST /api/properties`
- `PUT /api/properties/{id}`

**Regla de normalización**:
- `{id:guid}` y `{id}` se consideran equivalentes para efectos del drift.
- La comparación debe eliminar constraints de ruta sin alterar el nombre del parámetro.

## 3) `EsquemaCompartido`

Representa los componentes reutilizables que el documento debe exponer en `components/schemas`.

| Esquema | Tipo | Regla |
|---|---|---|
| `PropertyStatus` | enum | Debe reutilizarse en operaciones de propiedades y no duplicarse inline. |
| `ProblemDetails` | objeto | Debe representar errores de negocio o fallos esperados. |
| `HttpValidationProblemDetails` | objeto | Debe representar errores de validación de entrada. |

**Relaciones**:
- Los responses de error deben referenciar estos componentes mediante `$ref` cuando corresponda.
- Los esquemas compartidos deben mantenerse estables para consumidores externos.

## 4) `ValidacionContrato`

Representa la evidencia de calidad generada por el flujo reproducible.

| Campo | Tipo | Regla |
|---|---|---|
| `redoclyResult` | estado | Debe pasar sin errores bloqueantes. |
| `nswagResult` | estado | Debe generar un cliente tipado sin correcciones manuales. |
| `driftResult` | estado | Debe fallar cuando haya desalineación real entre código y documento. |
| `generatedClientPath` | string | Debe apuntar a `artifacts/openapi-client-smoke/`. |

## 5) `PrerequisitoHerramienta`

Representa el estado de las herramientas necesarias antes de validar.

| Herramienta | Condición |
|---|---|
| Node.js | Versión 20 o superior. |
| npm | Disponible en PATH. |
| npx | Disponible para ejecutar `@redocly/cli`. |
| Redocly CLI | Ejecutable con `npx @redocly/cli --version`. |
| NSwag.ConsoleCore | Disponible tras `dotnet tool restore`. |

## Reglas de integridad

- El documento no se considera correcto si falta cualquiera de los endpoints en alcance.
- La existencia del archivo no basta; debe coincidir con la implementación real.
- Las constraints de ruta nunca deben generar falsos positivos en drift.
- Los esquemas compartidos deben preferirse sobre duplicación inline.
- La validación reproducible debe fallar explícitamente si el documento no se genera o si una herramienta externa falla.
