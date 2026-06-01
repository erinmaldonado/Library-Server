# 📚 Library Management System

A full-stack web application for managing a library's books, authors, and loans. Built with an ASP.NET Core REST API backend and an Angular 21 frontend, featuring role-based access control, JWT authentication, and a real-time dashboard.

🔗 **[Live Demo](https://library-client-dev.vercel.app/login)**  
🖥️ **[Backend Repo](https://github.com/erinmaldonado/Library-Server)**  
💻 **[Frontend Repo](https://github.com/erinmaldonado/Library-Client)**

---

## Screenshots

### Dashboard
> Personalized welcome with live stats: total books, authors, active loans, and overdue count.

### Books
> Browse the full catalog with title, author, category, publisher, price, and year. Users can borrow books directly from this view.

### Authors
> Manage authors with add, edit, and delete functionality.

### Loans (Admin)
> Admins see all loans across all users with borrowed/due dates, status badges, and the ability to mark loans overdue, process returns, or delete records.

### My Library (User)
> Each user sees only their own borrowed books, due dates, and loan status.

---

## Features

### Authentication & Authorization
- JWT-based login with role-based access (Admin / User)
- Angular HTTP Interceptor automatically attaches Bearer tokens to all API requests
- Protected routes — unauthenticated users are redirected to login

### Books
- Browse full catalog with title, author, category, publisher, price, and publication year
- Add, edit, and delete books (Admin only)
- Borrow a book with one click (User)

### Authors
- View all authors
- Add, edit, and delete authors (Admin only)

### Loans
- Users can borrow books and track them under "My Library"
- Admins see all loans across all users
- Loan statuses: **Active**, **Overdue**, **Returned**
- Admins can mark loans overdue, process returns, or delete loan records

### Dashboard
- Live stats: total books, total authors, active loans, overdue count
- Recent books list on the home screen

---

## Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| ASP.NET Core | REST API framework |
| Entity Framework Core | ORM / database access |
| PostgreSQL | Relational database |
| ASP.NET Core Identity | User management |
| JWT (JSON Web Tokens) | Stateless authentication |

### Frontend
| Technology | Purpose |
|---|---|
| Angular 21 | SPA framework |
| TypeScript | Type-safe development |
| RxJS | Reactive data streams |
| Angular HTTP Interceptor | Automated token injection |

### DevOps
| Tool | Purpose |
|---|---|
| Vercel | Frontend hosting |
| Railway | Backend + PostgreSQL hosting |
| Docker | Containerized backend |
| GitHub | Version control |

---

## Architecture Highlights

- **Server-side pagination** and LINQ query optimization for low-latency data retrieval on large datasets
- **Angular HTTP Interceptor** centralizes auth token injection, keeping API calls clean across the entire app
- **Role-based UI** — admin users see a Loans management nav item and edit/delete controls; regular users see My Library and Borrow buttons
- **Secure identity management** via ASP.NET Core Identity with hashed passwords and JWT token issuance

---

## Getting Started

### Prerequisites
- Node.js 18+
- .NET 8 SDK
- PostgreSQL

### Backend Setup
```bash
git clone https://github.com/erinmaldonado/Library-Server
cd Library-Server
# Add your connection string to appsettings.json
dotnet ef database update
dotnet run
```

### Frontend Setup
```bash
git clone https://github.com/erinmaldonado/Library-Client
cd Library-Client
npm install
ng serve
```

The app will be available at `http://localhost:4200`.

---

## Demo Credentials

| Role | Username | Password |
|---|---|---|
| Admin | admin | admin123 |
| User | user@email.com | user123 |

---

## Author

**Erin Maldonado**  
[LinkedIn](https://linkedin.com/in/erinmaldonado) · [GitHub](https://github.com/erinmaldonado)