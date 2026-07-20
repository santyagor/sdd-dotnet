# Contrato: Backend Minimal API Base

## Alcance

Documento de contrato para la interfaz mínima que expone el backend de la solución en esta fase fundacional.

## Endpoint raíz

- Método: `GET`
- Ruta: `/`
- Respuesta esperada:
  - Código HTTP: `200 OK`
  - Contenido: Texto o JSON simple de estado/conexión.

### Ejemplo de respuesta JSON

```json
{
  "status": "ok",
  "application": "RealtorApi"
}
```

## Contrato de inicio de la aplicación

- `Program.cs` debe mapear al menos un endpoint inicial que permita validar la aplicación sin lógica de negocio.
- La aplicación no debe exponer datos de dominio ni rutas funcionales específicas en esta fase.

## Notas

Este contrato es una base para futuras features. No define aún recursos de dominio ni operaciones CRUD completas; su objetivo es garantizar que la solución arranca y responde correctamente en el contexto fundacional.
