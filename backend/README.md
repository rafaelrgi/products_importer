# Products Importer Backend (.NET)

A high-performance RESTful API built with ASP.NET Core, engineered to handle large-scale data processing and complex business logic for product synchronization.

<br>

## Technology Stack

* **Architecture:** Clean Architecture REST Api 
* **Framework:** .NET 9 / ASP.NET Core
* **Language:** C#
* **Database:** Entity Framework Core (PostgreSQL)

<br>

## Key Architectural & Technical Highlights

This service was designed to demonstrate mastery over enterprise patterns and cloud-ready development:

* **Clean Architecture:** Strict separation between Domain, Application, Infrastructure, and API layers
* **Bulk Ingestion Engine:** Optimized for processing large JSON files with low memory footprint using streaming techniques
* **Unit & Integration Testing:** Robust test suite using xUnit ensure domain integrity

<br>

## 🛠 Setup & Run

1. Navigate to the directory: `cd backend`
2. Restore dependencies: `dotnet restore`
3. Update database: `dotnet ef database update`
4. Run the API: `dotnet run`