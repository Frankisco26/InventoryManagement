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
