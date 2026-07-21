# research.md

## Decision: Seed data from external JSON and manifest files

- Chosen approach: Implement `DatabaseSeeder` that loads `properties.json`, `properties-statuses.json` and `seed-manifest.json` from the `support/seed-data` folder at runtime.
- Rationale: El prompt exige integrar archivos externos en el proyecto y no usar `HasData`. Un seeder basado en archivos mantiene el contenido de seed desacoplado del código y permite que las rutas de imagen sean resueltas dinámicamente desde el manifiesto.
- Alternatives considered:
  - `HasData`: rechazado porque la spec prohíbe su uso para este escenario de seeding.
  - Datos embebidos en código: rechazado porque no cumple la exigencia de archivos externos consumibles en runtime.
  - Recursos embebidos en ensamblado: rechazado porque la spec pide archivos JSON y manifest disponibles en runtime y la imagen debe ser servida desde una ruta pública de la API.

## Decision: Persistir `PropertyStatus` como string mediante conversión EF Core

- Chosen approach: Definir `PropertyStatus` como enum de dominio y configurar la columna con `HasConversion<string>()` dentro de `PropertyConfiguration`.
- Rationale: La spec solicita estado tipado en el dominio y almacenamiento string en la base de datos. EF Core conversion permite mantener ambos sin exponer valores numéricos.
- Alternatives considered:
  - Guardar el enum como int: rechazado porque no cumple el requisito de persistencia string.
  - Tabla separada para estados: rechazado porque la spec describe un valor de estado simple y pide conversión string.

## Decision: Publicar imágenes en `wwwroot` y guardar `ImageUrl` como ruta pública

- Chosen approach: Copiar o sincronizar los archivos de imagen desde el manifiesto hacia un destino público bajo `wwwroot/images/properties` y persistir `ImageUrl` con ruta relativa pública, por ejemplo `/images/properties/{filename}`.
- Rationale: El requisito es almacenar la ruta final pública servida por la API, no la ruta física de soporte. Un directorio de archivos estáticos en `wwwroot` cumple esa condición y permite servir las imágenes directamente.
- Alternatives considered:
  - Mantener imágenes en `support/seed-data` y servirlas con un `FileProvider`: viable, pero preferimos un destino público único y consistente con `wwwroot` para simplificar la URL final y la configuración de publicación.
  - Guardar rutas físicas absolutas: rechazado por la regla explícita contra rutas físicas en `ImageUrl`.

## Decision: Ejecutar siempre `MigrateAsync` y `UseAsyncSeeding`

- Chosen approach: Extender `AppDbContext` con `UseSeeding` y `UseAsyncSeeding`, y ejecutar `await app.MigrateAsync();` antes de `app.Run()`.
- Rationale: El prompt exige que el arranque siempre ejecute migraciones y seeding async, incluso si no hay migraciones pendientes, y que no se condicione `MigrateAsync` por migraciones pendientes.
- Alternatives considered:
  - Condicionar `MigrateAsync` a migraciones pendientes: rechazado por la restricción específica.
  - Ejecutar seeding solo desde código de migración: rechazado porque la spec lo prohíbe y porque el seeding debe ser independiente y idempotente.

## Open Questions / Clarifications

- No quedan aclaraciones abiertas. El alcance quedó confirmado para la infraestructura de persistencia, migración, seeding y hosting de imágenes; los endpoints de consulta quedan para otra spec.
