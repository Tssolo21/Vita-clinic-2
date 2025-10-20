# VitaClinic - Veterinary Management System

## Overview
VitaClinic is a comprehensive veterinary clinic management system built with ASP.NET Core 8.0 and SQLite. It provides a complete solution for managing clients, animals, appointments, medical records, invoicing, analytics, and clinic settings with email/SMS notifications for veterinary practices.

## Project Information
- **Technology Stack**: ASP.NET Core 8.0 Web API with Entity Framework Core
- **Database**: SQLite
- **Frontend**: Modern single-page application with HTML/CSS/JavaScript
- **Port**: 5000 (HTTP only in Replit environment)
- **Notifications**: SendGrid (Email) and Twilio (SMS) integration support

## Recent Changes
- **2025-10-20**: Major feature enhancements
  - Created comprehensive multi-page UI with sidebar navigation
  - Added ClinicSettings model and SettingsController for clinic configuration
  - Implemented EmailService and SmsService for appointment notifications
  - Built AnalyticsController with dashboard statistics and data visualization
  - Created MedicalRecordsController for complete medical history management
  - Enhanced AppointmentsController to send email/SMS notifications
  - Fixed SQLite decimal aggregation issues in analytics
  - Created modern responsive UI with Dashboard, Clients, Animals, Appointments, Medical Records, Analytics, and Settings pages
  - Integrated notification settings with toggle switches
  
- **2025-10-20**: Initial Replit setup
  - Upgraded from .NET 6.0 to .NET 8.0
  - Configured application to run on 0.0.0.0:5000 for Replit environment
  - Disabled HTTPS redirection for Replit compatibility
  - Updated CORS policy to allow all origins (required for Replit proxy)
  - Created fresh SQLite database with all tables
  - Configured deployment settings for autoscale
  - Added comprehensive .gitignore for .NET projects

## Project Architecture

### Backend Structure
```
├── Controllers/
│   ├── AnimalsController.cs        # Animal management endpoints
│   ├── AppointmentsController.cs   # Appointment scheduling with notifications
│   ├── ClientsController.cs        # Client management endpoints
│   ├── MedicalRecordsController.cs # Medical records CRUD operations
│   ├── AnalyticsController.cs      # Dashboard statistics and analytics
│   ├── SettingsController.cs       # Clinic settings management
│   └── WeatherForecastController.cs
├── Services/
│   ├── EmailService.cs             # SendGrid email integration
│   └── SmsService.cs               # Twilio SMS integration
├── Data/
│   └── VitaClinicDbContext.cs      # Entity Framework DbContext
├── Models/
│   └── Models.cs                   # Data models for all entities
├── wwwroot/
│   ├── index.html                  # Main UI with all pages
│   └── js/
│       └── app.js                  # JavaScript application logic
├── Program.cs                      # Application entry point
└── VitaClinic.WebAPI.csproj        # Project file
```

### Database Schema
The application uses SQLite with the following tables:
- **Clients**: Pet owners with contact information
- **Animals**: Pets with species, breed, weight, vaccination records
- **Appointments**: Scheduled visits with appointment types and status
- **Veterinarians**: Staff members with specialization info
- **MedicalRecords**: Diagnosis, treatment, medication, and next checkup dates
- **Invoices**: Billing information with payment tracking
- **InvoiceItems**: Line items for invoices
- **ClinicSettings**: Clinic configuration and notification preferences

### API Endpoints

#### Clients
- `GET /api/clients` - List all clients
- `POST /api/clients` - Create new client
- `GET /api/clients/{id}` - Get client by ID
- `PUT /api/clients/{id}` - Update client
- `DELETE /api/clients/{id}` - Delete client

#### Animals
- `GET /api/animals` - List all animals
- `POST /api/animals` - Add new animal
- `GET /api/animals/{id}` - Get animal by ID
- `PUT /api/animals/{id}` - Update animal
- `DELETE /api/animals/{id}` - Delete animal

#### Appointments
- `GET /api/appointments` - List all appointments
- `POST /api/appointments` - Schedule appointment (sends notifications)
- `GET /api/appointments/{id}` - Get appointment by ID
- `PUT /api/appointments/{id}` - Update appointment (sends notifications)
- `DELETE /api/appointments/{id}` - Cancel appointment

#### Medical Records
- `GET /api/medicalrecords` - List all medical records
- `POST /api/medicalrecords` - Create medical record
- `GET /api/medicalrecords/{id}` - Get medical record by ID
- `GET /api/medicalrecords/animal/{animalId}` - Get records for specific animal
- `PUT /api/medicalrecords/{id}` - Update medical record
- `DELETE /api/medicalrecords/{id}` - Delete medical record

#### Analytics
- `GET /api/analytics/dashboard` - Dashboard statistics
- `GET /api/analytics/appointments-by-type` - Appointment distribution by type
- `GET /api/analytics/appointments-by-status` - Appointment distribution by status
- `GET /api/analytics/species-distribution` - Pet species breakdown
- `GET /api/analytics/appointments-by-month` - Monthly appointment trends
- `GET /api/analytics/revenue-by-month` - Monthly revenue trends

#### Settings
- `GET /api/settings` - Get clinic settings
- `PUT /api/settings` - Update clinic settings

#### Documentation
- `GET /swagger` - Interactive API documentation (development only)

## Features

### Core Functionality
1. **Client Management**: Add, edit, delete, and view pet owners with contact information
2. **Animal Records**: Track pets with detailed information (species, breed, weight, etc.)
3. **Appointment Scheduling**: Book and manage veterinary appointments with multiple types
4. **Medical Records**: Complete medical history tracking with diagnosis, treatment, medication, and follow-ups
5. **Analytics Dashboard**: Real-time statistics, charts, and data visualization
6. **Clinic Settings**: Configure clinic information and notification preferences
7. **RESTful API**: Full CRUD operations via clean API endpoints
8. **Swagger Documentation**: Interactive API documentation in development mode

### Notification System
1. **Email Notifications** (via SendGrid):
   - Appointment confirmations sent to clients
   - Appointment reminders based on configurable hours
   - Requires `SENDGRID_API_KEY` secret to be configured

2. **SMS Notifications** (via Twilio):
   - SMS reminders for upcoming appointments
   - Configurable reminder timing (hours before appointment)
   - Requires `TWILIO_ACCOUNT_SID`, `TWILIO_AUTH_TOKEN`, and `TWILIO_PHONE_NUMBER` secrets

### Analytics Features
- Total client and animal counts
- Today's and weekly appointment statistics
- Monthly appointment tracking
- Appointment distribution by type (Checkup, Vaccination, Surgery, etc.)
- Appointment status breakdown
- Species distribution charts
- Revenue tracking (total and pending)
- Monthly revenue trends

### User Interface
- **Modern Sidebar Navigation**: Easy access to all sections
- **Dashboard**: Overview with key statistics and today's appointments
- **Clients Page**: Table view with add/delete functionality
- **Animals Page**: Pet management with owner information
- **Appointments Page**: Comprehensive appointment calendar and management
- **Medical Records Page**: Complete medical history tracking
- **Analytics Page**: Visual charts and statistics
- **Settings Page**: Clinic configuration with notification toggles
- **Responsive Design**: Clean, professional layout with purple gradient theme

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
5. Seed default clinic settings if none exist

### Configuring Notifications
To enable email and SMS notifications:

1. **For Email (SendGrid)**:
   - Add `SENDGRID_API_KEY` secret with your SendGrid API key
   - Enable email notifications in Settings page
   - Configure sender email in clinic settings

2. **For SMS (Twilio)**:
   - Add `TWILIO_ACCOUNT_SID` secret
   - Add `TWILIO_AUTH_TOKEN` secret
   - Add `TWILIO_PHONE_NUMBER` secret (format: +1234567890)
   - Enable SMS notifications in Settings page

### Database Initialization
The database is automatically created on first run using `EnsureCreated()`. All tables and relationships are set up based on the Entity Framework models. Default clinic settings are seeded automatically.

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

### Notification Services
Email and SMS services are configured to use SendGrid and Twilio respectively. Services gracefully handle missing API keys and log warnings when credentials are not configured.

## File Structure Notes
- `bin/` and `obj/` folders contain build artifacts (ignored by git)
- `vitaclinic_api.db*` files are the SQLite database (ignored by git)
- `wwwroot/` contains static files served by the application
- `wwwroot/js/` contains JavaScript application logic
- The database is recreated automatically if deleted
- Services are registered in Program.cs for dependency injection

## Known Issues
None at this time.

## Future Enhancements
- Add authentication and authorization for users
- Implement role-based access control (Admin, Veterinarian, Receptionist)
- Add invoice generation and payment processing
- Create appointment calendar view with drag-and-drop
- Add file upload for animal photos and medical documents
- Implement real-time notifications with SignalR
- Add export functionality (PDF reports, CSV exports)
- Create mobile-responsive improvements
- Add multi-language support
- Implement appointment recurring schedules

## User Preferences
None recorded yet.

## Notes
- The database file is excluded from git to prevent conflicts
- Build artifacts are excluded from version control
- The application uses Entity Framework Core with EnsureCreated for simplicity
- Static files are served from wwwroot directory
- JavaScript uses vanilla JS without frameworks for simplicity
- Notification services use constructor-based dependency injection
- Analytics endpoints handle SQLite decimal limitations by converting to LINQ to Objects
- All timestamps are stored in UTC format
