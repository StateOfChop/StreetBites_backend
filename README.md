# StreetBites Backend

API REST para sistema de gestión de pedidos de comida rápida.

## 🚀 Tecnologías

- **.NET 8** Web API
- **Entity Framework Core** con PostgreSQL
- **JWT** para autenticación
- **BCrypt** para hash de contraseñas
- **Docker** y Docker Compose

## 📋 Requisitos Previos

- .NET 8 SDK
- Docker y Docker Compose
- (Opcional) PostgreSQL local

## 🛠️ Instalación con Docker

```bash
# Levantar todos los servicios (PostgreSQL + API)
docker-compose up --build
```

La API estará disponible en `http://localhost:5000`
Swagger UI en `http://localhost:5000/swagger`

### ✨ Características Automáticas

Al iniciar con Docker:
- ✅ Se aplican las migraciones automáticamente
- ✅ Se crea un usuario admin por defecto

## 🔐 Credenciales por Defecto

| Rol   | Email                  | Password  |
|-------|------------------------|-----------|
| Admin | admin@streetbites.com  | Admin123! |

## 📁 Estructura del Proyecto

```
├── Controllers/         # Endpoints de la API
│   ├── AuthController   # Login, registro
│   ├── ProductsController
│   └── OrdersController
├── Models/              # Entidades del dominio
├── Dtos/                # Data Transfer Objects
├── Data/                # DbContext y configuración
├── Repositories/        # Acceso a datos
├── Services/            # Lógica de negocio
├── Interfaces/          # Contratos
└── Migrations/          # Migraciones de EF Core
```

## 📡 Endpoints

### Autenticación
| Método | Endpoint            | Descripción       |
|--------|---------------------|-------------------|
| POST   | /api/Auth/register  | Registrar usuario |
| POST   | /api/Auth/login     | Iniciar sesión    |

### Productos
| Método | Endpoint             | Rol   | Descripción          |
|--------|----------------------|-------|----------------------|
| GET    | /api/Products        | USER  | Productos activos    |
| GET    | /api/Products?includeInactive=true | ADMIN | Todos los productos |
| POST   | /api/Products        | ADMIN | Crear producto       |
| PUT    | /api/Products/{id}   | ADMIN | Actualizar producto  |
| DELETE | /api/Products/{id}   | ADMIN | Desactivar producto  |

### Pedidos
| Método | Endpoint                  | Rol   | Descripción           |
|--------|---------------------------|-------|-----------------------|
| GET    | /api/Orders               | ALL   | Listar pedidos        |
| GET    | /api/Orders/{id}          | ALL   | Detalle de pedido     |
| POST   | /api/Orders               | USER  | Crear pedido          |
| PUT    | /api/Orders/{id}/status   | ADMIN | Cambiar estado        |
| PUT    | /api/Orders/{id}/cancel   | USER  | Cancelar (si PENDING) |

## ⚙️ Variables de Entorno

```yaml
ConnectionStrings__DefaultConnection: "Server=...;Database=...;User Id=...;Password=..."
Jwt__Key: "tu-clave-secreta-min-32-caracteres"
Jwt__Issuer: "FastFoodApi"
Jwt__Audience: "FastFoodFront"
```

## 🔧 Desarrollo Local (sin Docker)

```bash
# Restaurar dependencias
dotnet restore

# Crear migración (si hay cambios en modelos)
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Ejecutar
dotnet run
```

## 📝 Licencia

MIT
