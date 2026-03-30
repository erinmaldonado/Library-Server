# Library Project — Local Dev Setup Guide

## Prerequisites

Make sure you have the following installed:

- [Git](https://git-scm.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- [Angular CLI](https://angular.io/cli) — install with `npm install -g @angular/cli`

---

## Folder Structure

```
Library-Project/
├── Library-Client-Dev/        ← Git repo → GitHub (Angular frontend)
│   └── Library-Client-Dev/
│       ├── src/
│       ├── public/
│       └── ...
├── Library-Server-Dev/        ← Git repo → GitHub (.NET Web API)
│   └── Library-Server-Dev/
│       ├── Controllers/
│       ├── DTOs/
│       └── Data/
└── Project.sln
```

---

## Setup Steps

### 1. Create the parent folder and clone the repos

```bash
mkdir Library-Project
cd Library-Project

git clone https://github.com/erinmaldonado/Library-Client-Dev.git
git clone https://github.com/erinmaldonado/Library-Server-Dev.git
```

### 2. Create the .NET Web API project (server)

```bash
cd Library-Server-Dev
dotnet new webapi -n Library-Server-Dev
cd ..
```

Then create the server subfolders:

```bash
mkdir Library-Server-Dev/Library-Server-Dev/Controllers
mkdir Library-Server-Dev/Library-Server-Dev/DTOs
mkdir Library-Server-Dev/Library-Server-Dev/Data
```

### 3. Create the Angular project (client)

```bash
cd Library-Client-Dev
rm -rf Library-Client-Dev   # remove any empty placeholder folder first
ng new Library-Client-Dev
# Choose: Sass (SCSS) for stylesheets
# Choose: N for Server-Side Rendering
cd ..
```

### 4. Create the solution file and add the server project

```bash
dotnet new sln -n Project
dotnet sln Project.sln add Library-Server-Dev/Library-Server-Dev/Library-Server-Dev.csproj
```

> Note: The Angular client does not have a `.csproj` file and does not need to be added to the solution.

### 5. Make the initial commit and push both repos

**Client:**
```bash
cd Library-Client-Dev
git add .
git commit -m "initial commit - frontend"
git push origin main
cd ..
```

**Server:**
```bash
cd Library-Server-Dev
git add .
git commit -m "initial commit - backend"
git push origin main
cd ..
```

---

## Tips

- The `Library-Project/` parent folder is **local only** — it is not its own Git repo.
- Each subfolder (`Library-Client-Dev/`, `Library-Server-Dev/`) is its own independent Git repo.
- To run the Angular app: `cd Library-Client-Dev/Library-Client-Dev && ng serve`
- To run the .NET API: `cd Library-Server-Dev/Library-Server-Dev && dotnet run`
- In the terminal, use **Shift + Enter** for a new line without sending.
