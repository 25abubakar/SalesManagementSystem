📊 Sales Management System

A complete ASP.NET Core MVC + Entity Framework Core based Sales Management System designed to manage sales transactions, accounts, products, charges, and date tracking in a structured ERP-style workflow.

📸 Screenshots
<img width="727" height="589" alt="image" src="https://github.com/user-attachments/assets/b0b21d30-8531-430c-8651-03072bbffac6" />
<img width="458" height="590" alt="image" src="https://github.com/user-attachments/assets/68968c54-79ba-4d2c-8b71-10e52c05c2cc" />
<img width="1439" height="734" alt="image" src="https://github.com/user-attachments/assets/a6bbecf3-e0eb-4593-904d-2df1f077313a" />

🚀 Features:

🧾 Sales Management:
Create and manage sales accounts (SaleAcct)
Track products, platforms, and transaction types
Manage sale statuses and accounts

📅 Transaction Date Tracking
Multi-date tracking system per sale:
Transaction Date
Payment Date
Order Date
Process Date
Sold Date
Fully dynamic date labeling system

🏷️ Date Label System:
Centralized SaleDate master table
Reusable date labels across transactions
Clean and scalable design

💰 Charges Management:
Add different charge types
Link charges to sales
Automatic profit calculation

📦 Product & Platform Management:
Manage products
Assign platforms
Link products to sales

🏗️ Tech Stack:
ASP.NET Core MVC
Entity Framework Core
SQL Server
Bootstrap 5
Razor Views

🗂️ Database Structure:
Main Tables:
SaleAcct
SaleTransactionDate
SaleDate
SaleProduct
SalePlatform
SaleCharge
SaleChargeType
SaleAccount
SaleStatus

🔗 Key Relationships:

SaleAcct (1) ──── (M) SaleTransactionDate
SaleDate (1) ──── (M) SaleTransactionDate
SaleAcct (1) ──── (M) SaleCharge
SaleProduct → SalePlatform
⚙️ Setup Instructions:

1. Clone Repository:
git clone https://github.com/25abubakar/salesmanagementsystem.git

2. Open Project:
SalesManagementSystem.sln

4. Configure Database:
Update connection string in:
appsettings.json
Example:
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDatabase;Trusted_Connection=True;"
}

5. Run Migrations:
Add-Migration InitialCreate
Update-Database

🎯 System Highlights:
Clean ERP-style architecture
Fully relational database design
Scalable multi-date tracking system
Professional UI with Bootstrap
Proper EF Core relationships

📌 Future Improvements:
Invoice generation (PDF)
Dashboard analytics
API integration
Role-based authentication
Reporting system

👨‍💻 Developer:
Abubakar Chughtai

📜 License:
This project is open-source and free to use for learning and development purposes.
