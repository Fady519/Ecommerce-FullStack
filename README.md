<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0ea5e9,50:3b82f6,100:6366f1&height=220&section=header&text=LuxeStore%20E-Commerce&fontSize=48&fontColor=ffffff&fontAlignY=38&desc=Enterprise%20Full-Stack%20Platform%20%7C%20ASP.NET%20Core%20Web%20API%20%2B%20React%2019&descSize=16&descAlignY=58&descColor=e0f2fe" width="100%"/>

</div>

<div align="center">

[![Live Demo](https://img.shields.io/badge/🌐%20Live%20Storefront-Visit%20Now-0284C7?style=for-the-badge&logo=vercel&logoColor=white)](https://ecommerce-full-stack-nine.vercel.app)
[![Swagger API](https://img.shields.io/badge/⚡%20Swagger%20API-Explore%20Endpoints-2563EB?style=for-the-badge&logo=swagger&logoColor=white)](https://ecommercesolution.runasp.net/swagger/index.html)
[![GitHub Repo](https://img.shields.io/badge/GitHub-Source%20Code-181717?style=for-the-badge&logo=github)](https://github.com/Fady519/Ecommerce-FullStack)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)
[![Portfolio](https://img.shields.io/badge/Portfolio-Visit%20Now-6366F1?style=for-the-badge)](https://fady519.github.io/MyPortfolio2/)

</div>

---

<div align="center">

## 🛍️ An enterprise full-stack E-Commerce platform — not just CRUD.
**Clean Onion Architecture · ASP.NET Core 10 Web API · React 19 · Redis Caching · Stripe Payments · Role-Based Access Control · Admin Intelligence Portal**  
Built to handle high-throughput modern retail operations with enterprise-level code quality and seamless user experience.

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
| 🏗️ **Clean Onion Architecture** | Strict separation of concerns across Domain, Persistence, Services, and Presentation layers |
| 🔐 **Role-Based Access Control (RBAC)** | ASP.NET Identity with JWT Bearer tokens and claims-based authorization for `Admin` and `Customer` |
| ⚡ **Redis Basket & Output Caching** | Distributed Redis caching for high-speed cart persistence and tag-evicted output caching |
| 📊 **Admin Intelligence Dashboard** | Real-time aggregation of store revenue, sales volume, order lifecycle tracking, and low-stock alerts |
| 🏷️ **Dynamic Multi-Filter Catalog** | Paginated catalog with multi-category filtering, brand selection, price sorting, and live search |
| 🛒 **Isolated User Basket & Wishlist** | Strict user-token key isolation preventing cross-session cart and favorites leakage |
| 💳 **Secure Payment Processing** | Dual-mode checkout supporting live **Stripe** card payments and Cash on Delivery |
| 📦 **Order Lifecycle & Address Management** | Atomic order creation, delivery tier selection, shipping address capture, and history tracking |
| 🌓 **Adaptive Dark / Light Theme** | Luxury modern UI theme engine with zero-flicker persistent user preferences |
| 🎭 **Automated Testing Suite** | Comprehensive Vitest unit/component suite + Playwright automated end-to-end testing |

---

## 🏗️ Architecture & System Design

The backend is built following strict **Clean / Onion Architecture** principles with repository patterns and specifications:

```
┌────────────────────────────────────────────────────────┐
│               Presentation Layer                       │
│    ASP.NET Core 10 Web API · REST Controllers          │
│    JWT Authentication Middleware · RFC 9457 Errors     │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                Services Layer                          │
│    Business Services · AutoMapper · Specifications     │
│    Image Upload Service · Dashboard Analytics          │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│              Persistence Layer                         │
│    StoreDbContext · EF Core Migrations · SQL Server    │
│    JSON Data Seeders · Unit of Work · Redis Store      │
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
| **Administrator** | Full access to **Admin Console** (`/admin`): Add/edit/delete products, manage brands & categories, quick stock adjustments (`+`/`-`), view order lifecycle & sales analytics |
| **Customer** | Full shopping experience: Add to cart, manage wishlist, complete checkout via Stripe or Cash on Delivery, and track personal orders (`/orders`) |
| **Visitor / Guest** | Browse catalog, filter categories, search products, read journal, toggle light/dark theme (Prompted to login for cart & checkout) |

> ⚡ **Demo Experience:** The login page includes **1-Click Demo Buttons** (`⚡ Login as Admin` and `⚡ Login as Customer`) allowing recruiters and visitors to test all roles instantly without typing credentials!

---

## 📊 Admin Intelligence & Revenue Analytics

The integrated **Admin Console** surfaces real-time store metrics directly from the database:

```
✦ Total Revenue ($) & Total Orders Volume
✦ Low-Stock Alerts & Real-time Inventory Warnings
✦ One-Click Quick Stock Increments / Decrements
✦ Catalog Item Management (Image Upload, Pricing, Categories, Brands)
✦ Customer Orders Status Management (Pending, Payment Received, Shipped)
```

> Replaces static management with live, reactive administration.

---

## 🛠️ Tech Stack

<div align="center">

### Backend & Database
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core Web API](https://img.shields.io/badge/ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![JWT](https://img.shields.io/badge/JWT%20Bearer-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe%20Payments-635BFF?style=for-the-badge&logo=stripe&logoColor=white)

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
![Vitest](https://img.shields.io/badge/Vitest-6E9F18?style=for-the-badge&logo=vitest&logoColor=white)
![Playwright](https://img.shields.io/badge/Playwright-2EAD33?style=for-the-badge&logo=playwright&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

</div>

---

## 📁 Repository Structure

```
Project_5_Ecommerce/
│
├── E-commerce.Domain/               # Core business entities, contracts & repository interfaces
├── E-commerce.Persistence/          # DbContext, migrations, seeders, repository implementations
├── E-commerce.Services/             # Application logic, DTO mapping, specifications, image service
├── E-commerce.ServicesAbstraction/  # Service contracts & interfaces
├── E-commerce.Presentation/         # REST API Controllers (Catalog, Admin, Orders, Basket, Auth)
├── E-commerce.Shared/               # DTOs, query parameters, unified API responses
├── E-Commerce.Web/                  # API host Composition Root, DI registration, Swagger & Middleware
│
├── ecommerce-web/                   # Frontend SPA (React 19 + Vite)
│   ├── src/
│   │   ├── components/              # Headers, modals, sliders, UI controls, Context providers
│   │   ├── page/                    # Home, Catalog, Product Details, Cart, Admin, Checkout, Orders
│   │   ├── services/                # Strongly-typed API client (universal fetch wrapper)
│   │   └── test/                    # Unit and component tests (Vitest)
│   └── e2e/                         # Playwright automated End-to-End user journeys
│
├── docs/                            # Architectural documentation, roadmap & live screenshots
└── scripts/                         # Database population, SQL seeding & helper scripts
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18+) & npm](https://nodejs.org/)
- SQL Server (LocalDB, SSMS, or MonsterASP)

### 1. Backend Setup
```bash
# 1. Clone the repository
git clone https://github.com/Fady519/Ecommerce-FullStack.git
cd Ecommerce-FullStack

# 2. Restore dependencies & run backend API
dotnet restore
dotnet run --project E-Commerce.Web

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
cd ecommerce-web

# Run Vitest unit & component tests
npm run test

# Run Playwright E2E scenarios (Storefront, Cart, Checkout, Admin flow)
npm run test:e2e

# Run all test suites combined
npm run test:all
```

---

<div align="center">

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)
[![GitHub](https://img.shields.io/badge/GitHub-Fady519-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Fady519)
[![Portfolio](https://img.shields.io/badge/Portfolio-Live%20Now-6366F1?style=for-the-badge)](https://fady519.github.io/MyPortfolio2/)

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0ea5e9,50:3b82f6,100:6366f1&height=100&section=footer" width="100%"/>

**⭐ If this project impressed you, drop a star on GitHub — it means a lot!**

</div>
