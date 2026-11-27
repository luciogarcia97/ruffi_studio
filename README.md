# Ruffi Calendarios

Catálogo, y más adelante ecommerce, de calendarios hechos a pedido. Ver [el plan](docs/PLAN.md).

Requisitos: .NET SDK 10, Node 22.22.3 o superior (rama 22) y Docker Desktop abierto.

## Levantar en desarrollo

La primera vez, creá el archivo de credenciales locales y completá las claves:

```powershell
Copy-Item .env.example .env
```

Desde la raíz, cada servidor en su propia terminal:

```powershell
docker compose up -d --wait
dotnet run --project backend/src/Ruffi.Api
```

```powershell
npm --prefix frontend install
npm --prefix frontend start
```

- Web: http://localhost:4200
- API: http://localhost:5044/api/catalogo
- MySQL: 127.0.0.1:3316

En desarrollo, la API lee `.env`, aplica las migraciones y carga productos de ejemplo si la base está vacía.
Las fotos de prueba van en `frontend/public/demo/`, que no se sube al repositorio.

## Tests

```powershell
dotnet test backend/Ruffi.slnx
npm --prefix frontend test -- --watch=false
```

## Nueva migración (desde `backend`)

```powershell
dotnet tool restore
dotnet ef migrations add Nombre -p src/Ruffi.Infrastructure -s src/Ruffi.Api -o Persistencia/Migraciones
```
