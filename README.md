# 🎬 Catálogo de Películas — Proyecto Final

API REST en **ASP.NET Core 10 + Entity Framework Core + SQLite** que reemplaza el backend de NestJS de los talleres, sirviendo **los mismos endpoints** con autenticación **JWT**. La aplicación web de Vue (Taller #1) y la app móvil de Ionic (Taller #2) funcionan contra ella **sin cambios de lógica**: solo se cambia la URL base con la variable de entorno `VITE_API_URL`.

**Integrantes:** Gerónimo Gaviria y Daniel David León López.

## ¿Por qué cuatro carpetas?

| Carpeta | Qué es | Origen | Puerto |
| --- | --- | --- | --- |
| [`api-aspnet/`](api-aspnet) | ⭐ **La API nueva en ASP.NET Core** | Proyecto Final | `5080` |
| [`backend/`](backend) | La API original en NestJS + Prisma (referencia del contrato) | Taller #1 | `3000` |
| [`frontend/`](frontend) | Cliente web en Vue 3 + Vite | Taller #1 | `5173` |
| [`mobile/`](mobile) | Cliente móvil en Ionic Vue + Capacitor | Taller #2 | `8100` |

Para el proyecto final no se modificó nada en `backend/`, `frontend/` ni `mobile/`: todo el trabajo está en `api-aspnet/`.

---

## Requisitos

- **.NET SDK 10** ([instalador oficial](https://dotnet.microsoft.com/download) o en macOS `brew install dotnet`)
- **Node.js 20** o superior y npm
- Git

> ⚠️ Clona el repositorio en una ruta **sin el carácter `#`**: Vite falla si alguna carpeta de la ruta lo contiene.

---

## Cómo arrancar

### 1. API de ASP.NET Core

```bash
cd api-aspnet
dotnet tool restore                 # restaura dotnet-ef (versión fijada en dotnet-tools.json)
cd PeliculasApi
dotnet user-secrets set "Jwt:Secret" "un-secreto-largo-de-al-menos-32-caracteres"
dotnet run
```

- La API queda en **http://localhost:5080/api**.
- La primera vez crea `peliculas.db`, aplica las migraciones y carga **12 películas** y el **usuario demo**.
- El secreto del JWT **no está en el repositorio**: cada persona lo configura una vez con `dotnet user-secrets`. Debe tener **al menos 32 caracteres** (HS256 lo exige). Si falta, la API no arranca y muestra el comando para configurarlo.

### 2. Cliente web (Vue) contra la nueva API

```bash
cd frontend
npm install
VITE_API_URL=http://localhost:5080/api npm run dev
```

Abre **http://localhost:5173**. También puedes poner `VITE_API_URL=http://localhost:5080/api` en `frontend/.env`.

### 3. Cliente móvil (Ionic) contra la nueva API

```bash
cd mobile
npm install --legacy-peer-deps
VITE_API_URL=http://localhost:5080/api npm run dev -- --port 8100
```

Abre **http://localhost:8100**.

> `--legacy-peer-deps` es necesario porque `@capacitor/android` 8.6 pide `@capacitor/core` 8.6 y el proyecto usa 8.5.2. No se cambió `mobile/package.json` para respetar el taller.

En **Windows (PowerShell)**, la variable se define antes del comando:

```powershell
$env:VITE_API_URL="http://localhost:5080/api"; npm run dev
```

---

## Documentación interactiva (Scalar)

Con la API corriendo en desarrollo:

- **Scalar:** http://localhost:5080/scalar
- **Documento OpenAPI:** http://localhost:5080/openapi/v1.json

Para probar un endpoint protegido:

1. En Scalar, abre `POST /api/auth/login` y envía el usuario demo.
2. Copia el `access_token` de la respuesta.
3. Pégalo en **Authentication → Bearer Token**.
4. Prueba `GET /api/auth/me`, o crea, edita o elimina una película. Los endpoints protegidos aparecen con candado.

### Usuario demo

| Correo | Contraseña |
| --- | --- |
| `demo@peliculas.com` | `demo1234` |

Las peticiones de prueba también están en [`api-aspnet/PeliculasApi/PeliculasApi.http`](api-aspnet/PeliculasApi/PeliculasApi.http), para la extensión REST Client.

---

## Endpoints

Base: `http://localhost:5080/api`. Son los mismos de NestJS, con los mismos códigos de estado y la misma forma de JSON (propiedades en `camelCase`, salvo `access_token`).

### Autenticación

| Método | Ruta | Token | Éxito | Errores |
| --- | --- | --- | --- | --- |
| `POST` | `/auth/register` | No | `201` `{ access_token, user }` | `400` validación, `409` correo ya registrado |
| `POST` | `/auth/login` | No | `200` `{ access_token, user }` | `400` validación, `401` credenciales inválidas |
| `GET` | `/auth/me` | **Sí** | `200` `{ id, email, nombre }` | `401` sin token, token vencido o inválido |

### Películas

| Método | Ruta | Token | Éxito | Errores |
| --- | --- | --- | --- | --- |
| `GET` | `/movies?search=&page=&limit=` | No | `200` `{ data, meta }` | `400` parámetros inválidos |
| `GET` | `/movies/:id` | No | `200` película | `404` no existe |
| `POST` | `/movies` | **Sí** | `201` película | `400`, `401` |
| `PATCH` | `/movies/:id` | **Sí** | `200` película | `400`, `401`, `404` |
| `DELETE` | `/movies/:id` | **Sí** | `200` `{ id, eliminado: true }` | `401`, `404` |

Los endpoints con token exigen el header `Authorization: Bearer <access_token>`. Los errores tienen la misma forma que en NestJS:

```json
{ "message": "Credenciales invalidas", "error": "Unauthorized", "statusCode": 401 }
```

El contrato detallado está en [`docs/API.md`](docs/API.md).

---

## Estructura de `api-aspnet/`

```
api-aspnet/
├── dotnet-tools.json            # dotnet-ef 10.0.14 como herramienta local
└── PeliculasApi/
    ├── Program.cs               # Servicios (EF, DI, JWT, CORS, JSON, OpenAPI) y pipeline HTTP
    ├── appsettings.json         # Conexión SQLite, orígenes de CORS y duración del JWT
    ├── Models/                  # Movie y User (las entidades)
    ├── Data/                    # AppDbContext y SeedData (12 películas + usuario demo)
    ├── Migrations/              # InitialCreate (Movies) y AddUsers (Users)
    ├── Dtos/                    # Entrada validada y respuestas (Movies y Auth)
    ├── Services/                # MoviesService, AuthService y JwtOptions (logica de negocio)
    ├── Repositories/            # MovieRepository y UserRepository (consultas con Entity Framework)
    ├── Controllers/             # MoviesController (/api/movies) y AuthController (/api/auth)
    ├── Errors/                  # Errores con la forma { message, error, statusCode }
    ├── OpenApi/                 # Esquema Bearer para Scalar
    └── PeliculasApi.http        # Peticiones de prueba
```

### Cómo se cumple cada parte

| Parte | Cómo se resolvió |
| --- | --- |
| Montaje | `dotnet new webapi --use-controllers`, EF Core con SQLite y migraciones |
| CRUD | `MoviesController` → `MoviesService` → `MovieRepository` → `Movie`, cada capa por interfaz e inyección de dependencias, + DTOs con validación |
| Patrones | **Servicio** (lógica de negocio), **Repositorio** (solo él usa Entity Framework), **Inyección de dependencias** por interfaz y **DTO** (el modelo nunca sale tal cual) |
| Búsqueda y paginación | `?search=` con `LIKE` (sin distinguir mayúsculas, como Prisma) y `?page=&limit=` con `Skip`/`Take` |
| CORS | Orígenes de Vue e Ionic en `appsettings.json` (`Cors:Origins`) |
| Registro | `POST /api/auth/register` guarda la contraseña con **BCrypt de 10 rondas** |
| Login | `POST /api/auth/login` emite un **JWT HS256** con `sub` y `email`, válido 24 horas |
| Protección | `[Authorize]` en `POST`, `PATCH` y `DELETE` de películas y en `GET /auth/me` |
| Integración | Vue e Ionic solo cambian `VITE_API_URL` |
| OpenAPI | `Microsoft.AspNetCore.OpenApi` + **Scalar** con esquema Bearer |

### Comandos útiles

| Comando (en `api-aspnet/PeliculasApi`) | Qué hace |
| --- | --- |
| `dotnet run` | Levanta la API en el puerto 5080 |
| `dotnet build` | Compila |
| `dotnet ef migrations add <Nombre>` | Crea una migración |
| `dotnet ef database update` | Aplica las migraciones |
| `rm peliculas.db` | Borra la base; al volver a correr se recrea con el seed |

---

## API original de NestJS (opcional)

Para comparar respuestas con la API de los talleres:

```bash
cd backend
npm install
cp .env.example .env
npx prisma migrate dev
npm run prisma:seed
npm run start:dev
```

Queda en **http://localhost:3000/api**, con el mismo usuario demo.
