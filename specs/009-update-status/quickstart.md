# Guía de validación rápida: Actualización de estado de propiedad

## Objetivo

Verificar que `PATCH /api/properties/{id}/status` actualiza únicamente el estado de una propiedad existente, devuelve la propiedad actualizada y conserva intactos los demás datos persistidos.

## Prerrequisitos

Antes de continuar, confirmar que:

- La solución compila con .NET 11.
- El backend `RealtorApi` puede ejecutarse localmente.
- Existe una propiedad de prueba en la base de datos.
- El archivo `app/backend/src/RealtorApi/RealtorApi.http` está disponible en el proyecto.

## Validación manual desde `.http`

1. Abrir `app/backend/src/RealtorApi/RealtorApi.http`.
2. Localizar el ejemplo de actualización de estado para la ruta `PATCH /api/properties/{id}/status`.
3. Ejecutar una solicitud con un `id` existente y un cuerpo JSON como `{"status":0}`.
4. Confirmar que la respuesta devuelve `200 OK`.
5. Verificar que la propiedad devuelta refleja el nuevo estado y conserva los demás campos, incluyendo `imageUrl`.

**Resultado esperado**: el estado cambia y el resto de datos se mantiene sin alteraciones.

## Verificación de error: estado omitido o inválido

1. Ejecutar la misma solicitud sin la clave `status`.
2. Ejecutar la misma solicitud con un valor no admitido.

**Resultado esperado**: la API responde `400 Bad Request` con `ProblemDetails`.

## Verificación de error: propiedad inexistente

1. Ejecutar la solicitud con un `id` que no exista.

**Resultado esperado**: la API responde `404 Not Found`.

## Flujo de aceptación recomendado

1. Ejecutar el backend.
2. Probar la solicitud de actualización en `RealtorApi.http`.
3. Validar el caso exitoso.
4. Validar los casos de error `400` y `404`.
5. Confirmar que ningún otro campo del recurso cambia.

## Evidencia de éxito

La feature se considera validada cuando:

- la actualización modifica solo `status`
- el recurso actualizado se devuelve en la respuesta
- los casos inválidos responden con `400`
- los ids inexistentes responden con `404`
- la prueba manual queda documentada en `.http`
