# WebApp API

<div align="center">

A professional e-commerce Web API built with ASP.NET Core, featuring product management, JWT authentication, shopping cart, and order processing.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

</div>

---

## 📋 Table of Contents

- [✨ Key Features](#-key-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [📦 Prerequisites](#-prerequisites)
- [🚀 Getting Started](#-getting-started)
- [⚙️ Configuration](#️-configuration)
- [📡 API Documentation](#-api-documentation)
- [🏗️ Architecture](#️-architecture)
- [📂 Project Structure](#-project-structure)
- [🔐 Security](#-security)
- [💾 Database Models](#-database-models)
- [📊 Error Handling](#-error-handling)
- [📝 License](#-license)

---

## ✨ Key Features

- **Product Management**: Complete CRUD operations with filtering, sorting, and pagination
- **JWT Authentication**: Secure user registration and login with token-based auth
- **Shopping Cart**: Add, update, remove items, and view cart contents
- **Order Processing**: Checkout functionality with order history
- **Caching**: Redis distributed caching for high-performance data retrieval
- **API Documentation**: Interactive Swagger/OpenAPI UI
- **Input Validation**: FluentValidation for robust request validation
- **Structured Logging**: Serilog with console and file sinks
- **Centralized Error Handling**: Custom middleware for consistent error responses

---

## 🛠️ Tech Stack

| Layer | Technology | Version |
|-------|------------|---------|
| Framework | ASP.NET Core | 10.0 |
| Database | SQL Server | LocalDB (dev) / SQL Server (prod) |
| ORM | Entity Framework Core | 10.0.9 |
| Caching | StackExchange Redis | 10.0.10 |
| Object Mapping | AutoMapper | 12.0.0 |
| Validation | FluentValidation | 11.3.1 |
| Logging | Serilog | 6.x |
| API Docs | Swashbuckle.AspNetCore | 10.2.3 |
| Password Hashing | BCrypt.Net-Next | 4.2.0 |

---

## � Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB is included with Visual Studio)
- [Redis Server](https://redis.io/download) (for caching, optional for basic development)

---

## � Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/GiorgiKavtaradze-prog/WebApp.Api.git
cd WebApp.Api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Application

Update [appsettings.json](file:///d:/add-project/WebApp.Api/WebApp.Api/appsettings.json) with your settings (see [Configuration](#️-configuration) section)

### 4. Apply Database Migrations

```bash
cd WebApp.Api
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7000`
- Swagger UI: `https://localhost:7000/swagger`

---

## ⚙️ Configuration

### Connection Strings

Update the database connection string in [appsettings.json](file:///d:/add-project/WebApp.Api/WebApp.Api/appsettings.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebApp;Trusted_Connection=True;MultipleActiveResultSets=True;"
  }
}
```

### JWT Settings

Configure JWT authentication:

```json
{
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "WebApp",
    "Audience": "WebAppUsers",
    "ExpiresInMinutes": 60
  }
}
```

### Redis Configuration

Redis caching is configured in [Program.cs](file:///d:/add-project/WebApp.Api/WebApp.Api/Program.cs#L76-L79):

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

---

## 📡 API Documentation

Full interactive API documentation is available via Swagger UI at `/swagger` when running in development mode.

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and receive JWT token | No |

### Products Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/products` | Get all products (pagination, filtering) | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| POST | `/api/products` | Create a new product | Yes |
| PUT | `/api/products/{id}` | Update an existing product | Yes |
| DELETE | `/api/products/{id}` | Delete a product | Yes |

### Cart Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/cart` | Get current user's cart | Yes |
| POST | `/api/cart` | Add item to cart | Yes |
| PUT | `/api/cart/{productId}` | Update cart item quantity | Yes |
| DELETE | `/api/cart/{productId}` | Remove item from cart | Yes |

### Orders Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/orders/checkout` | Create order from cart | Yes |
| GET | `/api/orders/my` | Get current user's orders | Yes |
| GET | `/api/orders` | Get all orders (admin only) | Yes |
| PUT | `/api/orders/{orderId}/status` | Update order status (admin only) | Yes |

---

## 🏗️ Architecture

The application follows a clean, layered architecture:

```
┌─────────────────────────────────────────┐
│         Controllers (API Layer)         │
│  - Handles HTTP requests/responses      │
│  - Input validation                     │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         Services (Business Logic)       │
│  - Domain logic                         │
│  - Service interfaces                   │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         Data Layer (EF Core)            │
│  - Database context                     │
│  - Migrations                           │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│         SQL Server / Redis              │
│  - Persistence & caching                │
└─────────────────────────────────────────┘
```

---

## � Project Structure

```
WebApp.Api/
├── Controllers/                    # API Controllers
│   ├── AuthController.cs          # Authentication endpoints
│   ├── CartController.cs          # Cart management
│   ├── OrdersController.cs        # Order processing
│   └── ProductsController.cs      # Product CRUD
├── Data/                          # Data access layer
│   └── AppDbContext.cs            # EF Core DbContext
├── DTOs/                          # Data Transfer Objects
│   ├── AddToCartDto.cs
│   ├── CreateProductDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── ... (validators included)
├── Mappings/                      # AutoMapper profiles
│   └── ProductProfile.cs
├── Middleware/                    # Custom middleware
│   └── ExceptionHandlingMiddleware.cs
├── Migrations/                    # EF Core database migrations
├── Models/                        # Domain entities
│   ├── Product.cs
│   ├── User.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Responses/                     # API response models
│   └── ApiErrorResponse.cs
├── Services/                      # Business logic layer
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ICartService.cs
│   │   ├── IOrderService.cs
│   │   └── IProductService.cs
│   ├── AuthService.cs
│   ├── CartService.cs
│   ├── OrderService.cs
│   └── ProductService.cs
├── Program.cs                     # Application entry point
├── appsettings.json               # Configuration
└── WebApp.Api.csproj              # Project file
```

---

## 🔐 Security

- **JWT Bearer Authentication**: Stateless token-based authentication
- **Password Hashing**: BCrypt.Net-Next for secure password storage
- **Input Validation**: FluentValidation for all request DTOs
- **Centralized Error Handling**: Sensitive error details only in development
- **HTTPS Redirection**: Enforced in production

---

## � Database Models

### Product
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| Name | string | Required, MaxLength(100) | Product name |
| Description | string | MaxLength(100) | Product description |
| Price | decimal | Range(0.01, 1000000), decimal(18,2) | Product price |
| Stock | int | Range(0, int.MaxValue) | Available quantity |
| CreatedAt | DateTime | - | Creation timestamp (UTC) |

### User
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| FullName | string | Required | User's full name |
| Email | string | Required, Unique | User's email address |
| PasswordHash | string | Required | BCrypt hashed password |
| Role | string | Default: "User" | User role |
| CreatedAt | DateTime | - | Registration timestamp (UTC) |

### Cart
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| UserId | int | FK → User | Associated user |
| User | User | Navigation | Navigation property |
| Items | List<CartItem> | - | Cart items collection |

### CartItem
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| CartId | int | FK → Cart | Parent cart |
| ProductId | int | FK → Product | Associated product |
| Quantity | int | ≥ 1 | Item quantity |

### Order
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| UserId | int | FK → User | Associated user |
| User | User | Navigation | Navigation property |
| CreatedAt | DateTime | - | Order creation timestamp (UTC) |
| Status | OrderStatus | Default: Pending | Order status |
| TotalAmount | decimal | - | Total order amount |
| Items | List<OrderItem> | - | Order items collection |

### OrderItem
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | int | PK | Primary key |
| OrderId | int | FK → Order | Parent order |
| ProductId | int | FK → Product | Associated product |
| ProductName | string | - | Product snapshot |
| UnitPrice | decimal | - | Price at time of order |
| Quantity | int | ≥ 1 | Item quantity |
| Total | decimal | - | Calculated total |

---

## 📊 Error Handling

The API uses centralized exception handling middleware ([ExceptionHandlingMiddleware.cs](file:///d:/add-project/WebApp.Api/WebApp.Api/Middleware/ExceptionHandlingMiddleware.cs)) that:

- Catches all unhandled exceptions
- Returns consistent error responses in `ApiErrorResponse` format
- Logs errors using Serilog
- Includes detailed error information only in development environment

### Error Response Format

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "details": "Detailed error information (development only)"
}
```

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<div align="center">
Made with ❤️ by <a href="https://github.com/GiorgiKavtaradze-prog">Giorgi Kavtaradze</a>
</div>
