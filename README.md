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
| ⚡ **Output Caching** | Catalog responses cached in memory (per query string) — no Redis server needed |
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

## 🗄️ Database

One SQL Server database (`ECommerceStore`) holds everything: users & roles (ASP.NET Identity), the catalog, baskets and orders.
Baskets used to live in Redis and users in a second database; now every relationship is a real foreign key.

```mermaid
erDiagram
    Users ||--o| Addresses : "saved address"
    Users ||--o{ UserRoles : has
    Roles ||--o{ UserRoles : "Admin / Customer"
    Users ||--o{ Orders : places
    Users |o--o{ Baskets : "owns (optional, guests have none)"

    ProductBrands ||--o{ Products : brand
    ProductTypes ||--o{ Products : type

    Baskets ||--o{ BasketItems : contains
    Products ||--o{ BasketItems : "in basket"
    DeliveryMethods |o--o{ Baskets : "chosen at checkout"

    Orders ||--|{ OrderItems : contains
    DeliveryMethods ||--o{ Orders : "ships with"

    Users {
        string Id PK
        string DisplayName
        string Email
        datetimeoffset CreatedAt
    }
    Addresses {
        int Id PK
        string UserId FK
        string FirstName
        string LastName
        string Street
        string City
        string Country
    }
    Products {
        int ID PK
        string Name
        decimal Price "CHECK >= 0"
        int Stock "CHECK >= 0"
        string PictureUrl
        int BrandId FK
        int TypeId FK
    }
    Baskets {
        string ID PK "created by the browser"
        string UserId FK "nullable"
        int DeliveryMethodId FK "nullable"
        string PaymentIntentId
        datetimeoffset UpdatedAt
    }
    BasketItems {
        int ID PK
        string BasketId FK
        int ProductId FK
        int Quantity "CHECK 1..100"
    }
    Orders {
        guid ID PK
        string UserId FK
        string UserEmail "snapshot"
        datetimeoffset OrderDate
        string Status "Pending, PaymentReceived, Shipped, Delivered..."
        string PaymentMethod "Card / CashOnDelivery"
        string Address_City "address copied into the order"
        decimal SubTotal
        int DeliveryMethodId FK
    }
    OrderItems {
        int ID PK
        guid OrderId FK
        int ProductId "snapshot, no FK"
        string ProductName "snapshot"
        decimal Price "price paid"
        int Quantity
    }
    DeliveryMethods {
        int ID PK
        string ShortName
        string DeliveryTime
        decimal Price
    }
```

**Design choices**
- **Prices are never stored in the basket** — they are read from `Products`, so a client can't send a cheaper price.
- **Orders keep snapshots** (product name, price paid, address, email): editing a product or an address later doesn't rewrite order history. That's why `OrderItems.ProductId` has no foreign key.
- **Deleting is safe**: removing a user or a delivery method that has orders is refused (`Restrict`); basket lines are removed with their basket (`Cascade`).
- **Readable data**: order status and payment method are stored as text (`"Delivered"`, not `3`), and Identity tables have short names (`Users`, `Roles`).
- **Seeding**: the catalog (13 products, 6 brands, 4 types, 4 delivery methods in EGP) is seeded on first start from JSON files embedded in the dll. `--seed-demo` adds 30 customers and ~3 months of orders (see below).

---

## 🛠️ Tech Stack

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-635BFF?style=for-the-badge&logo=stripe&logoColor=white)
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

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB / Express / Developer) — no Redis needed anymore
- A Stripe account in **test mode** (only for card payments)

### Installation

```bash
# 1. Clone the repo
git clone https://github.com/Fady519/E_CommerceSolution.git
cd E_CommerceSolution

# 2. Secrets (never in appsettings.json) — stored outside the repo with user-secrets
dotnet user-secrets set "JWTOptions:SecretKey" "<a long random string, 32+ characters>" --project E-Commerce.Web
dotnet user-secrets set "Stripe:SecretKey"     "sk_test_..."                          --project E-Commerce.Web
dotnet user-secrets set "AdminUser:Password"   "<password for the admin account>"     --project E-Commerce.Web
dotnet user-secrets set "DemoData:Password"    "<password for the demo logins>"       --project E-Commerce.Web

# 3. Run — in Development the database (ECommerceStore on Server=.) is created, migrated and seeded automatically
dotnet run --project E-Commerce.Web --launch-profile http

# 4. (optional) Demo data: 30 customers + ~3 months of orders
dotnet run --project E-Commerce.Web --launch-profile http -- --seed-demo
```

Swagger: `http://localhost:5163/swagger`. Demo logins: `customer@demo.store` and `admin@demo.store` (password = `DemoData:Password`).
The connection string lives in `E-Commerce.Web/appsettings.Development.json` (change `Server=.` if your SQL Server instance has another name).

---

<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:1E3A5F,50:2563EB,100:1D4ED8&height=100&section=footer" width="100%"/>

**⭐ Found this useful? A star keeps the momentum going.**

</div>
