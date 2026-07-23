# Investigación: Actualización de estado de propiedad

## 1) Exponer la operación como `PATCH /api/properties/{id}/status`

**Decisión**: Usar `PATCH` en una ruta dedicada para modificar únicamente el estado de una propiedad existente.

**Racional**:
- La intención del caso de uso es parcial y acotada a un único campo.
- Mantiene la semántica pública clara sin reutilizar el flujo de actualización completa.
- Evita ambigüedad con los demás campos editables del recurso.

**Alternativas consideradas**:
- **`PUT /api/properties/{id}`**: descartado porque su semántica implica actualización completa y podría inducir a modificar otros campos.
- **`PATCH /api/properties/{id}`**: descartado porque la ruta no deja explícito que la operación solo afecta al estado.

## 2) Representar `status` como valor numérico del enum de dominio

**Decisión**: Aceptar `status` como valor numérico correspondiente a `PropertyStatus`.

**Racional**:
- Coincide con la aclaración definida en la spec.
- Reutiliza el enum ya existente en el dominio sin introducir conversiones adicionales.
- Simplifica la validación del cuerpo JSON y la documentación OpenAPI.

**Alternativas consideradas**:
- **Valores textuales del enum**: descartado porque no coincide con la decisión aclarada para esta feature.
- **Body con múltiples campos**: descartado porque contradice el alcance mínimo.

## 3) Validar `status` y el id antes de actualizar

**Decisión**: Resolver primero la propiedad por id, validar el cuerpo de entrada y solo después persistir el cambio.

**Racional**:
- Permite responder `404` cuando el recurso no existe.
- Permite responder `400` cuando `status` falta o no es válido.
- Evita efectos secundarios sobre campos no autorizados.

**Alternativas consideradas**:
- **Actualizar primero y validar después**: descartado por riesgo de persistencia inválida.
- **Validación implícita únicamente por model binding**: descartado porque no basta para separar `400` y `404` de manera clara.

## 4) Reutilizar la entidad persistente y devolver la representación pública actualizada

**Decisión**: Cargar la entidad `Property`, cambiar solo `Status` y devolver la representación pública actualizada, preservando `imageUrl` y el resto de datos persistidos.

**Racional**:
- Mantiene consistencia con el contrato público ya usado por las demás consultas.
- Evita duplicar lógica de mapeo y reduce riesgo de perder campos al responder.
- Cumple la restricción de no modificar otros atributos.

**Alternativas consideradas**:
- **Devolver solo confirmación sin contenido**: descartado porque la spec exige retornar la propiedad actualizada.
- **Crear un DTO nuevo con solo `status`**: descartado porque no cumple el contrato de respuesta solicitado.

## 5) Documentar y probar manualmente desde `RealtorApi.http`

**Decisión**: Incluir un ejemplo manual de `PATCH` en `app/backend/src/RealtorApi/RealtorApi.http`.

**Racional**:
- La spec exige verificación manual desde el archivo `.http` del proyecto.
- Centraliza una forma reproducible de probar el endpoint durante desarrollo.
- No cambia la lógica del backend ni requiere herramientas nuevas.

**Alternativas consideradas**:
- **Solo documentación en quickstart**: descartado porque la spec exige el ejemplo en `.http`.
- **Usar un archivo externo adicional**: descartado porque fragmentaría la validación manual.

## 6) Sin migración de base de datos

**Decisión**: Reutilizar el esquema existente de `Property` sin cambios de base de datos.

**Racional**:
- La operación solo cambia el valor de una columna ya existente.
- No se introducen nuevas entidades, relaciones ni columnas.
- Mantiene el cambio acotado y de bajo riesgo.

**Alternativas consideradas**:
- **Crear una migración nueva**: descartado porque no aporta valor funcional.
- **Agregar una tabla de auditoría**: descartado por estar fuera del alcance.
