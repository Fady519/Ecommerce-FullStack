<div align="center">

<img src="https://capsule-render.vercel.app/api?type=waving&color=0:0ea5e9,50:3b82f6,100:6366f1&height=220&section=header&text=LuxeStore%20E-Commerce&fontSize=48&fontColor=ffffff&fontAlignY=38&desc=Full-Stack%20E-Commerce%20Platform%20%7C%20ASP.NET%20Core%20Web%20API%20%2B%20React%2019&descSize=16&descAlignY=58&descColor=e0f2fe" width="100%"/>

</div>

<div align="center">

[![GitHub Repo](https://img.shields.io/badge/GitHub-Source%20Code-181717?style=for-the-badge&logo=github)](https://github.com/Fady519/Ecommerce-FullStack)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Fady%20Kaiser-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/fady-kaiser/)
[![Portfolio](https://img.shields.io/badge/Portfolio-Visit%20Now-6366F1?style=for-the-badge)](https://fady519.github.io/MyPortfolio2/)
[![Framework](https://img.shields.io/badge/.NET%209-ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Frontend](https://img.shields.io/badge/React%2019-Vite-61DAFB?style=for-the-badge&logo=react&logoColor=black)](https://react.dev/)

</div>

---

<div align="center">

## 🛍️ Enterprise Full-Stack E-Commerce Platform — Clean Architecture & High Performance
Onion / Clean Architecture · ASP.NET Core Identity & JWT · Redis & Output Cache · Dynamic Catalog · Admin Dashboard  
Built for high-throughput, enterprise standards, and seamless modern user experience.

</div>

---

## ✨ Key Features
| Feature | Description |
|---|---|
| 🏗️ **Clean / Onion Architecture** | Domain, Persistence, Services, Presentation, and Shared contracts separation |
| 🔐 **Authentication & RBAC** | ASP.NET Identity with JWT Bearer tokens and Role-Based Authorization (`Admin`, `Customer`) |
| ⚡ **High-Speed Caching & OutputCache** | Redis basket caching and response output caching with tag-based invalidation |
| 📊 **Admin Intelligence Dashboard** | Real-time SQL aggregation for total sales, unit counts, revenue charts, and low-stock alerts |
| 🏷️ **Dynamic Catalog & Search** | Paginated products with multi-filter brand/type queries, price sorting, and live keyword search |
| 🛒 **Per-User Isolated Cart & Wishlist** | Strict user-key storage isolation preventing data leakage between guest and authenticated sessions |
| 💳 **Checkout & Order Processing** | Atomic order creation with shipping address capture and multiple delivery method tiers |
| 🌓 **Dark & Light Mode Engine** | Seamless luxury theme switcher with persistent preference stored in `localStorage` |
| 🎭 **Automated Quality & E2E Testing** | Vitest unit/component suite + Playwright automated end-to-end user journey tests |

---

## 🏗️ Architecture & System Design
The backend is structured according to strict **Onion / Clean Architecture** principles to eliminate tight coupling:

```
┌────────────────────────────────────────────────────────┐
│               Presentation Layer                       │
│    ASP.NET Core Web API · REST Controllers             │
│    JWT Middleware · RFC 9457 Error Handling            │
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
│    StoreDbContext · StoreIdentityDbContext            │
│    EF Core · Migrations · JSON Seeding · Redis         │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                Domain Layer                            │
│    Entities (Product, Brand, Type, Order, Basket)      │
│    Value Objects · Repository Contracts                │
└────────────────────────────────────────────────────────┘
```

---

## 👥 User Roles & Access Control
| Role | Permissions |
|---|---|
| **Admin** | Full access to Admin Portal (`/admin`): Add/Edit/Delete products, manage brands/types, view orders, inventory alerts & revenue charts |
| **Customer** | Browse catalog, search, filter, manage cart & wishlist, proceed through checkout, view personal order history |
| **Visitor / Guest** | Browse all products, categories, search, read blog journal, switch themes (Prompted to login before cart/checkout) |

> Enforced securely on both backend (`[Authorize(Roles = "Admin")]` / Policies) and frontend (`AdminRoute` & `ProtectedRoute`).

---

## 🛠️ Tech Stack

<div align="center">

### Backend
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 9](https://img.shields.io/badge/.NET%209-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core Web API](https://img.shields.io/badge/ASP.NET%20Core%20Web%20API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

### Frontend
![React 19](https://img.shields.io/badge/React%2019-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![React Router 7](https://img.shields.io/badge/React%20Router-CA4245?style=for-the-badge&logo=reactrouter&logoColor=white)
![Framer Motion](https://img.shields.io/badge/Framer%20Motion-0055FF?style=for-the-badge&logo=framer&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white)

### Testing & Tooling
![Vitest](https://img.shields.io/badge/Vitest-6E9F18?style=for-the-badge&logo=vitest&logoColor=white)
![Playwright](https://img.shields.io/badge/Playwright-2EAD33?style=for-the-badge&logo=playwright&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

</div>

---

## 📁 Repository Structure
```
Project_5_Ecommerce/
│
├── E-commerce.Domain/               # Core entities, contracts & domain models
├── E-commerce.Persistence/          # EF Core DbContexts, migrations, seeding
├── E-commerce.Services/             # Application services, specifications, mapping
├── E-commerce.ServicesAbstraction/  # Service interfaces
├── E-commerce.Presentation/         # API Controllers (Catalog, Admin, Orders, Auth)
├── E-commerce.Shared/               # DTOs, parameters, request/response models
├── E-Commerce.Web/                  # Startup host, DI configuration, middleware, static files
│
└── ecommerce-web/                   # Frontend SPA (React 19 + Vite)
    ├── src/
    │   ├── components/              # Headers, modals, error boundaries, UI cards
    │   ├── page/                    # Home, Catalog, Product Details, Cart, Admin, Orders
    │   ├── services/                # Strongly-typed API client
    │   └── test/                    # Vitest unit & component test suites
    └── e2e/                         # Playwright End-to-End automated test specs
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18+) & npm](https://nodejs.org/)
- SQL Server (LocalDB or full instance)

### 1. Clone & Backend Setup
```bash
# Clone the repository
git clone https://github.com/Fady519/Ecommerce-FullStack.git
cd Ecommerce-FullStack

# Restore dependencies & run backend API
dotnet restore
dotnet run --project E-Commerce.Web
# API will start at http://localhost:5163 with Swagger at /swagger
```

### 2. Frontend Setup
```bash
# Navigate to the frontend directory
cd ecommerce-web

# Install packages
npm install

# Start Vite dev server
npm run dev
# Frontend will be live at http://localhost:5173
```

---

## 🔑 Demo Credentials

| Role | Email | Password |
|---|---|---|
| **Administrator** | `fadykaiser2004@gmail.com` | `Fady@Store2026` |
| **Demo Customer** | `fady@demo.store` | `Pa$$w0rd123` |

---

## 🧪 Testing

```bash
cd ecommerce-web

# Run Vitest unit & component tests
npm run test

# Run Playwright E2E scenarios (browsing, purchase, admin)
npm run test:e2e

# Run all test suites
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
