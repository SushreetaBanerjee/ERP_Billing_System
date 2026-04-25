# ERP Billing Management System

A real-world inspired **Enterprise Billing Management System** built with **ASP.NET MVC 8**, **Entity Framework Core**, and **SQL Server**. This project covers the complete billing workflow found in enterprise ERP systems — from customer and product management to invoice generation with GST calculation and payment tracking.

---

## Features

- **Dashboard** — Live summary of total revenue, pending amount, paid/unpaid invoice counts and recent 5 invoices
- **Customer Management** — Full CRUD with GST number support and invoice history per customer
- **Product / Service Catalogue** — Manage items with unit price, tax percentage and category
- **Invoice Generation** — Create invoices with dynamic line items, auto GST calculation, subtotal and grand total — unique invoice numbers in INV-0001 format
- **Payment Tracking** — Record full or partial payments, auto-updates invoice status (Unpaid → Partial → Paid)
- **Invoice Filtering** — Filter invoices by status (Paid / Unpaid / Partial)
- **Overdue Detection** — Invoices past due date are highlighted automatically

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | ASP.NET MVC (.NET 8) |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (SSMS) |
| UI | Bootstrap 5 + Bootstrap Icons |
| Architecture | 3-Layer — Repository + Service + Controller |

---

## Architecture

This project follows a clean **3-Layer Architecture**:

```
Controller Layer       → Receives HTTP requests, calls Service, returns View
    ↓
Service Layer          → All business logic, validations, calculations
    ↓
Repository Layer       → All database queries via EF Core
    ↓
SQL Server Database    → ERPBillingDB
```

```
ERPBilling/
├── Controllers/           # Layer 1 — HTTP request handling
├── Services/              # Layer 2 — Business logic
│   └── Interface/         # Service interfaces
├── Repository/            # Layer 3 — EF Core DB queries
│   └── Interface/         # Repository interfaces
├── Models/                # Entity classes + ViewModels
├── Data/                  # ApplicationDbContext
└── Views/                 # Razor views (Bootstrap 5 UI)
```

---

## Database Schema

```
Customers       → Id, Name, Email, Phone, Address, GSTNumber
Products        → Id, Name, Category, UnitPrice, TaxPercent
Invoices        → Id, InvoiceNumber, CustomerId, InvoiceDate, DueDate, Status, TotalAmount
InvoiceItems    → Id, InvoiceId, ProductId, Quantity, UnitPrice, TaxAmount, LineTotal
Payments        → Id, InvoiceId, AmountPaid, PaymentDate, PaymentMode, Notes
```

---

## Getting Started

### Prerequisites
- Visual Studio 2022
- .NET 8 SDK
- SQL Server (Express or full)
- SQL Server Management Studio (SSMS)

### Setup Steps

**1. Clone the repository**
```bash
git clone https://github.com/SushreetaBanerjee/ERP_Billing_System.git
cd ERP_Billing_System
```

**2. Set up the database**
- Open SSMS and connect to your SQL Server
- Run the SQL script located at `ERPBilling_Database.sql`
- This creates the `ERPBillingDB` database with all tables and seed data

**3. Update connection string**

Open `appsettings.json` and update with your SQL Server name:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ERPBillingDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**4. Run the project**
```bash
dotnet run
```
Or press **F5** in Visual Studio.

Open your browser at `https://localhost:7175`

---

## Modules Overview

### Dashboard
- Total revenue collected
- Total pending amount
- Paid vs Unpaid vs Partial invoice counts with progress bars
- Recent 5 invoices with clickable rows

### Customer Management
- Add, edit, view, delete customers
- GST number support
- View full invoice history per customer
- Delete protection — cannot delete customer with existing invoices

### Product / Service Catalogue
- Add products with unit price and GST tax percentage
- Live price + tax preview while adding
- Delete protection — cannot delete products used in invoices

### Invoice Generation
- Dynamic line item form — add multiple products per invoice
- Auto-fills unit price and tax when product is selected
- Real-time calculation of tax amount, line total and grand total
- Unique invoice number auto-generated (INV-0001 format)
- Invoice date and due date selection

### Payment Tracking
- Record full or partial payments against any invoice
- Payment modes — Cash, Bank Transfer, Cheque, UPI, Credit Card
- Invoice status auto-updates after each payment
- Full payment history shown on invoice details page
- Balance due calculated in real time

---

## Key Technical Highlights

- **3-Layer separation** — Controllers never touch DbContext directly
- **Interface-driven design** — All services and repositories use interfaces for loose coupling
- **EF Core relationships** — Cascade delete on line items and payments, restrict delete on customers and products
- **Enum to TINYINT mapping** — InvoiceStatus and PaymentMode stored as TINYINT in SQL
- **Server-side validation** — Data annotations on all model classes
- **TempData alerts** — Success and error messages across redirects
- **Bootstrap 5 sidebar** — Fixed sidebar with active link highlighting

---

## Author

**Sushreeta Banerjee**
[GitHub](https://github.com/SushreetaBanerjee) · [LinkedIn](https://linkedin.com/in/YOUR_LINKEDIN_ID)

---

## License

This project is licensed under the MIT License.
