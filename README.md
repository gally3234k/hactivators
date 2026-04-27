# Medical — Pharmacy Management System

> Full-stack pharmacy platform with role-based access, medicine inventory, and order management.
> TypeScript SPA frontend consuming an ASP.NET Core REST API backed by SQL Server.

![TypeScript](https://img.shields.io/badge/TypeScript-55.8%25-3178C6?style=flat-square)
![C#](https://img.shields.io/badge/C%23-40.6%25-512BD4?style=flat-square)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat-square)

---

## Contents

**Frontend (PharmacyClient)**
- [Overview](#frontend-overview)
- [Structure](#frontend-structure)
- [Pages & views](#pages--views)
- [API integration](#api-integration)

**Backend (PharmacyAPI)**
- [Overview](#backend-overview)
- [Structure](#backend-structure)
- [API endpoints](#api-endpoints)
- [Authentication & roles](#authentication--roles)
- [Data model](#data-model)

**[Getting started](#getting-started)**

---

## System architecture

```
┌────────────────────────────────────────────────────────────────────┐
│                          Medical.sln                               │
│                                                                    │
│  ┌──────────────────┐  fetch/JSON   ┌──────────────────────────┐   │
│  │  PharmacyClient  │ ────────────► │       PharmacyAPI        │   │
│  │  TypeScript SPA  │ ◄──────────── │   ASP.NET Core Web API   │   │
│  │  HTML · CSS      │               │   Controllers · Services │   │
│  └──────────────────┘               │   EF Core · Middleware   │   │
│                                     └──────────┬───────────────┘   │
│  ┌──────────────────┐                          │ EF Core / SQL     │
│  │   HashGen CLI    │ ── seed ──────────────►  ▼                   │
│  │  BCrypt utility  │               ┌──────────────────────────┐   │
│  └──────────────────┘               │       SQL Server         │   │
│                                     │  Users · Medicines       │   │
│                                     │  Orders · OrderItems     │   │
│                                     └──────────────────────────┘   │
└────────────────────────────────────────────────────────────────────┘
```

---

## Frontend overview

`PharmacyClient` is a lightweight vanilla TypeScript SPA — no framework, just compiled ES modules and plain CSS. Auth state is stored in `localStorage` and injected as a `Bearer` token on every request. The navbar and page controls conditionally render based on the decoded role claim from the JWT.

### Frontend structure

```
PharmacyClient/
├── index.html                     # App shell — loads compiled JS
├── src/
│   ├── main.ts                    # Entry point, client-side router
│   ├── auth.ts                    # Login, register, token helpers
│   ├── api.ts                     # Typed fetch wrapper, base URL config
│   ├── pages/
│   │   ├── login.ts               # Login / register page
│   │   ├── medicines.ts           # Medicine catalog + search
│   │   ├── cart.ts                # Cart, quantity management
│   │   ├── orders.ts              # Order history (customer view)
│   │   ├── manage-medicines.ts    # Admin / Pharmacist CRUD UI
│   │   └── manage-orders.ts       # Admin / Pharmacist order queue
│   └── components/
│       ├── navbar.ts              # Role-aware navigation bar
│       └── toast.ts               # Notification toasts
└── css/
    ├── main.css                   # Global reset, typography
    └── components.css             # Cards, tables, forms, buttons
```

### Pages & views

| Page                  | File                    | Visible to              | Purpose                         |
| --------------------- | ----------------------- | ----------------------- | ------------------------------- |
| `/login`              | `login.ts`              | Public                  | Login and register forms        |
| `/medicines`          | `medicines.ts`          | All                     | Browse catalog, search, add to cart |
| `/cart`               | `cart.ts`               | Customer                | Review cart, place order        |
| `/orders`             | `orders.ts`             | Customer                | Own order history and status    |
| `/manage/medicines`   | `manage-medicines.ts`   | Admin · Pharmacist      | Add, edit, delete medicines     |
| `/manage/orders`      | `manage-orders.ts`      | Admin · Pharmacist      | View all orders, update status  |

### API integration

All HTTP calls go through `api.ts`, which attaches the stored JWT and handles `401` redirects automatically.

```typescript
// src/api.ts
const BASE = 'https://localhost:5001/api';

async function apiFetch<T>(path: string, opts?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token');
  const res = await fetch(`${BASE}${path}`, {
    ...opts,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...opts?.headers,
    },
  });
  if (res.status === 401) { logout(); return; }
  return res.json();
}
```

---

## Backend overview

`PharmacyAPI` is an ASP.NET Core Web API following a clean three-layer pattern. Controllers are thin — they validate input and delegate to service classes. Services hold all business logic and call into Entity Framework Core. BCrypt handles password hashing at rest.

### Backend structure

```
PharmacyAPI/
├── PharmacyAPI.csproj
├── Program.cs                     # Service registration, middleware pipeline
├── appsettings.json               # Connection strings, JWT config
├── Controllers/
│   ├── AuthController.cs          # POST /login, POST /register
│   ├── MedicinesController.cs     # CRUD /api/medicines
│   └── OrdersController.cs        # CRUD /api/orders
├── Models/
│   ├── User.cs
│   ├── Medicine.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── DTOs/                          # Request / response shapes
│   ├── LoginRequest.cs
│   ├── MedicineDto.cs
│   └── OrderDto.cs
├── Services/
│   ├── AuthService.cs             # BCrypt verify, token issue
│   ├── MedicineService.cs         # Inventory business logic
│   └── OrderService.cs            # Stock check, order creation
└── Data/
    ├── PharmacyDbContext.cs        # EF Core DbContext
    └── Migrations/                 # Code-first migration history
```

### API endpoints

| Method     | Endpoint                    | Role                  | Description                      |
| ---------- | --------------------------- | --------------------- | -------------------------------- |
| `POST`     | `/api/auth/login`           | Public                | Authenticate, receive JWT        |
| `POST`     | `/api/auth/register`        | Public                | Create customer account          |
| `GET`      | `/api/medicines`            | All                   | List all medicines with stock    |
| `GET`      | `/api/medicines/{id}`       | All                   | Single medicine detail           |
| `POST`     | `/api/medicines`            | Admin · Pharmacist    | Add new medicine                 |
| `PUT`      | `/api/medicines/{id}`       | Admin · Pharmacist    | Update medicine or stock level   |
| `DELETE`   | `/api/medicines/{id}`       | Admin                 | Remove from catalog              |
| `POST`     | `/api/orders`               | Customer              | Place order, deduct stock        |
| `GET`      | `/api/orders`               | Admin · Pharmacist    | All orders with status           |
| `GET`      | `/api/orders/my`            | Customer              | Own order history                |
| `PUT`      | `/api/orders/{id}/status`   | Admin · Pharmacist    | Update order status              |

### Authentication & roles

Passwords are BCrypt-hashed at 10 rounds. The `HashGen` CLI project generates hashes for the database seed script.

```csharp
// HashGen.cs
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin@123"));
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Pharma@123"));
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("John@123"));
```

Default seeded accounts:

| Username      | Password      | Role         |
| ------------- | ------------- | ------------ |
| `admin`       | `Admin@123`   | Admin        |
| `pharmacist`  | `Pharma@123`  | Pharmacist   |
| `john`        | `John@123`    | Customer     |

> **Warning:** Change all default credentials before any deployment.

Role permission matrix:

| Role         | Medicines    | Orders          | Users      | Reports    |
| ------------ | ------------ | --------------- | ---------- | ---------- |
| Admin        | Full CRUD    | All orders      | Manage all | Full       |
| Pharmacist   | Full CRUD    | View + process  | None       | Limited    |
| Customer     | Browse only  | Own orders only | None       | None       |

### Data model

Four core entities. Stock is decremented atomically when an order is confirmed.

```
USERS ──────────────────── ORDERS ────────────── ORDER_ITEMS
  Id         PK              Id       PK            Id          PK
  Username                   UserId   FK ──►         OrderId     FK ──►
  PasswordHash                Status                 MedicineId  FK ──┐
  Role                        TotalAmount             Quantity        │
  Email                       CreatedAt               UnitPrice       │
                                                                      │
MEDICINES ◄───────────────────────────────────────────────────────────┘
  Id         PK
  Name
  Description
  Price
  StockQty
  Category
```

Relationships:
- `USERS` → `ORDERS` — one user places many orders (`1..*`)
- `ORDERS` → `ORDER_ITEMS` — one order contains one or more line items (`1..*`)
- `MEDICINES` → `ORDER_ITEMS` — one medicine referenced by many line items (`1..*`)

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2019 or later (local or Azure SQL)
- Node.js 18+ *(optional — only needed to compile TypeScript or use `npx serve`)*
- Visual Studio 2022 or the `dotnet` CLI

### Setup

**1. Clone**

```bash
git clone https://github.com/worksbyrohith/Medical.git
cd Medical
```

**2. Restore .NET dependencies**

```bash
dotnet restore
```

**3. Configure the connection string**

Edit `PharmacyAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PharmacyDB;Trusted_Connection=True;"
  }
}
```

**4. Apply migrations**

```bash
cd PharmacyAPI
dotnet ef database update
```

**5. Generate seed password hashes**

```bash
cd ../HashGen
dotnet run
# Copy the output hashes into your seed SQL script
```

**6. Start the API**

```bash
cd ../PharmacyAPI
dotnet run
# → https://localhost:5001
```

**7. Open the frontend**

Open `PharmacyClient/index.html` directly in your browser, or serve it with a local static server:

```bash
cd ../PharmacyClient
npx serve .
# → http://localhost:3000
```

---

## Contributing

1. Fork the repository
2. Create a feature branch — `git checkout -b feature/your-feature`
3. Commit your changes — `git commit -m 'Add your feature'`
4. Push to the branch — `git push origin feature/your-feature`
5. Open a pull request

---

*Built by [@worksbyrohith](https://github.com/worksbyrohith)*
