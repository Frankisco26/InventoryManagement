# Inventory Management System

Es un Sistema utilizado para evaluar el conocimiento técnico de un desarrollador senior,desarrollado con tecnologías modernas de Microsoft usando ASP.NET Core 8 y Blazor Server.

---

# Tecnologías Utilizadas

- ASP.NET Core 8
- Blazor Server
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- Bootstrap 5
- Bootstrap Icons
- C#
- LINQ
- REST APIs

---

# Arquitectura del Proyecto

El sistema está dividido en múltiples capas para mantener una arquitectura limpia, escalable y mantenible.

InventoryManagement
│
├── Web
│   ├── Components
│   ├── Controllers
│   ├── Services
│   ├── Pages
│   ├── Authentication
│   ├── Layout
│   └── wwwroot
│
├── Data
│   ├── Data
│   ├── DTOs
│   ├── Services
│   └── Migrations
│
└── Models


# Características del Sistema

## Autenticación y Seguridad

- Login con ASP.NET Identity
- Registro de usuarios
- Roles y permisos
- JWT Bearer Authentication
- Cookies Authentication
- Protección de APIs
- Claims Authentication
- Password hashing
- Authorization Policies

---

## Gestión de Productos

- Crear productos
- Editar productos
- Eliminar productos
- Consultar productos
- Control de stock
- Categorías
- Descripciones
- Precios
- Inventario mínimo

---

## Movimientos de Inventario

- Entradas de inventario
- Salidas de inventario
- Historial de movimientos
- Control de usuarios por movimiento
- Fecha de movimiento
- Tipo de movimiento
- Cantidades

---

## APIs REST

- CRUD Productos
- CRUD Movimientos
- APIs protegidas con JWT
- Responses tipadas
- DTOs
- Validaciones
- Control de errores

---

# Configuración del Proyecto

---

# Clonar Repositorio

```bash
git clone https://github.com/Frankisco26/InventoryManagement.git
```

---

# Configuración Base de Datos

Modificar el archivo:

```
appsettings.json
```

# Connection String

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=localhost;Initial Catalog=InventoryManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
}
```

---

# 🔑 Configuración JWT

```json
"Jwt": {
  "Key": "qwertrtyuikjghgfdsazxcvgsdf4555465gfdsgt786876786fsfsdg786dg78g6rew23r34f3t54gtrvbt5",
  "Issuer": "InventoryApi",
  "Audience": "InventoryClient",
  "DurationInMinutes": 60
}
```

---

# 🧱 Migraciones

Abrir:

```text
Package Manager Console
```

Ejecutar:

```powershell
Update-Database
```

---

# 🌱 Seed Inicial

El sistema genera automáticamente un usuario administrador al iniciar.

## 👤 Usuario Administrador

```text
Correo:
admin@inventory.com

Contraseña:
Admin123*
```

---

# ▶️ Ejecutar Proyecto

## Desde Visual Studio

```text
F5
```

---

## Desde CLI

```bash
dotnet run
```

---

# 🌐 URLs del Sistema

## Aplicación Web

```text
https://localhost:7001
```

---

## APIs REST

### Productos

```text
https://localhost:7001/api/products
```

### Movimientos

```text
https://localhost:7001/api/stockmovements
```

---

# Arquitectura de Seguridad

El sistema usa dos tipos de autenticación.

---

# Blazor UI

Usa:

```text
ASP.NET Identity Cookies
```

Para manejar sesiones autenticadas en la interfaz.

---

# APIs

Usa:

```text
JWT Bearer Authentication
```

Para proteger endpoints REST.

```

---

# Dependencias Principales

```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.AspNetCore.Authentication.JwtBearer
System.IdentityModel.Tokens.Jwt
```

---

# Buenas Prácticas Implementadas

- Arquitectura por capas
- Dependency Injection
- DTO Pattern
- Service Pattern
- JWT Authentication
- Entity Framework Core
- Async/Await
- Repository Pattern
- Claims Authentication
- Authorization
- Validaciones
- Separation of Concerns
- SOLID Principles

---


# 📚 Estructura Técnica

## Controllers

Responsables de exponer APIs REST.

---

## Services

Contienen lógica de negocio.

---

## DTOs

Objetos de transferencia de datos.

---

## Models

Entidades del sistema.

---

## DbContext

Manejo de Entity Framework Core.

---

## Authentication

Manejo de Identity y JWT.

---

# Flujo de Autenticación

```text
Blazor Login
↓
ASP.NET Identity
↓
Cookie Authentication
↓
JWT Generation
↓
Bearer Token
↓
Protected APIs
↓
SQL Server
```

---

# Herramientas Utilizadas

- Visual Studio 2022
- SQL Server
- Git
- GitHub
- Fork
- Insomnia

---

# Archivos Importantes

## Program.cs

Configuración principal del sistema.

---

## appsettings.json

Configuraciones generales.

---

## JwtService.cs

Generación de JWT Tokens.

---

## AuthService.cs

Manejo de autenticación del frontend.

---

## DbSeeder.cs

Carga inicial de usuarios y roles.

---

# Licencia

Proyecto de uso educativo y demostrativo.

---

# Autor

Francisco Aguirre

---

# Contacto

```text
faguirrea1@gmail.com - 0998095771
```

---

# ⭐ Notas

Este proyecto fue desarrollado usando:

- ASP.NET Core 8
- Blazor Server
- Clean Architecture
- JWT Authentication
- Identity Framework
- SQL Server

siguiendo buenas prácticas modernas de desarrollo empresarial.