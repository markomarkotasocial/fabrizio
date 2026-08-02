# Fabrizio App

A cross-platform travel planning application built with **.NET MAUI**, featuring a layered **.NET 8** backend hosted on **Azure**. The project demonstrates a full end-to-end architecture — from a mobile client through a REST API, business logic, and data access layers, backed by an Azure-hosted SQL Server database.

> **Status:** Phase 1 — Core architecture & MVP features complete. Built with a deliberate focus on establishing a solid, scalable foundation (layered architecture, shared contracts, cloud infrastructure) before expanding feature scope. See [Roadmap](#roadmap) for planned directions.

<br>

## Overview

Fabrizio App lets users plan and manage trips: each trip has a name, description, date range, status, and a list of destinations (not individually date-bound). The app also includes account/app settings (language, theme, currency, timezone).

The project intentionally started with an architecture-first approach — the goal was to correctly set up a maintainable, layered solution using the latest .NET/MAUI stack before layering on additional functionality. The domain (trip planning) has room to expand in several directions (e.g. AI-assisted itinerary building, partner service integrations, or a broader digital-nomad focused product).

<br>

## Architecture

The solution is organized into clearly separated projects:

```
fabrizio app (solution)
├── fabrizio.API          → REST API, controllers, JWT authentication
├── fabrizio.App          → .NET MAUI client (mobile)
├── fabrizio.BLL          → Business logic layer
├── fabrizio.DAL          → Data access layer (EF Core)
├── fabrizio.Repository   → Repository pattern implementation
└── fabrizio.Shared       → Shared DTOs & contracts used by both API and MAUI client
```

<br>

```
┌─────────────────┐        HTTPS / JWT        ┌──────────────────┐
│  fabrizio.App    │ ─────────────────────────▶│  fabrizio.API     │
│  (.NET MAUI)     │                            │                    │
└─────────────────┘                            └─────────┬────────┘
                                                            │
                                    ┌────────────┐  ┌───────▼────────┐
                                    │ fabrizio.  │◀─┤ fabrizio.BLL   │
                                    │ Shared     │  └───────┬────────┘
                                    │ (DTOs)     │          │
                                    └────────────┘  ┌───────▼────────┐
                                                     │ fabrizio.      │
                                                     │ Repository /   │
                                                     │ DAL (EF Core)  │
                                                     └───────┬────────┘
                                                             │
                                                     ┌───────▼────────┐
                                                     │ Azure SQL      │
                                                     │ Server         │
                                                     └────────────────┘
```

Sharing DTOs between the API and the MAUI client through `fabrizio.Shared` avoids model duplication and keeps the contract between backend and client consistent as both evolve.

<br>

## Tech Stack

- **.NET 8**
- **.NET MAUI** — cross-platform mobile client
- **Entity Framework Core** — data access / ORM
- **JWT Bearer Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **Azure App Service** — API hosting
- **Azure SQL Server** — database
- **GitHub Actions** — CI/CD, build and deploy to Azure

<br>

## Features

**Implemented**

- Trip management: create, update, list, and filter trips (name, description, date range, status)
- Destinations list per trip
- User authentication via username/password with JWT
- Account & app settings (language, theme, currency, timezone) — persisted to database
- MAUI client with 5 main sections:
  - **Home** — splash view of the current or next upcoming trip
  - **Trips** — trip list with filtering and pull-to-refresh
  - **Discover** — planned: partner/affiliate service suggestions (e.g. activities, bookings)
  - **Hub** — planned: AI-assisted itinerary building from booking documents
  - **Profile** — account and app settings

**In progress / not yet wired up**

- Settings are persisted but not yet reflected in the UI (e.g. theme/language switch)

<br>

## Roadmap

- Google account sign-in (in addition to username/password)
- Apply persisted settings to the UI (theme, language, currency, timezone)
- AI-assisted trip creation from booking documents (Hub page)
- Partner/affiliate service suggestions (Discover page)
- Background processing (Azure Functions / WebJobs) once justified by usage
- Potential domain expansion (e.g. digital-nomad focused features)

<br>

## Getting Started

> This repository is currently private. Local setup instructions below assume access has been granted.

1. Clone the repository
2. Configure the connection string via .NET User Secrets in `fabrizio.API` (never committed to source control)
3. Apply EF Core migrations:
   ```
   dotnet ef database update --project fabrizio.DAL --startup-project fabrizio.API
   ```
4. Run the API:
   ```
   dotnet run --project fabrizio.API
   ```
5. Run the MAUI client (`fabrizio.App`) from Visual Studio, selecting your target platform (Android/iOS/Windows)

<br>

## About This Project

Built as a hands-on exercise to apply the latest .NET 8 / .NET MAUI stack end-to-end — from mobile UI down to cloud infrastructure — with an emphasis on clean layering and shared contracts between client and server.

<br>

---

**Author:** [Your Name] — [LinkedIn/GitHub/portfolio link]
