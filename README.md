# Inventory Management System

Project developed using a microservices architecture, designed to provide high scalability, availability, and ease of maintenance. The solution is composed of independent services, each responsible for a specific business functionality, allowing deployments and updates to be performed independently without affecting the rest of the system.


---

# Technologies Used

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

# Project Architecture

The system is divided into multiple layers to maintain a clean, scalable, and maintainable architecture.

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


# System Features

## Authentication and Security

- Login with ASP.NET Identity
- User Registration
- Roles and Permissions
- JWT Bearer Authentication
- Cookies Authentication
- API Protection
- Claims Authentication
- Password Hashing
- Authorization Policies


---

## Product Management

- Create Products
- Edit Products
- Delete Products
- View Products
- Stock Control
- Categories
- Descriptions
- Prices
- Minimum Inventory
  
---

## Inventory Movements

- Inventory Entries
- Inventory Outputs
- Movement History
- User Tracking per Movement
- Movement Date
- Movement Type
- Quantities

---

## REST APIs

- Product CRUD
- Movement CRUD
- JWT-Protected APIs
- Typed Responses
- DTOs
- Validations
- Error Handling

---

# Project Configuration

---

# Clone Repository

```bash
git clone https://github.com/Frankisco26/InventoryManagement.git
```

---

# Database Configuration

Modify the file:

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

Open:

```text
Package Manager Console
```

Run:

```powershell
Update-Database
```

---

# Initial Seed

The system automatically generates an administrator user on startup.

## Administrator User

```text
Correo:
admin@inventory.com

Contraseña:
Admin123*
```

---

# Run the Project

## From Visual Studio

```text
 Right-click on the Web project and select "Set as Startup Project". Then press F5.
```

---

## From CLI

```bash
dotnet run
```

---

# System URLs

## Web Application

```text
https://localhost:7001
```

---

## REST APIs

### Products

```text
https://localhost:7001/api/products
```

### Movements

```text
https://localhost:7001/api/stockmovements
```

---

# Security Architecture

The system uses two types of authentication.

---

# Blazor UI

Uses:

```text
ASP.NET Identity Cookies
```

To manage authenticated sessions in the user interface.

---

# APIs

Uses:

```text
JWT Bearer Authentication
Para proteger endpoints REST.
```

# Main Dependencies

```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.AspNetCore.Authentication.JwtBearer
System.IdentityModel.Tokens.Jwt
```

---

# Implemented Best Practices

- Layered Architecture
- Dependency Injection
- DTO Pattern
- Service Pattern
- JWT Authentication
- Entity Framework Core
- Async/Await
- Repository Pattern
- Claims Authentication
- Authorization
- Validations
- Separation of Concerns
- SOLID Principles

---


# Technical Structure

## Controllers

Responsible for exposing REST APIs.

---

## Services

Contain business logic.

---

## DTOs

Data Transfer Objects.

---

## Models

System entities.
---

## DbContext

Entity Framework Core management.

---

## Authentication

Identity and JWT management.

---

# FAuthentication Flow

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

# Tools Used

- Visual Studio 2022
- SQL Server
- Git
- GitHub
- Fork
- Insomnia

---

# Important Files

## Program.cs

Main system configuration.

---

## appsettings.json

General configurations.

---

## JwtService.cs

JWT token generation.

---

## AuthService.cs

Frontend authentication management.

---

## DbSeeder.cs

Initial loading of users and roles.

---

# Licencia

Educational and demonstration project.

---

# Autor

Francisco Aguirre

---

# Contacto

```text
faguirrea1@gmail.com - 0998095771
```

---

# Notes

This project was developed using:

- ASP.NET Core 8
- Blazor Server
- Clean Architecture
- JWT Authentication
- Identity Framework
- SQL Server

following modern enterprise software development best practices.
