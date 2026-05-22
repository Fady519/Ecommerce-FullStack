<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:1E3A5F,50:2563EB,100:1D4ED8&height=220&section=header&text=E-Commerce%20RESTful%20API&fontSize=50&fontColor=ffffff&fontAlignY=38&desc=ASP.NET%20Core%20%7C%20Onion%20Architecture%20%7C%20Redis%20%7C%20Stripe%20%7C%20JWT&descSize=16&descAlignY=58&descColor=ffffff" width="100%"/>

</div>

<div align="center">

[![Swagger UI](https://img.shields.io/badge/📄%20Swagger%20UI-Explore%20API-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](http://ecommercesolution.runasp.net/swagger/index.html)
[![GitHub](https://img.shields.io/badge/GitHub-Source%20Code-181717?style=for-the-badge&logo=github)](https://github.com/Fady519/E_CommerceSolution)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)

</div>

---

<div align="center">

## ⚙️ A production-grade E-Commerce API — built the right way.

Onion Architecture. Generic Repository. Redis Caching. Stripe Payments. JWT Auth.  
Every layer clean. Every endpoint secured. Every response standardized.

</div>

---

## 🖥️ API Explorer

<div align="center">

> 📄 **[Explore via Swagger UI](http://ecommercesolution.runasp.net/swagger/index.html)**

> _Add a Swagger UI screenshot here_

</div>

---

## ✨ Core Capabilities

| Capability | Detail |
|---|---|
| 🏗️ **Onion Architecture** | Domain → Application → Persistence — zero cross-layer leakage |
| 🗃️ **Generic Repository + Specification Pattern** | ~40% less code duplication across all data-access modules |
| ⚡ **Redis Distributed Caching** | Eliminates redundant DB round-trips on high-frequency catalog queries |
| 🔐 **JWT Bearer Auth** | Stateless authentication with ASP.NET Identity and claims-based RBAC |
| 💳 **Stripe Integration** | Full payment lifecycle — intent creation, confirmation, webhook handling |
| 🛡️ **Custom Middleware** | Centralized exception handling + standardized API response envelope |
| 📄 **Swagger / OpenAPI** | Fully documented and interactively testable endpoints |
| 🔄 **Async Pipeline** | `async/await` throughout — maximum throughput, zero thread blocking |

---

## 🏗️ Architecture

```
┌──────────────────────────────────────────────┐
│                   API Layer                  │
│   Controllers · Middleware · Swagger         │
│   JWT Config · Exception Handler             │
└─────────────────────┬────────────────────────┘
                      │
┌─────────────────────▼────────────────────────┐
│              Application Layer               │
│   Services · DTOs · AutoMapper Profiles      │
│   Stripe Service · Interfaces                │
└─────────────────────┬────────────────────────┘
                      │
┌─────────────────────▼────────────────────────┐
│              Persistence Layer               │
│   EF Core · Generic Repository               │
│   Specification Pattern · Redis Cache        │
└─────────────────────┬────────────────────────┘
                      │
┌─────────────────────▼────────────────────────┐
│                Domain Layer                  │
│   Entities · Interfaces · Business Rules     │
└──────────────────────────────────────────────┘
```

---

## 🛠️ Tech Stack

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-635BFF?style=for-the-badge&logo=stripe&logoColor=white)
![AutoMapper](https://img.shields.io/badge/AutoMapper-BE0000?style=for-the-badge)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

**Patterns & Architecture**

![Onion Architecture](https://img.shields.io/badge/Onion%20Architecture-2563EB?style=for-the-badge)
![Generic Repository](https://img.shields.io/badge/Generic%20Repository-1D4ED8?style=for-the-badge)
![Specification Pattern](https://img.shields.io/badge/Specification%20Pattern-1E40AF?style=for-the-badge)
![SOLID](https://img.shields.io/badge/SOLID%20Principles-1E3A8A?style=for-the-badge)

</div>

---

## 🔌 API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/account/register` | ❌ | Register new user |
| `POST` | `/api/account/login` | ❌ | Login & get JWT |
| `GET` | `/api/products` | ❌ | Get all products (cached) |
| `GET` | `/api/products/{id}` | ❌ | Get product details |
| `GET` | `/api/basket` | ✅ | Get user basket |
| `POST` | `/api/basket` | ✅ | Add item to basket |
| `POST` | `/api/orders` | ✅ | Place an order |
| `POST` | `/api/payments` | ✅ | Create Stripe payment intent |

> Full interactive docs available on **[Swagger UI](http://ecommercesolution.runasp.net/swagger/index.html)**

---

## 💳 Stripe Payment Flow

```
1. Client sends POST /api/payments
         ↓
2. API creates PaymentIntent via Stripe SDK
         ↓
3. Client confirms payment on frontend
         ↓
4. Stripe fires webhook → API updates order status
         ↓
5. Order confirmed ✅
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server
- Redis Server (or [Upstash](https://upstash.com) free tier)
- Stripe Account

### Installation

```bash
# 1. Clone the repo
git clone https://github.com/Fady519/E_CommerceSolution.git
cd E_CommerceSolution

# 2. Configure appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Your SQL Server Connection String",
  "Redis": "localhost:6379"
},
"JWT": {
  "SecretKey": "your-secret-key",
  "Issuer":    "YourApp",
  "Audience":  "YourApp"
},
"StripeSettings": {
  "PublishableKey": "pk_test_...",
  "SecretKey":      "sk_test_..."
}

# 3. Apply migrations
dotnet ef database update

# 4. Run
dotnet run
```

Navigate to `https://localhost:{port}/swagger` to explore all endpoints.

---

<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:1E3A5F,50:2563EB,100:1D4ED8&height=100&section=footer" width="100%"/>

**⭐ Found this useful? A star keeps the momentum going.**

</div>
