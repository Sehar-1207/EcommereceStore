<h1 align="center"> A mini Commerce Store with High-Performance Microservices Backend. </h1>
<p align="center"> An enterprise-grade, scalable, and secure microservices-based ecommerce ecosystem built with .NET Core, Ocelot Gateway, and Clean Architecture principles. </p>

<p align="center">
  <img alt="Build" src="https://img.shields.io/badge/Build-Passing-brightgreen?style=for-the-badge">
  <img alt="Issues" src="https://img.shields.io/badge/Issues-0%20Open-blue?style=for-the-badge">
  <img alt="Contributions" src="https://img.shields.io/badge/Contributions-Welcome-orange?style=for-the-badge">
  <img alt="License" src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge">
</p>
<!-- 
  **Note:** These are static placeholder badges. Replace them with your project's actual badges.
  You can generate your own at https://shields.io
-->

## 📌 Table of Contents
- [Overview](#-overview)
- [Key Features](#-key-features)
- [Tech Stack & Architecture](#-tech-stack--architecture)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Usage](#-usage)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🌟 Overview

### Hook
Omni Commerce is a robust backend solution designed to power modern retail experiences through a modular microservices architecture, ensuring high availability, secure authentication, and seamless product management.

### The Problem
> Scaling an ecommerce platform often leads to monolithic bottlenecks where authentication logic, inventory management, and API routing become tightly coupled. This results in fragile deployments, difficult maintenance, and challenges in scaling specific services (like high-traffic product browsing) independently from sensitive services (like user authentication).

### The Solution
OmniCommerce solves these challenges by decoupling core business domains into independent microservices. By utilizing an **Ocelot API Gateway**, the system provides a unified entry point for clients while maintaining a distributed architecture. Each service—**Authentication** and **Product Catalogue**—follows Clean Architecture patterns, ensuring that business logic remains independent of external frameworks and databases.

### Architecture Overview
The system is built on **ASP.NET Core** and organized into several specialized layers:
- **Presentation Layer:** Handles HTTP requests and controller logic.
- **Application Layer:** Contains DTOs, mapping configurations, and service interfaces.
- **Domain Layer:** Defines the core entities and business rules.
- **Infrastructure Layer:** Manages data persistence through Entity Framework Core and the Repository pattern.
- **Shared Library:** Encapsulates cross-cutting concerns like global exception handling and JWT authentication.

---

## ✨ Key Features

### 🔐 Secure Identity Management
The Authentication service provides a comprehensive security framework.
- **Centralized Auth:** Leverage JWT-based authentication to secure endpoints across the entire microservices mesh.
- **User DTOs:** Streamlined data transfer for login and user registration, ensuring only necessary data is exposed.
- **Identity Integration:** Built on top of ASP.NET Core Identity for robust user and role management.

### 📦 Dynamic Product Cataloguing
A highly organized system for managing complex retail inventories.
- **Hierarchical Categorization:** Organize products into categories and brands for enhanced searchability.
- **Decoupled Repositories:** Use the Repository pattern to abstract data access, making the system database-agnostic.
- **Automated Mapping:** Utilize AutoMapper (via `MappingConfiguration`) to seamlessly convert domain entities into client-ready DTOs.

### 🛣️ Intelligent API Routing
The Gateway service acts as the traffic controller for the entire ecosystem.
- **Ocelot Orchestration:** Route client requests to the appropriate microservice based on the `Ocelot.json` configuration.
- **Header Enrichment:** Custom middleware (`AttachedSignedHeader.cs`) ensures that internal requests are properly signed and identified.
- **Unified Interface:** Clients interact with a single URL, hiding the complexity of the underlying microservices.

### 🛠️ Enterprise Cross-Cutting Concerns
The Shared Library (`ClassLibrary1`) ensures consistency across all services.
- **Global Exception Handling:** A centralized middleware to capture and format errors, providing a consistent API response structure.
- **Standardized Responses:** Unified `ApiResponse.cs` ensures that all services return data in a predictable format for frontend consumption.
- **Gateway Awareness:** Specialized middleware to ensure services only respond to requests routed through the authorized Gateway.

---

## 🛠️ Tech Stack & Architecture

### Verified Technologies

| Technology | Purpose | Why it was Chosen |
| :--- | :--- | :--- |
| **.NET / C#** | Primary Language & Framework | Provides enterprise-level performance, type safety, and a rich ecosystem for microservices. |
| **Ocelot** | API Gateway | Lightweight and powerful routing middleware designed specifically for .NET microservices. |
| **Entity Framework Core** | ORM / Data Access | Simplifies database interactions and supports migrations for version-controlled schema updates. |
| **JWT (JSON Web Tokens)** | Authentication | Enables stateless, scalable authentication across distributed services. |
| **AutoMapper** | Object Mapping | Reduces boilerplate code by automating the conversion between Entities and DTOs. |
| **ASP.NET Core Identity** | Identity Management | A proven framework for handling user accounts, passwords, and roles. |

---

## 📁 Project Structure

Commerce/
├── 📄 EcommereceStore.sln             # Main solution file
├── 📂 Gateway/                         # API Gateway Service
│   ├── 📄 Ocelot.json                  # Gateway routing rules
│   ├── 📄 Program.cs                   # Entry point for Gateway
│   └── 📂 Midleware/                   # Gateway-specific logic
│       └── 📄 AttachedSignedHeader.cs  # Header signing middleware
├── 📂 Authentication.Application/      # Auth Business Logic
│   ├── 📂 Interfaces/                  # IUsers, IToken definitions
│   ├── 📂 Dtos/                        # User-related Data Transfer Objects
│   └── 📂 Service/                     # Mapping and Application services
├── 📂 Authentication.Domain/           # Auth Core Entities
│   └── 📂 Entities/                    # Appuser entity definition
├── 📂 Authentication.Infrastructure/   # Auth Data Access
│   ├── 📂 Repositories/                # UserRepository and TokenService
│   ├── 📂 Data/                        # AppDbContext and Factory
│   └── 📂 Migrations/                  # EF Core database snapshots
├── 📂 Authentication.Presentation/     # Auth API Endpoints
│   └── 📂 Controllers/                 # AuthController implementation
├── 📂 Product.Catalogue.Application/   # Product Business Logic
│   ├── 📂 Interfaces/                  # IProduct, IBrand, ICategory
│   ├── 📂 Services/                    # Service registration logic
│   └── 📂 Dtos/                        # Product, Category, and Brand DTOs
├── 📂 Product.Catalogue.Domain/        # Product Core Entities
│   └── 📂 Entities/                    # BaseEntity, Product, Category, Brand
├── 📂 Products.Catalogue.Infrastructure/ # Product Data Access
│   ├── 📂 Repositories/                # Implementation of product interfaces
│   ├── 📂 Data/                        # ProductDbContext
│   └── 📂 Migrations/                  # Product catalogue DB schema
├── 📂 Products.Catalogue.Presentation/  # Product API Endpoints
│   └── 📂 Controllers/                 # Product, Brand, and Category Controllers
└── 📂 ClassLibrary1/                   # Shared Cross-Cutting Library
    ├── 📂 DependencyInjection/         # Shared Auth and Service registration
    ├── 📂 Middleware/                  # GlobalExceptions and Gateway listener
    └── 📂 Resonse/                     # Standardized ApiResponse model
```

---

## 🚀 Getting Started

### Prerequisites
- **.NET SDK:** Ensure you have the latest .NET SDK installed on your machine.
- **IDE:** Visual Studio 2022 or VS Code with C# Dev Kit is recommended.
- **SQL Server:** Required for Entity Framework Core migrations and data persistence.

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/Sehar-1207/EcommereceStore-d25a5f2.git
   cd EcommereceStore-d25a5f2
   ```

2. **Restore Dependencies**
   Run this command from the root directory to restore all NuGet packages for the solution:
   ```bash
   dotnet restore
   ```

3. **Database Setup**
   The project uses EF Core Migrations. Update the connection strings in `appsettings.json` within the Presentation projects, then run:
   ```bash
   # For Authentication Service
   cd Authentication.Presentation
   dotnet ef database update

   # For Product Catalogue Service
   cd ../Products.Catalogue.Presentation
   dotnet ef database update
   ```

4. **Run the Services**
   You will need to run multiple projects simultaneously. You can use the Visual Studio "Multiple Startup Projects" feature or run them via CLI:
   ```bash
   # Terminal 1: Gateway
   dotnet run --project Gateway/Gateway.csproj

   # Terminal 2: Auth Service
   dotnet run --project Authentication.Presentation/Authentication.Presentation.csproj

   # Terminal 3: Product Service
   dotnet run --project Products.Catalogue.Presentation/Products.Catalogue.Presentation.csproj
   ```

---

## 🔧 Usage

### API Endpoints

Once the services are running, all requests should be directed to the **Gateway** (typically `localhost:5000` or as defined in `launchSettings.json`).

#### Authentication
- `POST /auth/login`: Authenticate a user and receive a JWT.
- **Payload:** Uses `LoginDto`.

#### Product Management
- `GET /products`: Retrieve all products.
- `GET /categories`: List all available categories.
- `GET /brands`: List all product brands.
- `POST /products`: Create a new product (Requires Authorized Header).

### Inter-Service Communication
The system utilizes a shared response model. Every API call returns an `ApiResponse` object:
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

---

## 🤝 Contributing

We welcome contributions to improve OmniCommerce! Your input helps make this project better for everyone.

### How to Contribute

1. **Fork the repository** - Click the 'Fork' button at the top right of this page.
2. **Create a feature branch** 
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Make your changes** - Improve code, documentation, or features.
4. **Test thoroughly** - Ensure all functionality works as expected.
   ```bash
   dotnet test
   ```
5. **Commit your changes** - Write clear, descriptive commit messages.
   ```bash
   git commit -m 'Add: Amazing new feature that does X'
   ```
6. **Push to your branch**
   ```bash
   git push origin feature/amazing-feature
   ```
7. **Open a Pull Request** - Submit your changes for review.

### Development Guidelines

- ✅ **Follow Clean Architecture:** Ensure logic is placed in the correct layer (Domain, Application, etc.).
- 📝 **Documentation:** Update the relevant README or add XML comments for complex logic.
- 🧪 **Migrations:** If you change an entity in the `.Domain` layer, ensure you generate a new migration in the `.Infrastructure` layer.
- 🎯 **Focus:** Keep commits atomic and focused on a single responsibility.

### Ideas for Contributions

- 🐛 **Bug Fixes:** Address any issues in the Gateway routing logic.
- ✨ **New Features:** Implement a shopping cart or order management microservice.
- 📖 **Documentation:** Improve API documentation using Swagger/OpenAPI.
- ⚡ **Performance:** Optimize the Repository queries to reduce database load.

---

<p align="center">Made with ❤️ by the Sehar Ajmal</p>
