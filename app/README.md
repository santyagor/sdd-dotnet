# Realtor Solution Foundation

## Overview

Esta solución contiene la base inicial para el proyecto Realtor:

- `app/backend/src/RealtorApi/`: ASP.NET Core Minimal API backend
- `app/backend/tests/RealtorApiTests/`: backend xUnit tests
- `app/frontend/src/RealtorWeb/`: Blazor Server Web App frontend
- `app/frontend/test/RealtorWeb/`: frontend xUnit/BUnit tests

## Build

Desde la raíz del repositorio:

```powershell
cd /d d:\ProyGitPersonal\sdd
dotnet build app/
```

## Test

```powershell
dotnet test app/
```

## Project structure

- `app/Realtor.sln`: solución que agrupa backend, frontend y pruebas
- `app/.gitignore`: exclusiones locales para compilación .NET
- `app/backend/src/RealtorApi/`: backend base sin lógica de negocio
- `app/frontend/src/RealtorWeb/`: frontend base Blazor Server

## Notes

- El proyecto usa `.NET 11` definido en `global.json`
- Esta fase es de fundación: no incluye lógica de negocio
- La estructura y los tests placeholder permiten validar la solución base
