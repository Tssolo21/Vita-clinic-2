# VitaClinic - Veterinary Management System

## Overview
VitaClinic is a comprehensive veterinary clinic management system built with ASP.NET Core 8.0 and SQLite. It provides a complete solution for managing clients, animals, appointments, medical records, and invoicing for veterinary practices.

## Project Information
- **Technology Stack**: ASP.NET Core 8.0 Web API with Entity Framework Core
- **Database**: SQLite
- **Frontend**: Static HTML/CSS/JavaScript (SPA-style)
- **Port**: 5000 (HTTP only in Replit environment)

## Recent Changes
- **2025-10-20**: Initial Replit setup
  - Upgraded from .NET 6.0 to .NET 8.0
  - Configured application to run on 0.0.0.0:5000 for Replit environment
  - Disabled HTTPS redirection for Replit compatibility
  - Updated CORS policy to allow all origins (required for Replit proxy)
  - Fixed JavaScript bug in index.html (animalsList variable)
  - Created fresh SQLite database with all tables
  - Configured deployment settings for autoscale
  - Added comprehensive .gitignore for .NET projects

## Project Architecture

### Backend Structure
```
├── Controllers/
│   ├── AnimalsController.cs       # Animal management endpoints
│   ├── AppointmentsController.cs  # Appointment scheduling endpoints
│   ├── ClientsController.cs       # Client management endpoints
│   └── WeatherForecastController.cs
├── Data/
│   └── VitaClinicDbContext.cs     # Entity Framework DbContext
├── Models/
│   └── Models.cs                   # Data models for all entities
├── wwwroot/
│   └── index.html                  # Frontend web interface
├── Program.cs                      # Application entry point
└── VitaClinic.WebAPI.csproj        # Project file
```

### Database Schema
The application uses SQLite with the following tables:
- **Clients**: Pet owners with contact information
- **Animals**: Pets with species, breed, weight, vaccination records
- **Appointments**: Scheduled visits with appointment types and status
- **Veterinarians**: Staff members with specialization info
- **MedicalRecords**: Diagnosis, treatment, and medication history
- **Invoices**: Billing information
- **InvoiceItems**: Line items for invoices

### API Endpoints
- `GET /api/clients` - List all clients
- `POST /api/clients` - Create new client
- `GET /api/clients/{id}` - Get client by ID
- `PUT /api/clients/{id}` - Update client
- `DELETE /api/clients/{id}` - Delete client

(Similar endpoints exist for animals and appointments)

- `GET /swagger` - API documentation (development only)

## Features
1. **Client Management**: Add, edit, and manage pet owners
2. **Animal Records**: Track pets with detailed information
3. **Appointment Scheduling**: Book and manage veterinary appointments
4. **Dashboard Statistics**: Real-time counts of clients, animals, and today's appointments
5. **RESTful API**: Full CRUD operations via API endpoints
6. **Swagger Documentation**: Interactive API documentation in development mode

## Development Setup

### Prerequisites
- .NET 8.0 SDK (installed via Replit)
- SQLite (included with Entity Framework Core)

### Running Locally in Replit
The application is configured to run automatically via the workflow. Simply click "Run" or restart the workflow.

The application will:
1. Restore NuGet packages
2. Build the project
3. Initialize the SQLite database (if not exists)
4. Start the web server on port 5000

### Database Initialization
The database is automatically created on first run using `EnsureCreated()`. All tables and relationships are set up based on the Entity Framework models.

## Deployment
The project is configured for Replit Autoscale deployment:
- **Build Command**: `dotnet build VitaClinic.WebAPI.csproj -c Release`
- **Run Command**: `dotnet run --project VitaClinic.WebAPI.csproj --no-build`
- **Deployment Type**: Autoscale (stateless, scales with traffic)

The SQLite database file (`vitaclinic_api.db`) will persist between deployments.

## Configuration

### CORS Settings
In the Replit environment, CORS is configured to allow all origins to support the proxy-based preview system. In production, this should be restricted to specific domains.

### Port Configuration
The application is configured to listen on `0.0.0.0:5000` which is required for Replit's port forwarding to work correctly.

### HTTPS
HTTPS redirection is disabled in the current configuration since Replit handles SSL termination at the proxy level.

## File Structure Notes
- `bin/` and `obj/` folders contain build artifacts (ignored by git)
- `vitaclinic_api.db*` files are the SQLite database (ignored by git)
- `wwwroot/` contains static files served by the application
- The database is recreated automatically if deleted

## Known Issues
None at this time.

## Future Enhancements
- Add authentication and authorization
- Implement email notifications for appointments
- Add reporting and analytics features
- Create a more advanced frontend framework (React/Vue/Angular)
- Add medical record management UI
- Implement invoice generation and payment tracking

## User Preferences
None recorded yet.

## Notes
- The database file is excluded from git to prevent conflicts
- Build artifacts are excluded from version control
- The application uses Entity Framework Core migrations (via EnsureCreated for simplicity)
- Static files are served from wwwroot directory
