# Inventory Management System

A full-stack web application built with ASP.NET MVC and MySQL database to manage 
inventory, suppliers, and stock movements for manufacturing and petrochemical 
industries — targeting real-world client domains served by enterprise software 
companies.

## 🔗 Links
- **GitHub:** https://github.com/DveshKaushik/InventoryManagementSystem
- **Live Demo:** Coming soon (Railway deployment)

## 🛠️ Technologies Used

### Backend
- ASP.NET MVC (.NET 8)
- C#
- MySQL 8.0
- MySQL Workbench or any MySQL Client
- Groq API + LLaMA 3.3 70B (AI features)
- DotNetEnv (secure API key management)

### Frontend
- Bootstrap 5 (responsive UI)
- Chart.js (stock level visualization)
- Bootstrap Icons
- Razor (.cshtml templating)

### DevOps
- Git + GitHub (version control)
- GitHub Actions (CI/CD pipeline)
- Docker + docker-compose (containerization)

## ✨ Features

### Core Modules
- 📊 **Dashboard** — live stats cards, stock level bar chart, low stock alerts
- 📦 **Product Management** — add, view, delete products with category and supplier
- 🏭 **Supplier Management** — manage suppliers with conflict detection on delete
- 📋 **Stock Movements** — record stock IN/OUT with automatic quantity updates
- ⚠️ **Low Stock Alerts** — automatic detection and highlighting of items below minimum level

### AI Features
- 🤖 **AI Stock Analyzer** — ask questions about inventory health, get reorder recommendations
- 💬 **AI Inventory Chatbot** — natural language queries on real inventory data

### Technical Highlights
- JOIN queries across 4 tables
- Parameterized queries (SQL injection prevention)
- Foreign key constraint handling
- Real-time stock level calculations
- Dependency injection pattern
- CI/CD pipeline via GitHub Actions
- Docker containerization

## 🗄️ Database Structure
Categories
CategoryId (PK)
Name
Description
Suppliers
SupplierId (PK)
Name, Contact, Phone, Email, Address
Products
ProductId (PK)
CategoryId (FK) → Categories
SupplierId (FK) → Suppliers
Name, Quantity, MinStockLevel
UnitPrice, Description
StockMovements
MovementId (PK)
ProductId (FK) → Products
MovementType (IN/OUT)
Quantity, MovementDate, Reason

## 🚀 How to Run Locally

### Prerequisites
- .NET 8 SDK
- MySQL Server
- MySQL Workbench / CLI or docker

### Steps

1. Clone the repository
```bash
git clone https://github.com/DveshKaushik/InventoryManagementSystem.git
cd InventoryManagementSystem
```

2. Set up the database in MySQL:
```sql
CREATE DATABASE InventoryManagementDB;
```
Then run the table creation and seeding scripts from `Database/setup_mysql.sql`

3. Create `.env` file in project root:
GROQ_API_KEY=your_groq_key_here

4. Update connection string in `appsettings.json`:
```json
"DefaultConnection": "Server=localhost;Database=InventoryManagementDB;Uid=root;Pwd=YourStrongPassword!;"
```

5. Run the application:
```bash
dotnet run
```

6. Open browser:
http://localhost:5001

### Run with Docker
```bash
docker-compose up --build
```
Open browser: http://localhost:8081

## ⚙️ CI/CD Pipeline

Every push to main branch automatically:
- Checks out code
- Sets up .NET 8
- Restores dependencies
- Builds the project
- Publishes release version

## 🔒 Security
- API keys stored in `.env` file — never pushed to GitHub
- `.env` is gitignored
- Parameterized SQL queries prevent SQL injection
- GitHub Secrets used for CI/CD pipeline

## 🤖 AI Integration
- **Model:** LLaMA 3.3 70B via Groq API (free tier)
- **Stock Analyzer:** Ask inventory questions → AI analyzes real data → gives actionable recommendations
- **Chatbot:** Natural language queries about products, suppliers and stock levels

## 📁 Project Structure
InventoryManagementSystem/
│
├── Controllers/
│   ├── HomeController.cs      ← Dashboard
│   ├── ProductController.cs   ← Product CRUD + Stock management
│   ├── SupplierController.cs  ← Supplier CRUD
│   ├── StockMovementController.cs ← Stock IN/OUT tracking
│   └── AIController.cs        ← AI Analyzer + Chatbot
│
├── Models/
│   ├── Product.cs
│   ├── Supplier.cs
│   ├── Category.cs
│   ├── StockMovement.cs
│   ├── DashboardViewModel.cs
│   ├── DatabaseHelper.cs
│   └── GroqService.cs
│
├── Views/
│   ├── Home/         ← Dashboard
│   ├── Product/      ← Product pages
│   ├── Supplier/     ← Supplier pages
│   ├── StockMovement/ ← Movement pages
│   ├── AI/           ← AI pages
│   └── Shared/       ← Layout + navbar
│
├── .github/workflows/
│   └── build.yml     ← CI/CD pipeline
│
├── Dockerfile        ← Docker image config
├── docker-compose.yml ← Multi-container setup
├── .env.example      ← Environment variables template
└── appsettings.json  ← App configuration
