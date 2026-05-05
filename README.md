# 📚 Library Management System

![.NET](https://img.shields.io/badge/.NET%209-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor%20WASM-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

A full-stack Library Management System built with **ASP.NET Core Web API (.NET 9)** and **Blazor WebAssembly (.NET 8)**. It covers the full lifecycle of library operations — from borrowing and returning books to tracking overdue loans and automatically generating fines.

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│               Blazor WebAssembly (.NET 8)            │
│                                                      │
│  Pages (.razor + .razor.cs)                          │
│       │                                              │
│  Feature Services  (BookApi, LoanApi, FineApi...)    │
│       │                                              │
│  ApiClient  (HttpClient wrapper + CancellationToken) │
└────────────────────┬────────────────────────────────┘
                     │  HTTP + JWT
┌────────────────────▼────────────────────────────────┐
│               ASP.NET Core Web API (.NET 9)          │
│                                                      │
│  Controllers  (thin HTTP handlers)                   │
│       │                                              │
│  Services     (business logic + ServiceResult<T>)   │
│       │                                              │
│  Repositories (EF Core data access)                  │
│       │                                              │
│  SQL Server   (EF Core Code-First)                   │
└─────────────────────────────────────────────────────┘
```

---

## ✨ Features

### 📖 Book Management
- Full CRUD for books (Librarian only)
- Automatic `BookCopy` generation per book based on `totalCopies`
- Available copies tracked in real time

### 🔄 Loan Lifecycle
- **Borrow** — member borrows a book; copy marked `OnLoan`, available count decreases
- **Extend** — member can extend the due date before expiry
- **Overdue** — loans transition to `overdue` status past the due date
- **Return** — librarian returns the book; copy restored to `Available`

### 💰 Fine Management
- Fines automatically generated for overdue loans
- Fine amount increases weekly the longer a book is overdue
- Fines marked as paid automatically when the book is returned
- Members can view their own fines in the **My Fines** tab

### 🔐 Authentication & Authorization
- JWT-based authentication
- Role-based access control: **Librarian** and **Member** roles
- ASP.NET Core Identity for user management

### ⚡ Performance & Robustness
- End-to-end `CancellationToken` propagation — from Blazor component teardown through `HttpClient` → controller → service → EF Core queries
- Abandoned requests abort in-flight SQL queries and release DB connections immediately
- Transactional data operations (borrow, return, create) with full rollback on failure

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly .NET 8 |
| Backend | ASP.NET Core Web API .NET 9 |
| Database | SQL Server + EF Core Code-First |
| Auth | ASP.NET Core Identity + JWT Bearer |
| ORM | Entity Framework Core |
| Architecture | Repository + Service pattern |

---

## 📁 Project Structure

```
LibraryNETApi/
├── Library.Api/                  # ASP.NET Core Web API
│   ├── Controllers/              # HTTP endpoints (thin handlers)
│   ├── Services/                 # Business logic + ServiceResult<T>
│   ├── Repositories/             # EF Core data access (interface + SQL impl)
│   ├── Models/
│   │   ├── Domain/               # EF Core entities
│   │   └── Dto/                  # Request/Response DTOs
│   ├── Data/                     # DbContext + migrations
│   └── Program.cs
│
├── LibraryBlazor/                # Blazor WebAssembly frontend
│   ├── Features/
│   │   ├── Books/                # Book list, create, edit
│   │   ├── Loans/                # Loan list, extend
│   │   ├── MyLoans/              # My Loans + My Fines (member view)
│   │   └── Auth/                 # Login, Register
│   ├── Http/
│   │   └── ApiClient.cs          # Central HttpClient wrapper
│   └── Shared/                   # Layout, nav, shared components
│
└── Library.Api.Tests/            # Unit tests
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or VS Code

### 1. Clone the repository
```bash
git clone https://github.com/basilismt8/LibraryNETApi.git
cd LibraryNETApi
```

### 2. Configure the API

Edit `Library.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "LibraryApi",
    "Audience": "LibraryClient"
  }
}
```

### 3. Apply database migrations
```bash
cd Library.Api
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run --project Library.Api
```

### 5. Run the Blazor app
```bash
dotnet run --project LibraryBlazor
```

The Blazor app will open in your browser. The API runs on `https://localhost:7000` by default (check `launchSettings.json` for the exact port).

---

## 🔑 Default Roles

| Role | Permissions |
|---|---|
| **Librarian** | Full access — manage books, view all loans/fines, return books, process overdue loans |
| **Member** | Borrow books, view own loans and fines, extend loan due dates |

---

## 📄 License

This project is for portfolio and educational purposes.
