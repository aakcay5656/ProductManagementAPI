# 🛍️ Product Management API

**Enterprise-grade RESTful API built with Clean Architecture principles**

A comprehensive Product Management system featuring JWT authentication, Redis caching, and PostgreSQL database integration. Built using CQRS pattern with MediatR for scalable and maintainable code architecture.

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7.0-red.svg)](https://redis.io/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 📋 Table of Contents

- [🛍️ Product Management API](#️-product-management-api)
  - [📋 Table of Contents](#-table-of-contents)
  - [✨ Features](#-features)
  - [🏗️ Architecture](#️-architecture)
  - [🛠️ Technology Stack](#️-technology-stack)
  - [📁 Project Structure](#-project-structure)
  - [🚀 Getting Started](#-getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Configuration](#configuration)
    - [Database Setup](#database-setup)
    - [Running the Application](#running-the-application)
  - [📚 API Documentation](#-api-documentation)
    - [Authentication Endpoints](#authentication-endpoints)
    - [Product Endpoints](#product-endpoints)
  - [🧪 Testing the API](#-testing-the-api)
  - [🐳 Docker Support](#-docker-support)
  - [🔧 Development](#-development)
  - [📊 Performance Features](#-performance-features)
  - [🛡️ Security](#️-security)


## ✨ Features

- **🔐 JWT Authentication** - Secure user registration and login
- **📦 Product Management** - Full CRUD operations for products
- **⚡ Redis Caching** - High-performance data caching with TTL
- **🗄️ PostgreSQL Database** - Robust relational database with EF Core
- **🎯 CQRS Pattern** - Command Query Responsibility Segregation
- **✅ Input Validation** - FluentValidation with pipeline behavior
- **📊 Swagger Documentation** - Interactive API documentation
- **🔄 Unit of Work** - Transaction management
- **🎨 Clean Architecture** - Onion Architecture implementation
- **📝 Structured Logging** - Serilog integration
- **🚦 Error Handling** - Global exception handling middleware

## 🏗️ Architecture

This project follows **Clean Architecture** (Onion Architecture) principles:

```
┌─────────────────────────────────────────────┐
│                  🌐 API Layer                │
│            (Controllers, Middleware)        │
├─────────────────────────────────────────────┤
│              📋 Application Layer           │
│         (CQRS, Handlers, DTOs, Validation) │
├─────────────────────────────────────────────┤
│             🏗️ Infrastructure Layer         │
│    (EF Core, Redis, JWT, Repositories)     │
├─────────────────────────────────────────────┤
│                💎 Core Layer                │
│         (Entities, Interfaces, Enums)      │
└─────────────────────────────────────────────┘
```

## 🛠️ Technology Stack

### Backend
- **.NET 9** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping

### Database & Caching
- **PostgreSQL 16** - Primary database
- **Redis 7** - Caching and session storage

### Security & Authentication
- **JWT Bearer Token** - Authentication
- **PBKDF2** - Password hashing
- **HTTPS** - Secure communication

### Tools & Libraries
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging
- **Npgsql** - PostgreSQL .NET driver
- **StackExchange.Redis** - Redis client

## 📁 Project Structure

```
ProductManagementAPI/
│   ├── 📂 ProductManagement.API/          # 🌐 Presentation Layer
│   │   ├── Controllers/                   # API Controllers
│   │   └── Middleware/                    # Custom middleware
│   │   
│   │
│   ├── 📂 ProductManagement.Application/  # 📋 Application Layer
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Features/                     # CQRS Commands & Queries
│   │   │   ├── Auth/                     # Authentication features
│   │   │   └── Products/                 # Product features
│   │   └── Validators/                   # FluentValidation validators
│   │
│   ├── 📂 ProductManagement.Infrastructure/ # 🏗️ Infrastructure Layer
│   │   ├── Data/                         # Database context
│   │   ├── Repositories/                 # Repository implementations
│   │   ├── Services/                     # Service implementations
│   │   └── Cache/                        # Redis cache service
│   │
│   └── 📂 ProductManagement.Core/         # 💎 Core Layer
│       ├── Entities/                     # Domain entities
│       ├── Interfaces/                   # Repository interfaces
│       ├── Enums/                        # Enumerations
│       └── Common/                       # Common utilities
│
└── 📄 .gitignore                         # Git ignore rules
```

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL 16+** - [Download here](https://www.postgresql.org/download/)
- **Redis 7+** - [Download here](https://redis.io/download) or use Docker
- **Git** - [Download here](https://git-scm.com/downloads)

### Installation

1. **Clone the repository**
   ```
   git clone https://github.com/aakcay5656/ProductManagementAPI.git
   cd ProductManagementAPI
   ```

2. **Restore dependencies**
   ```
   dotnet restore
   ```

### Configuration

1. **Update connection strings in `appsettings.json`:**
   ```
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5433;Database=ProductManagementDB;Username=postgres;Password=yourpassword;",
       "Redis": "localhost:6379"
     },
     "Jwt": {
       "Secret": "your-super-secret-jwt-key-that-should-be-at-least-64-characters-long",
       "Issuer": "ProductManagementAPI",
       "Audience": "ProductManagementAPI",
       "ExpirationMinutes": "1440"
     }
   }
   ```

### Database Setup

1. **Start PostgreSQL service**
   ```
   # Windows
   net start postgresql-x64-16
   
   # macOS
   brew services start postgresql@16
   
   # Linux
   sudo systemctl start postgresql
   ```

2. **Start Redis service**
   ```
   # Windows
   redis-server
   
   # macOS
   brew services start redis
   
   # Linux
   sudo systemctl start redis
   
   # Docker (alternative)
   docker run -d -p 6379:6379 redis:7-alpine
   ```

3. **Database will be created automatically on first run**

### Running the Application

```
# Development
dotnet run --project src/ProductManagement.API

# Production
dotnet run --project src/ProductManagement.API --environment Production
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/v1/auth/register` | Register a new user |
| `POST` | `/api/v1/auth/login` | Authenticate user and get JWT token |

### Product Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/v1/products` | Get all products (with filtering) | ❌ |
| `GET` | `/api/v1/products/{id}` | Get product by ID | ❌ |
| `POST` | `/api/v1/products` | Create new product | ✅ |
| `PUT` | `/api/v1/products/{id}` | Update product | ✅ |
| `DELETE` | `/api/v1/products/{id}` | Delete product | ✅ |

**Query Parameters for GET `/api/v1/products`:**
- `category` - Filter by category
- `search` - Search in name and description
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

## 🧪 Testing the API

### Using Swagger UI
1. Navigate to `https://localhost:5001/swagger`
2. Register a new user
3. Login to get JWT token
4. Click "Authorize" and enter: `Bearer {your-jwt-token}`
5. Test product endpoints

### Using cURL

**Register User:**
```
curl -X POST "https://localhost:5001/api/v1/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

**Login:**
```
curl -X POST "https://localhost:5001/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

**Create Product:**
```
curl -X POST "https://localhost:5001/api/v1/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "name": "iPhone 15",
    "description": "Latest iPhone model",
    "price": 999.99,
    "stock": 50,
    "category": "Electronics"
  }'
```


## 🔧 Development

### Running with Hot Reload
```
dotnet watch run --project src/ProductManagement.API
```


### Code Formatting
```
dotnet format
```

## 📊 Performance Features

- **Redis Caching**: Product lists cached for 5 minutes
- **Query Optimization**: EF Core include statements for related data
- **Pagination**: Efficient data loading with page-based results
- **Connection Pooling**: Database connection optimization
- **Async Operations**: Non-blocking database operations

## 🛡️ Security

- **JWT Authentication**: Stateless authentication with configurable expiration
- **Password Hashing**: PBKDF2 with salt for secure password storage
- **Input Validation**: Comprehensive validation using FluentValidation
- **SQL Injection Protection**: Entity Framework parameterized queries
- **HTTPS Enforcement**: Secure communication in production
- **CORS Configuration**: Cross-origin request management

