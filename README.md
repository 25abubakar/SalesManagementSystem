📊 Sales Management System

A complete ASP.NET Core MVC + Entity Framework Core based Sales Management System designed to manage sales transactions, accounts, products, charges, and date tracking in a structured ERP-style workflow.

📸 Screenshots
<img width="727" height="589" alt="image" src="https://github.com/user-attachments/assets/b0b21d30-8531-430c-8651-03072bbffac6" />
<img width="458" height="590" alt="image" src="https://github.com/user-attachments/assets/68968c54-79ba-4d2c-8b71-10e52c05c2cc" />
<img width="1083" height="643" alt="image" src="https://github.com/user-attachments/assets/e2ad1ae0-7d9f-4d75-8248-02861538c093" />
<img width="885" height="726" alt="image" src="https://github.com/user-attachments/assets/ef373b10-5c8c-4b50-b146-34ac4139601b" />

# Sales Management System — Developer Documentation

**Framework:** ASP.NET Core MVC (.NET 10)  
**Database:** SQL Server (LocalDB for development)  
**Developer:** Abubakar Chughtai  

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [Project Structure](#3-project-structure)
4. [Database Design](#4-database-design)
5. [Models](#5-models)
6. [Controllers](#6-controllers)
7. [Repository Pattern](#7-repository-pattern)
8. [Background Jobs (Hangfire)](#8-background-jobs-hangfire)
9. [Real-Time Updates (SignalR)](#9-real-time-updates-signalr)
10. [Dashboard](#10-dashboard)
11. [Authentication (Partial)](#11-authentication-partial)
12. [Validation Rules](#12-validation-rules)
13. [Setup & Configuration](#13-setup--configuration)
14. [Known Issues & Future Improvements](#14-known-issues--future-improvements)

---

## 1. Project Overview

Sales Management System is an ERP-style web application for managing sales transactions. It tracks every sale from creation through to profit calculation, including:

- Products and platforms
- From/To accounts
- Multiple charge types per sale
- Multiple transaction dates per sale
- Automatic profit calculation via a SQL computed column
- Real-time UI updates via SignalR
- Auto-fill of missing data via Hangfire background jobs

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Web Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core |
| Micro ORM | Dapper (for stored procedure calls) |
| Database | SQL Server / LocalDB |
| Background Jobs | Hangfire |
| Real-Time | SignalR |
| UI | Bootstrap 5 + Razor Views |
| Session | ASP.NET Core Session |

---

## 3. Project Structure

```
SalesManagementSystem/
├── Controllers/          # MVC Controllers (one per feature)
├── Data/
│   ├── AppDbContext.cs   # EF Core DbContext (ApplicationDbContext)
│   └── DapperContext.cs  # Dapper connection factory
├── Filters/
│   └── SessionAuthorizeAttribute.cs  # Session-based auth filter (disabled)
├── Hubs/
│   └── SalesHub.cs       # SignalR hub for real-time sale events
├── Jobs/
│   └── SaleChargeJob.cs  # Hangfire background job
├── Migrations/           # EF Core migration history
├── Models/               # Domain models + ViewModels
├── Repository/
│   ├── ISaleRepository.cs
│   └── SaleRepository.cs # Dapper-based repository
├── Views/                # Razor views per controller
├── wwwroot/              # Static files (CSS, JS, libs)
├── Program.cs            # App startup & DI configuration
└── appsettings.json      # Connection strings & logging config
```

---

## 4. Database Design

### Tables

| Table | Description |
|---|---|
| `SaleAcct` | Main sale record — core transaction data |
| `SaleProduct` | Product master |
| `SalePlatform` | Platform master (e.g. Amazon, eBay) |
| `SaleAccount` | From/To account master |
| `SaleTransactionType` | Transaction type master (e.g. Sale, Return) |
| `SaleStatus` | Status master (e.g. Pending, Completed) |
| `SaleChargeType` | Charge type master (e.g. Amazon Fee, Shipping) |
| `SaleCharge` | Charges linked to a sale |
| `SaleDate` | Date label master (seeded, read-only) |
| `SaleTransactionDate` | Multiple dates per sale |

### Views

| View | Description |
|---|---|
| `SaleTransactionDatesPivot` | Pivots transaction dates into columns per sale |

### Key Relationships

```
SalePlatform (1) ──── (M) SaleProduct
SalePlatform (1) ──── (M) SaleAccount

SaleAcct (M) ──── (1) SalePlatform
SaleAcct (M) ──── (1) SaleProduct
SaleAcct (M) ──── (1) SaleTransactionType
SaleAcct (M) ──── (1) SaleAccount  [FromAccount]
SaleAcct (M) ──── (1) SaleAccount  [ToAccount]
SaleAcct (M) ──── (1) SaleStatus

SaleAcct (1) ──── (M) SaleCharge
SaleCharge (M) ──── (1) SaleChargeType

SaleAcct (1) ──── (M) SaleTransactionDate
SaleTransactionDate (M) ──── (1) SaleDate
```

### Computed Column — ProfitAmount

`ProfitAmount` is calculated directly in SQL Server as a **stored computed column** on `SaleAcct`:

```sql
ISNULL(SoldAmount, 0)
- ISNULL(CostAmount, 0)
- ISNULL(TotalProCharges, 0)
- ISNULL(AmazonFee, 0)
- ISNULL(OtherCharges, 0)
+ ISNULL(TotalRroRebate, 0)
```

### Unique Index

```sql
UNIQUE INDEX on SaleAcct (PlatformId, ProductId, OrderID)
WHERE OrderID IS NOT NULL
```

Prevents duplicate Order IDs for the same platform/product combination.

### Database Triggers (Audit)

The following tables have SQL triggers registered (audit trail):

- `SaleAcct` → `TR_SaleAcct_Audit`
- `SaleCharge` → `TR_SaleCharge_Audit`
- `SaleAccount` → `TR_SaleAccount_Audit`
- `SaleProduct` → `TR_SaleProduct_Audit`

### Seeded Data — SaleDate

The `SaleDate` table is seeded automatically via EF Core migrations:

| Id | DateLabel |
|---|---|
| 1 | TransactionDate |
| 2 | PaymentDate |
| 3 | OrderDate |
| 4 | ProcessDate |
| 5 | SoldDate |

These labels are **read-only** — the Create/Edit/Delete actions are disabled in `SaleDateController`.

---

## 5. Models

### SaleAcct
Main sale record. All financial fields are `decimal(18,2)`.

| Property | Type | Notes |
|---|---|---|
| Id | long | Primary key |
| CompanyId | int? | Auto-assigned from Platform |
| PlatformId | int? | FK → SalePlatform |
| ProductId | int? | FK → SaleProduct |
| OrderID | string? | Unique per Platform+Product |
| QtyHeld | decimal? | Quantity held |
| QtySold | decimal? | Quantity sold |
| TransactionId | int? | FK → SaleTransactionType |
| FromAccountId | int? | FK → SaleAccount |
| ToAccountId | int? | FK → SaleAccount |
| TotalProCharges | decimal? | |
| AmazonFee | decimal? | |
| OtherCharges | decimal? | |
| TotalPromotion | decimal? | |
| SoldAmount | decimal? | |
| TotalRroRebate | decimal? | |
| CostAmount | decimal? | |
| ProfitAmount | decimal? | **Computed column** (read-only) |
| AmzProRef | string? | Amazon promo reference |
| Status | string? | Free-text status |
| StatusID | int? | FK → SaleStatus |
| Action | string? | Free-text action note |
| CreatedDate | DateTime? | |
| Charges | List\<SaleCharge\> | Navigation property |
| SaleTransactionDates | ICollection\<SaleTransactionDate\> | Navigation property |
| ChargeTypesCsv | string? | `[NotMapped]` — populated at runtime |
| TransactionDatesCsv | string? | `[NotMapped]` — populated at runtime |

---

### SaleAcctCreateVM
ViewModel used for both Create and Edit forms. Contains inline lists for charges and dates.

| Property | Notes |
|---|---|
| Charges | `List<SaleChargeEntryVM>` — dynamic rows |
| SaleTransactionDates | `List<SaleTransactionDateEntryVM>` — dynamic rows |
| ProfitAmount | Read-only display on Edit |

---

### SaleCharge

| Property | Type | Notes |
|---|---|---|
| SaleChargeId | long | PK |
| SaleId | long | FK → SaleAcct |
| ChargeTypeId | int | FK → SaleChargeType |
| Amount | decimal | |
| Remarks | string? | Max 250 chars |

---

### SaleTransactionDate

| Property | Type | Notes |
|---|---|---|
| Id | int | PK |
| SaleAcctId | long | FK → SaleAcct |
| DateLabelId | int | FK → SaleDate |
| Date | DateTime | Required |

---

### SaleTransactionDatePivot
Maps to the `SaleTransactionDatesPivot` SQL view. Has no key (`HasNoKey()`).

| Property | Type |
|---|---|
| SaleAcctId | long |
| TransactionDate | DateTime? |
| PaymentDate | DateTime? |
| OrderDate | DateTime? |
| ProcessDate | DateTime? |
| SoldDate | DateTime? |

---

### DashboardViewModel

| Property | Type | Notes |
|---|---|---|
| TotalSales | int | Count of all sales |
| TotalSoldAmount | decimal | Sum of SoldAmount |
| TotalProfitAmount | decimal | Sum of ProfitAmount |
| AverageProfitAmount | decimal | TotalProfit / TotalSales |
| MonthlySales | List\<DashboardChartPoint\> | Grouped by Month-Year |
| RecentSales | List\<SaleAcct\> | All sales ordered by CreatedDate desc |

---

## 6. Controllers

### HomeController
**Route:** `/Home`

| Action | Method | Description |
|---|---|---|
| `Index` | GET | Dashboard — totals, monthly chart data, recent sales |
| `Privacy` | GET | Privacy page |
| `Error` | GET | Error page |

---

### SaleAcctController
**Route:** `/SaleAcct`  
The core controller. Uses EF Core with full `Include()` chains.

| Action | Method | Description |
|---|---|---|
| `Index` | GET | List all sales with charges and dates |
| `Create` | GET | Empty create form |
| `Create` | POST | Save new sale + charges + dates |
| `Edit(id)` | GET | Load sale into edit form |
| `Edit(id)` | POST | Update sale + sync charges + replace dates |
| `Details(id)` | GET | Read-only detail view |
| `Print(id)` | GET | Print-friendly view |
| `Delete(id)` | GET | Delete confirmation view |
| `DeleteConfirmed(id)` | POST | Delete sale record |

**Private helpers:**

| Method | Purpose |
|---|---|
| `PopulateDropDowns` | Loads all ViewBag dropdowns for the form |
| `PopulateSqlSummaryColumns` | Fills `ChargeTypesCsv` and `TransactionDatesCsv` on each sale |
| `BuildTransactionDateSummary` | Formats pivot dates into a readable string |
| `AssignCompanyFromPlatformAsync` | Auto-sets CompanyId from selected Platform |
| `ValidateDuplicateSaleAsync` | Checks for duplicate OrderID |
| `ValidateProductPlatformMappingAsync` | Ensures Product belongs to selected Platform |
| `ValidateToAccountPlatformMappingAsync` | Ensures ToAccount belongs to selected Platform |

**Hangfire scheduling (on Create):**
```csharp
// Fires if user skipped charges OR transaction dates
if (missingCharges || missingDates)
{
    _backgroundJobs.Schedule<ISaleChargeJob>(
        x => x.CreateAutoChargeAndDateIfMissingAsync(sale.Id),
        TimeSpan.FromMinutes(1));
}
```

**SignalR events broadcast:**
- `"SalesUpdated", "created", sale.Id`
- `"SalesUpdated", "updated", sale.Id`
- `"SalesUpdated", "deleted", id`

---

### SaleChargeController
**Route:** `/SaleCharge`  
Standalone CRUD for charges. Supports filtering by `saleId`.

| Action | Method | Description |
|---|---|---|
| `Index(?saleId)` | GET | List charges, optionally filtered by sale |
| `Create(?saleId)` | GET | Create form pre-filled with saleId |
| `Create` | POST | Save charge |
| `Edit(id)` | GET/POST | Edit charge |
| `Delete(id)` | GET/POST | Delete charge |
| `Details(id)` | GET | View charge detail |

---

### SaleChargeTypeController
**Route:** `/SaleChargeType`  
Full CRUD for charge type master. Duplicate name check (case-insensitive).

---

### SaleTransactionDateController
**Route:** `/SaleTransactionDate`  
Manages transaction dates. Index uses the **pivot view** (`SaleTransactionDatesPivot`).

| Action | Method | Description |
|---|---|---|
| `Index(?saleId)` | GET | Pivot view of dates per sale |
| `Create(?saleId)` | GET/POST | Add a date row (duplicate label check) |
| `Edit(id)` | GET/POST | Edit a date row |
| `Delete(id)` | GET/POST | Delete a date row |

---

### SalePlatformController
**Route:** `/SalePlatform`  
Full CRUD for platform master. Requires `CompanyId`. Duplicate name check.

---

### SaleProductController
**Route:** `/SaleProduct`  
Full CRUD for product master. Product is scoped to a Platform. `CompanyId` auto-assigned from Platform.

---

### SaleAccountController
**Route:** `/SaleAccount`  
Full CRUD for account master (From/To accounts). Scoped to a Platform. `CompanyId` auto-assigned.

---

### SaleStatusController
**Route:** `/SaleStatus`  
Full CRUD for status master. Duplicate name check.

---

### SaleTransactionTypeController
**Route:** `/SaleTransactionType`  
Full CRUD for transaction type master. Duplicate name check.

---

### SaleDateController
**Route:** `/SaleDate`  
**Read-only.** Date labels are system-defined via seed data. Create/Edit/Delete are disabled. Index shows the list only.

---

### AuthController
**Currently fully commented out.** Session-based login was implemented but disabled. See [Authentication](#11-authentication-partial) section.

---

## 7. Repository Pattern

The project uses a dual data access approach:

### EF Core (Primary)
Used directly in all controllers via `ApplicationDbContext`. Handles all CRUD with navigation property loading.

### Dapper (Secondary)
Used via `SaleRepository` for stored procedure calls.

**`DapperContext`** — singleton service that creates `IDbConnection` from the connection string.

**`ISaleRepository` / `SaleRepository`:**

| Method | Stored Procedure |
|---|---|
| `GetAll()` | `sp_Sale_GetAll` |
| `GetById(id)` | `sp_Sale_GetById` |
| `Create(sale)` | `sp_Sale_Insert` |
| `Update(sale)` | `sp_Sale_Update` |
| `Delete(id)` | `sp_Sale_Delete` |

> **Note:** `SaleRepository` is registered in DI but the main `SaleAcctController` uses EF Core directly. The repository is available for use if stored procedure-based access is preferred.

---

## 8. Background Jobs (Hangfire)

### Setup
Hangfire uses SQL Server storage (same connection string as the app).

```csharp
builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
```

Dashboard is available at: `/hangfire`

### SaleChargeJob

**Registered as:** `IBackgroundJobClient.Schedule(...)` with a 1-minute delay.

**Triggered when:** A new sale is created and either charges or transaction dates are missing.

#### `CreateAutoChargeAndDateIfMissingAsync(long saleId)`
Entry point. Calls both methods below in sequence.

#### `CreateAutoChargeIfMissingAsync(long saleId)`
- Loads the sale including its `TransactionType`
- Skips if the sale already has charges
- Calls `GetOrCreateChargeTypeMatchingTransactionAsync` to find/create the right `SaleChargeType`
- Creates a `SaleCharge` with `Amount = SoldAmount ?? 0`

#### `CreateAutoTransactionDateIfMissingAsync(long saleId)`
- Skips if the sale already has transaction dates
- Uses the first `SaleDate` label (Id = 1, "TransactionDate")
- Creates a `SaleTransactionDate` with `Date = DateTime.Today`

#### `GetOrCreateChargeTypeMatchingTransactionAsync(string? transactionName)`
| Scenario | Behaviour |
|---|---|
| Sale has a TransactionType name | Looks for a `SaleChargeType` with the same name |
| Match found | Uses existing `SaleChargeType` |
| No match | Creates a new `SaleChargeType` with that name |
| Sale has no TransactionType | Falls back to first existing `SaleChargeType` |
| No `SaleChargeType` exists at all | Creates `"Auto Charge"` as default |

---

## 9. Real-Time Updates (SignalR)

**Hub:** `SalesHub` at `/hubs/sales`

The hub itself is empty — it acts as a broadcast channel. Events are sent from `SaleAcctController` using `IHubContext<SalesHub>`.

### Events

| Event Name | Trigger | Payload |
|---|---|---|
| `SalesUpdated` | Sale created | `"created", saleId` |
| `SalesUpdated` | Sale updated | `"updated", saleId` |
| `SalesUpdated` | Sale deleted | `"deleted", saleId` |

### Client-Side Usage
Listen in JavaScript:
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/sales")
    .build();

connection.on("SalesUpdated", (action, saleId) => {
    console.log(`Sale ${saleId} was ${action}`);
    // refresh table, show notification, etc.
});

connection.start();
```

---

## 10. Dashboard

**Route:** `/` or `/Home/Index`

The dashboard aggregates data directly from `SaleAccts` using EF Core:

| Metric | Query |
|---|---|
| Total Sales | `COUNT(*)` |
| Total Sold Amount | `SUM(SoldAmount)` |
| Total Profit Amount | `SUM(ProfitAmount)` |
| Average Profit | `TotalProfit / TotalSales` |
| Monthly Sales Chart | Grouped by `Month-Year` of `CreatedDate` |
| Recent Sales | All sales ordered by `CreatedDate DESC` |

---

## 11. Authentication (Partial)

Authentication is **implemented but disabled**. Both the controller and the filter are fully commented out.

### What exists (commented out):
- `AuthController` — Login/Logout with hardcoded credentials (`laltech` / `lal@123`)
- `SessionAuthorizeAttribute` — Action filter that checks `HttpContext.Session["User"]`
- `[SessionAuthorize]` attribute on `HomeController` (commented out)
- `app.UseAuthorization()` in `Program.cs` (commented out)

### To re-enable:
1. Uncomment `AuthController.cs`
2. Uncomment `SessionAuthorizeAttribute.cs`
3. Add `[SessionAuthorize]` to controllers that need protection
4. Uncomment `app.UseAuthorization()` in `Program.cs`
5. Replace hardcoded credentials with a proper user store

---

## 12. Validation Rules

### SaleAcct (Create & Edit)

| Rule | Details |
|---|---|
| Required fields | Platform, Product, OrderID, QtySold, TransactionType, FromAccount, ToAccount, SoldAmount, CostAmount, Status |
| Duplicate OrderID | Case-insensitive check across all sales (excluding self on edit) |
| CompanyId | Auto-assigned from Platform — cannot be manually overridden |
| Product ↔ Platform | Product must belong to the selected Platform |
| ToAccount ↔ Platform | ToAccount must belong to the selected Platform |
| Charge rows | Each row must have both ChargeType and Amount |
| Duplicate charge types | Same ChargeType cannot appear twice in one sale |
| Date rows | Each row must have both DateLabel and Date |
| Duplicate date labels | Same DateLabel cannot appear twice in one sale |

### Master Tables (Platform, Product, Account, Status, ChargeType, TransactionType)

All master table controllers enforce:
- **Duplicate name check** (case-insensitive, scoped to platform where applicable)
- **CompanyId auto-assignment** from Platform (for Product and Account)

---

## 13. Setup & Configuration

### Prerequisites
- .NET 10 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022+ or VS Code

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/25abubakar/salesmanagementsystem.git
```

**2. Configure the connection string**

Edit `SalesManagementSystem/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SalesDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**3. Apply migrations**

In Package Manager Console:
```powershell
Update-Database
```

Or via CLI:
```bash
dotnet ef database update
```

**4. Run the application**
```bash
dotnet run --project SalesManagementSystem
```

Or press F5 in Visual Studio.

**5. Access Hangfire Dashboard**

Navigate to `/hangfire` to monitor background jobs.

### appsettings.json Overview

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."   // Used by EF Core, Dapper, and Hangfire
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Program.cs Service Registration

| Service | Registration |
|---|---|
| `ApplicationDbContext` | `AddDbContext` with SQL Server |
| `DapperContext` | `AddSingleton` |
| `ISaleRepository` | `AddScoped<ISaleRepository, SaleRepository>` |
| `ISaleChargeJob` | `AddScoped<ISaleChargeJob, SaleChargeJob>` |
| Hangfire | `AddHangfire` + `AddHangfireServer` |
| SignalR | `AddSignalR` |
| Session | `AddSession` |

---

## 14. Known Issues & Future Improvements

### Known Issues

| Issue | Details |
|---|---|
| Auth disabled | Session-based authentication is fully commented out — app is currently open access |
| Dapper repository unused | `SaleRepository` is registered but `SaleAcctController` uses EF Core directly |
| `SaleController.cs` exists | There is a `SaleController.cs` alongside `SaleAcctController.cs` — likely a legacy file |
| `SaleAcct.cs` vs `SaleAccount.cs` | Two similarly named models — `SaleAcct` is the main transaction, `SaleAccount` is the account master |

### Planned Improvements (from README)

- Invoice generation (PDF export)
- Dashboard analytics enhancements
- REST API integration
- Role-based authentication
- Reporting system

