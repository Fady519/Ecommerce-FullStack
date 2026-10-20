<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0ea5e9,50:3b82f6,100:6366f1&height=220&section=header&text=LuxeStore%20E-Commerce&fontSize=48&fontColor=ffffff&fontAlignY=38&desc=Enterprise%20Full-Stack%20Platform%20%7C%20ASP.NET%20Core%20Web%20API%20%2B%20React%2019&descSize=16&descAlignY=58&descColor=e0f2fe" width="100%"/>

</div>

<div align="center">

[![Live Demo](https://img.shields.io/badge/🌐%20Live%20Storefront-Visit%20Now-0284C7?style=for-the-badge&logo=vercel&logoColor=white)](https://ecommerce-full-stack-nine.vercel.app)
[![Swagger API](https://img.shields.io/badge/⚡%20Swagger%20API-Explore%20Endpoints-2563EB?style=for-the-badge&logo=swagger&logoColor=white)](https://ecommercesolution.runasp.net/swagger/index.html)
[![GitHub Repo](https://img.shields.io/badge/GitHub-Source%20Code-181717?style=for-the-badge&logo=github)](https://github.com/Fady519/Ecommerce-FullStack)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)
[![Portfolio](https://img.shields.io/badge/Portfolio-Visit%20Now-6366F1?style=for-the-badge)](https://fady519.github.io/Portfolio-Fady/)

</div>

---

<div align="center">

## 🛍️ A full-stack E-Commerce platform — not just CRUD.
**Onion Architecture · ASP.NET Core 10 Web API · React 19 · SQL Server · Output Caching · Stripe (test mode) + Cash on Delivery · Role-Based Access Control · Admin Console**

</div>

---

## 🖥️ Live Preview

<div align="center">

> 🔗 **Storefront:** [ecommerce-full-stack-nine.vercel.app](https://ecommerce-full-stack-nine.vercel.app)  
> 🔗 **Swagger API Docs:** [ecommercesolution.runasp.net/swagger](https://ecommercesolution.runasp.net/swagger/index.html)

</div>

### 1. Modern Storefront & Dynamic Catalog
![LuxeStore Home](docs/screenshots/01_home.png)

### 2. Multi-Filter Catalog, Live Search & Sorting
![LuxeStore Catalog](docs/screenshots/02_catalog.png)

### 3. Integrated Admin Console & Inventory Intelligence
![Admin Dashboard](docs/screenshots/03_admin_dashboard.png)

### 4. Interactive Swagger OpenAPI Specification (Live on MonsterASP)
![Swagger API](docs/screenshots/04_swagger_api.png)

---

## ✨ Features

| Feature | Description |
|---|---|
| 🏗️ **Onion Architecture** | Separation of concerns across Domain, Persistence, Services, Services Abstraction and Presentation layers |
| 🔐 **Role-Based Access Control (RBAC)** | ASP.NET Core Identity with JWT Bearer tokens and role-based policies for `Admin` and `Customer`, account lockout and login rate limiting |
| 🗄️ **SQL Server Baskets & Output Caching** | Baskets stored in SQL Server (Baskets + BasketItems) with server-side pricing and owner-only access; catalog responses cached with ASP.NET Core Output Caching (5 min, tag-evicted on admin changes) |
| 📊 **Admin Dashboard** | Total sales and order count (non-cancelled orders), top 5 best sellers, low-stock alerts (stock ≤ 10) and order status management |
| 🏷️ **Filterable Catalog** | Paginated catalog (max 10 per page) with category and brand filters, name/price sorting and name search |
| 🛒 **Isolated User Basket & Wishlist** | Owner-only baskets on the server (other users get 404) and per-account cart/favorites storage in the browser |
| 💳 **Payments** | Cash on Delivery, or card via a server-side **Stripe** test-mode PaymentIntent with a signature-verified webhook (client-side card confirmation is not implemented yet) |
| 📦 **Orders & Address Management** | Order creation with stock checks (409 when stock is insufficient), delivery method selection, shipping address snapshot and order history |
| 🌓 **Dark / Light Theme** | Persisted theme preference (dark by default) |
| 🎭 **Automated Tests** | xUnit integration tests (WebApplicationFactory + SQL Server), Vitest component tests and Playwright end-to-end scenarios |

---

## 🏗️ Architecture & System Design

The backend follows **Onion Architecture** with the Repository, Unit of Work and Specification patterns:

```
┌────────────────────────────────────────────────────────┐
│               Presentation Layer                       │
│    ASP.NET Core 10 Web API · REST Controllers          │
│    JWT Authentication Middleware · RFC 9457 Errors     │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                Services Layer                          │
│    Business Services · Mapping · Specifications        │
│    Image Upload Service · Dashboard Analytics          │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│              Persistence Layer                         │
│    StoreDbContext · EF Core Migrations · SQL Server    │
│    JSON Data Seeders · Unit of Work · Basket Repository│
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                Domain Layer                            │
│    Entities (Product, Brand, Type, Order, Basket)      │
│    Value Objects · Repository Contracts · Enums        │
└────────────────────────────────────────────────────────┘
```

---

## 👥 User Roles & Access Control

| Role | Permissions & Access Scope |
|---|---|
| **Administrator** | Full access to **Admin Console** (`/admin`): Add/edit/delete products with image upload, add/delete brands & categories, update order status, view sales totals, top sellers & low-stock alerts |
| **Customer** | Full shopping experience: Add to cart, manage wishlist, check out with Cash on Delivery (card checkout creates a Stripe test-mode PaymentIntent), and track personal orders (`/orders`) |
| **Visitor / Guest** | Browse catalog, filter categories, search products, read journal, toggle light/dark theme (Prompted to login for cart & checkout) |

> ⚡ **Demo Experience:** The login page includes **1-Click Demo Buttons** (`⚡ Login as Admin (1-Click)` and `👤 Demo Customer`) so visitors can try the roles without typing credentials.

---

## 📊 Admin Intelligence & Revenue Analytics

The integrated **Admin Console** reads its metrics from the database through the admin dashboard endpoint:

```
✦ Total Sales (EGP) & Total Orders (non-cancelled orders)
✦ Top 5 Best-Selling Products
✦ Low-Stock Alerts (stock ≤ 10)
✦ Catalog Management (Image Upload, Pricing, Categories, Brands)
✦ Order Status Management (Pending, Payment Received, Shipped, Delivered, Cancelled)
```

---

## 🛠️ Tech Stack

<div align="center">

### Backend & Database
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core Web API](https://img.shields.io/badge/ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT%20Bearer-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe%20(test%20mode)-635BFF?style=for-the-badge&logo=stripe&logoColor=white)

### Architecture & Patterns
![Onion Architecture](https://img.shields.io/badge/Onion%20Architecture-0284C7?style=for-the-badge)
![Repository Pattern](https://img.shields.io/badge/Repository%20Pattern-2563EB?style=for-the-badge)
![Unit of Work](https://img.shields.io/badge/Unit%20of%20Work-1D4ED8?style=for-the-badge)
![Specification Pattern](https://img.shields.io/badge/Specification%20Pattern-3B82F6?style=for-the-badge)
![SOLID](https://img.shields.io/badge/SOLID%20Principles-1E40AF?style=for-the-badge)
![Dependency Injection](https://img.shields.io/badge/Dependency%20Injection-0369A1?style=for-the-badge)

### Frontend & Styling
![React 19](https://img.shields.io/badge/React%2019-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![React Router 7](https://img.shields.io/badge/React%20Router-CA4245?style=for-the-badge&logo=reactrouter&logoColor=white)
![Framer Motion](https://img.shields.io/badge/Framer%20Motion-0055FF?style=for-the-badge&logo=framer&logoColor=white)
![Swiper](https://img.shields.io/badge/Swiper-6332F6?style=for-the-badge&logo=swiper&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

### Testing & Quality Assurance
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Vitest](https://img.shields.io/badge/Vitest-6E9F18?style=for-the-badge&logo=vitest&logoColor=white)
![Playwright](https://img.shields.io/badge/Playwright-2EAD33?style=for-the-badge&logo=playwright&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

</div>

---

## 📁 Repository Structure

```
Ecommerce-FullStack/
│
├── E-commerce.Domain/               # Core business entities, contracts & repository interfaces
├── E-commerce.Persistence/          # DbContext, migrations, seeders, repository implementations
├── E-commerce.Services/             # Application logic, DTO mapping, specifications, image service
├── E-commerce.ServicesAbstraction/  # Service contracts & interfaces
├── E-commerce.Presentation/         # REST API Controllers (Catalog, Admin, Orders, Basket, Auth)
├── E-commerce.Shared/               # DTOs, query parameters, unified API responses
├── E-Commerce.Web/                  # API host Composition Root, DI registration, Swagger & Middleware
├── E-commerce.IntegrationTests/     # xUnit integration tests (WebApplicationFactory)
│
├── ecommerce-web/                   # Frontend SPA (React 19 + Vite)
│   ├── src/
│   │   ├── components/              # Headers, modals, sliders, UI controls, Context providers
│   │   ├── page/                    # Home, Catalog, Product Details, Cart, Admin, Checkout, Orders
│   │   ├── services/                # Strongly-typed API client (universal fetch wrapper)
│   │   └── test/                    # Unit and component tests (Vitest)
│   └── e2e/                         # Playwright automated End-to-End user journeys
│
└── docs/                            # Screenshots
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18+) & npm](https://nodejs.org/)
- SQL Server (local default instance `Server=.`)

### 1. Backend Setup
```bash
# 1. Clone the repository
git clone https://github.com/Fady519/Ecommerce-FullStack.git
cd Ecommerce-FullStack

# 2. Restore dependencies and set the required secrets
dotnet restore
dotnet user-secrets set "JWTOptions:SecretKey" "<random value, 32+ chars>" --project E-Commerce.Web
dotnet user-secrets set "AdminUser:Password" "<admin password>" --project E-Commerce.Web
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..." --project E-Commerce.Web

# 3. Run the API (Development applies migrations + seed data on startup)
dotnet run --project E-Commerce.Web
# optional demo data: set "DemoData:Password", then: dotnet run --project E-Commerce.Web -- --seed-demo

# The API will be live at http://localhost:5163 with Swagger at /swagger
```

### 2. Frontend Setup
```bash
# 1. Navigate to frontend
cd ecommerce-web

# 2. Install dependencies
npm install

# 3. Launch Vite development server
npm run dev

# The storefront will be live at http://localhost:5173
```

---

## 🔑 Default Credentials & Demo Access

| Role | Email | Password | Access Highlights |
|---|---|---|---|
| **Administrator** | `fadykaiser2004@gmail.com` | `Fady@Store2026` | Full Admin Console, Analytics, Product Management |
| **Demo Customer** | `fady@demo.store` | `Pa$$w0rd123` | Cart, Wishlist, Checkout, Order Tracking |

> 💡 **Tip:** On the live website login screen, simply click the **1-Click Demo Buttons** to sign in instantly without typing anything!

---

## 🧪 Automated Testing

```bash
# Backend: xUnit integration tests (needs a local SQL Server, Server=.)
dotnet test

cd ecommerce-web

# Run Vitest unit & component tests
npm run test

# Run Playwright E2E scenarios (browse & search, login + add to cart, admin dashboard tabs) — needs the API and the Vite dev server running
npm run test:e2e

# Run all test suites combined
npm run test:all
```

---

<div align="center">

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)
[![GitHub](https://img.shields.io/badge/GitHub-Fady519-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Fady519)
[![Portfolio](https://img.shields.io/badge/Portfolio-Live%20Now-6366F1?style=for-the-badge)](https://fady519.github.io/Portfolio-Fady/)

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0ea5e9,50:3b82f6,100:6366f1&height=100&section=footer" width="100%"/>

**⭐ If this project impressed you, drop a star on GitHub — it means a lot!**

</div>
