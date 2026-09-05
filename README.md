# CinemaVerse - Cinema Booking and Management Platform

A full-stack ASP.NET Core MVC application for managing cinemas, movies, showtimes, seats, bookings, and online checkout.

**Repository:** https://github.com/mazenessam2000/P03_Cinema

## Features

### Customer experience
- Browse movies and filter by category.
- View movie details and showtimes.
- Select seats through an interactive seat map.
- Manage a booking cart.
- Complete checkout through Stripe.
- Receive booking confirmation.

### Administration
- Manage movies, actors, categories, cinemas, halls, and showtimes.
- Upload and manage movie images.
- View dashboard data.
- Restrict administrative features through role-based authorization.

## Tech stack

- C# and ASP.NET Core MVC
- Entity Framework Core and SQL Server
- ASP.NET Identity
- Stripe Checkout
- Razor Views, HTML, CSS, JavaScript, Bootstrap
- Repository and Unit of Work patterns

## Architecture

The project separates responsibilities through:

- Controllers for HTTP requests and routing.
- Services for business logic.
- Repositories and Unit of Work for data access.
- ViewModels for UI-specific data.
- EF Core configurations for database mappings.
- ASP.NET Identity for authentication and authorization.

## Local setup

1. Clone the repository.
2. Configure `DefaultConnection` in `appsettings.Development.json`.
3. Configure Stripe keys through User Secrets or environment variables.
4. Run:

```bash
dotnet restore
dotnet run
```
