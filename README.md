# CalapanCarRentalMVC

A web-based car rental management application built with ASP.NET MVC. It streamlines vehicle inventory, reservations, pricing, and customer management for a rental business in Calapan.

## Features
- Vehicle catalog with availability status and pricing
- Booking workflow with pickup/return dates and cost calculation
- Customer profiles and reservation history
- Admin CRUD for cars, rates, and bookings
- Basic reporting/overview dashboard
- Responsive UI (HTML/CSS) with server-side rendering in C#

## Tech Stack
- **Framework:** ASP.NET MVC (C#)
- **Views:** Razor, HTML, CSS
- **Data:** Entity Framework (assumed) with SQL database
- **Auth:** ASP.NET Identity (optional — update if different)
- **Build/Tools:** .NET SDK (version to match the project), NuGet

## Getting Started

### Prerequisites
- .NET SDK (version aligned with the project, e.g., .NET 6/7 or .NET Framework if applicable)
- SQL Server / LocalDB (or your configured database)
- Node/npm (only if front-end tooling is used)

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/emmanetan/CalapanCarRentalMVC.git
   cd CalapanCarRentalMVC
   ```
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Configure the connection string in `appsettings.json` (or `web.config` if .NET Framework).
4. Apply migrations / ensure database exists:
   ```bash
   dotnet ef database update
   ```
5. Run the app:
   ```bash
   dotnet run
   ```
   Then open the URL shown in the console (e.g., https://localhost:5001).

### Configuration
- **Connection Strings:** Update `DefaultConnection` to point to your SQL instance.
- **Seeding:** Add sample vehicles and rates in the migration seed or a data initializer.
- **Identity/Auth:** Configure password policies, external logins, or role management as needed.

## Project Structure
- `Controllers/` — MVC controllers for cars, bookings, customers, and admin operations
- `Models/` — Domain models and view models
- `Views/` — Razor views for pages and partials
- `wwwroot/` or `Content/` — Static assets (CSS, images, JS)
- `Data/` — DbContext and migrations (if using EF)
- `appsettings.json` or `web.config` — Configuration and connection strings

## Development Notes
- Follow MVC patterns: thin controllers, validation in view models, logic in services where applicable.
- Use data annotations and server-side validation for forms.
- Keep CSS scoped to views/components; prefer layout/shared partials for common UI.
- Add unit/integration tests for booking calculations and availability checks.

## Deployment
- Publish with `dotnet publish -c Release`.
- Deploy to IIS, Azure App Service, or a containerized environment.
- Ensure database connection strings and environment secrets are set via environment variables or platform settings.

## Testing
- Add tests under `Tests/` (xUnit/NUnit/MSTest).
- Sample:
  ```bash
  dotnet test
  ```

## Roadmap Ideas
- Payment integration and invoices
- Improved availability calendar and blackout dates
- Pricing rules (weekend/seasonal rates, promos)
- Email/SMS notifications for bookings
- Role-based dashboards and reports

## License
Specify your license (e.g., MIT) in `LICENSE`.

---
