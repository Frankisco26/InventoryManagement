# Inventory Management System

Es un Sistema utilizado para evaluar el conocimiento técnico de un desarrollador senior, desarrollado con tecnologías modernas de Microsoft usando ASP.NET Core 10 MVC y Bootstrap, con la misma arquitectura multiempresa de Contagestor.

---

# Technologies Used

- ASP.NET Core 10 (MVC + Razor Pages para Identity)
- Entity Framework Core 10
- SQL Server
- ASP.NET Identity
- JWT Authentication (API REST)
- Bootstrap 4, jQuery, DataTables, SweetAlert, Font Awesome
- C#
- LINQ
- REST APIs

---

# Project Architecture

The system is divided into multiple layers to maintain a clean, scalable, and maintainable architecture.

```text
InventoryManagement
│
├── Web
│   ├── Areas
│   │   ├── Identity        (Login, Logout, AccessDenied, Lockout)
│   │   ├── Configuracion   (Panel inicial, Empresas, Usuarios, Permisos)
│   │   └── Stock           (Productos, Movimientos)
│   ├── Controllers         (Principal y API REST)
│   ├── Extensiones         (Autorización por módulo, sesión, identidad)
│   ├── Services            (Empresas del usuario, JWT)
│   ├── Views               (Layouts, menú lateral, selector de empresa)
│   └── wwwroot
│
├── Data
│   ├── Base                (ContextoIdentidad, ContextoPrincipal, datos iniciales)
│   ├── Dtos
│   ├── Services
│   └── Migraciones
│       ├── Identidad
│       └── Principal
│
└── Models
    ├── Identidad           (Usuario, Rol, Pantalla, RolPantalla...)
    ├── Administracion      (Empresa, Modulo, Parametro)
    ├── Utilitarios
    └── Models              (Product, StockMovement)
```

---

# System Features

## Multiempresa

- Cada producto pertenece a una empresa; todas las consultas filtran por la empresa activa.
- Un usuario puede tener roles en varias empresas y cambiar entre ellas desde el menú de usuario ("Cambiar empresa").
- La empresa activa se guarda en la sesión con sus módulos y las pantallas que permite el rol.

## Autenticación y Seguridad

- Login con ASP.NET Identity (contexto `ContextoIdentidad`, esquema `db_identidad`)
- Roles por empresa con asignación de pantallas y permisos de crear / editar / eliminar
- Autorización por módulo: el módulo debe estar activo en la empresa y la pantalla asignada al rol
- Bloqueo y desbloqueo de usuarios
- JWT Bearer Authentication para la API
- Cookies Authentication para la aplicación web

## Configuración

- Empresas: crear, editar, bloquear y elegir sus módulos. Al crear una empresa se genera su rol Administrador y su usuario administrador (usuario = correo de la empresa, contraseña = primeros 10 dígitos del RUC).
- Usuarios: crear, editar, asignar empresas/roles, bloquear, habilitar y restablecer contraseña.
- Permisos: roles por empresa con sus pantallas.

## Gestión de Productos

- Crear, editar, consultar y eliminar productos
- Control de stock y stock mínimo
- Categorías y precios

## Product Management

- Entradas y salidas de inventario
- Historial de movimientos por producto
- Validación de stock insuficiente

## REST APIs

- `POST api/auth/token` para obtener el token de una empresa del usuario
- CRUD Productos y movimientos (`api/products`, `api/products/{id}/movements`)
- APIs protegidas con JWT, limitadas a la empresa del token

---

# Project Configuration

# Clonar Repositorio

```bash
git clone https://github.com/Frankisco26/InventoryManagement.git
```

---

# Database Configuration

Modificar el archivo `Web/appsettings.json` (y `appsettings.Development.json`):

```json
"ConnectionStrings": {
  "ContextoIdentidad": "Data Source=localhost;Initial Catalog=InventoryManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
  "ContextoPrincipal": "Data Source=localhost;Initial Catalog=InventoryManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
}
```

---

# Configuración JWT

```json
"Jwt": {
  "Key": "qwertrtyuikjghgfdsazxcvgsdf4555465gfdsgt786876786fsfsdg786dg78g6rew23r34f3t54gtrvbt5",
  "Issuer": "InventoryApi",
  "Audience": "InventoryClient",
  "DurationInMinutes": 60
}
```

---

# Migrations

Igual que en Contagestor, se usa el script `migrate.ps1` en la raíz (instala `dotnet-ef` desde `dotnet-tools.json`). El parámetro `-context` es `Identidad` o `Principal` (por defecto `Principal`):

```powershell
./migrate.ps1 -context Identidad -update               # aplica las migraciones de identidad
./migrate.ps1 -context Principal -update               # aplica las migraciones principales
./migrate.ps1 -context Principal -name NuevoCambio     # crea una migración
./migrate.ps1 -context Principal -name NuevoCambio -update
./migrate.ps1 -context Principal -rollback             # quita la última migración
./migrate.ps1 -context Principal -output Release       # genera el script SQL en Data/Migraciones/Principal/Scripts
```

---

# Datos Iniciales

Las migraciones siembran:

- Empresa principal `1250130620001` con los módulos Configuración y Stock
- Pantallas: Empresas, Usuarios, Permisos, Productos y Movimientos
- Rol `Administrador` de la empresa principal con todas las pantallas
- Usuario principal `MANTENIMIENTO` (usuario `1727495366001`), el mismo de Contagestor

---

# Run the Project

## From Visual Studio

```text
 Right-click on the Web project and select "Set as Startup Project". Then press F5.
```

## Desde CLI

```bash
dotnet run --project Web
```

---

# System URLs

## Web Application

```text
https://localhost:7001
```
