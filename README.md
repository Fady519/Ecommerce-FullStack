# 🛒 E-Commerce RESTful API

A production-grade **E-Commerce REST API** built with **ASP.NET Core Web API** following **Onion Architecture**. Features JWT authentication, Redis caching, Stripe payment integration, and clean separation of concerns across Domain, Application, and Persistence layers.

🌐 **Live Demo (Swagger):** [ecommercesolution.runasp.net/swagger/index.html](http://ecommercesolution.runasp.net/swagger/index.html)

---

## 📸 Screenshots

> _Add Swagger UI screenshots here_

---

## ✨ Features

- 🏗️ **Onion Architecture** — Clean separation between Domain, Application, and Persistence layers
- 🔐 **JWT Authentication** — Secure Bearer Token auth with ASP.NET Identity and RBAC
- ⚡ **Redis Caching** — Distributed caching for high-frequency product catalog queries, reducing redundant DB round-trips
- 💳 **Stripe Integration** — End-to-end payment processing
- 📦 **Generic Repository + Specification Pattern** — ~40% less code duplication across all data-access modules
- 🛡️ **Custom Middleware** — Centralized exception handling and standardized API response formatting
- 🔄 **AutoMapper** — Clean object mapping across layers
- 📄 **Swagger UI** — Fully documented and testable API endpoints
- ⚙️ **Async/Await** — Throughout the entire request pipeline for maximum concurrent throughput

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Caching | Redis |
| Auth | ASP.NET Identity + JWT |
| Payments | Stripe API |
| Mapping | AutoMapper |
| Docs | Swagger / OpenAPI |
| Architecture | Onion Architecture + Generic Repository + Specification Pattern |

---

## 🏗️ Architecture Overview

```
├── Domain  (Core)
│   ├── Entities
│   ├── Interfaces
│   └── Specifications
│
├── Application  (Business Logic)
│   ├── Services
│   ├── DTOs
│   └── AutoMapper Profiles
│
├── Persistence  (Infrastructure)
│   ├── EF Core DbContext
│   ├── Generic Repository
│   └── Redis Cache Service
│
└── API  (Presentation)
    ├── Controllers
    ├── Middleware
    └── JWT + Identity Config
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)
- Redis Server
- Stripe Account (for payment testing)
- Visual Studio 2022 or VS Code

### Installation

```bash
# 1. Clone the repo
git clone https://github.com/Fady519/E_CommerceSolution.git
cd E_CommerceSolution

# 2. Update appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server Connection String",
  "Redis": "localhost:6379"
},
"JwtSettings": {
  "SecretKey": "Your Secret Key",
  "Issuer": "YourApp",
  "Audience": "YourApp"
},
"StripeSettings": {
  "PublishableKey": "Your Stripe Publishable Key",
  "SecretKey": "Your Stripe Secret Key"
}

# 3. Apply migrations
dotnet ef database update

# 4. Run the app
dotnet run
```

### 📄 API Documentation

Once running, navigate to:
```
https://localhost:{port}/swagger/index.html
```

---

## 🔑 Authentication

All protected endpoints require a **JWT Bearer Token**.

```http
Authorization: Bearer {your_token}
```

Obtain a token via the `/api/account/login` endpoint.

---

## 💳 Stripe Test Payments

Use Stripe test cards to simulate payments:

| Card Number | Scenario |
|---|---|
| `4242 4242 4242 4242` | Success |
| `4000 0000 0000 9995` | Declined |

---

## 📬 Contact

**Fady Kaiser** — [LinkedIn](https://www.linkedin.com/in/fady-kaiser/) · [GitHub](https://github.com/Fady519)
