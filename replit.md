# VitaClinic - Desktop Veterinary Management System

## Overview
VitaClinic is a cross-platform desktop veterinary clinic management system built with Avalonia UI and .NET 8.0. It provides a secure login system and complete solution for managing clients, animals, appointments, and medical records for veterinary practices.

## Project Information
- **Technology Stack**: .NET 8.0 Desktop App with Avalonia UI Framework
- **Database**: SQLite (vitaclinic_desktop.db)
- **Platform Support**: Windows, macOS, and Linux
- **Authentication**: Secure login with SHA-256 password hashing
- **UI Framework**: Avalonia 11.3.8 with Fluent theme

## Recent Changes
- **2025-10-26**: Fixed Data Display Issue in UI
  - **Problem**: Data was saving to database but not displaying in DataGrids
  - **Root Cause**: UI threading and timing issues with DataGrid initialization
  - **Solution**: 
    - Implemented AttachedToVisualTree event to ensure controls are fully loaded
    - Use Dispatcher.UIThread for all ObservableCollection updates
    - Force DataGrid refresh by resetting ItemsSource after updates
    - Added comprehensive debug logging
  - **Result**: Data now displays immediately after being added, refresh works correctly
  - All three views fixed: ClientsView, AnimalsView, AppointmentsView

- **2025-10-25**: Enhanced Settings Section and Fixed Data Display Issues
  - Created comprehensive SettingsView with rich features:
    - View user profile information (full name, username, email, role, last login)
    - Update full name with real-time database sync
    - Change username with uniqueness validation
    - Update email address with format validation
    - Secure password change dialog with validation
  - Added ChangePasswordDialog with comprehensive validation:
    - Current password verification
    - Minimum length requirement (6 characters)
    - Password confirmation matching
    - Prevention of reusing current password
  - Fixed public constructor warnings in all View classes (ClientsView, AnimalsView, AppointmentsView)
  - All data operations now properly save to and load from SQLite database
  - Status messages provide user feedback for all update operations

- **2025-10-24**: Converted to Desktop Application with Login System
  - Migrated from ASP.NET Core Web API to Avalonia desktop app
  - Added User model with authentication system (Admin, Veterinarian, Receptionist roles)
  - Created AuthService with SHA-256 password hashing
  - Built LoginWindow with username/password authentication
  - Created MainWindow with sidebar navigation and dashboard
  - Removed web-specific dependencies (Controllers, Web services)
  - Updated database schema to include Users table
  - Default admin credentials: admin / admin123

## Project Architecture

### Desktop Application Structure
```
├── App.axaml                      # Main application definition
├── App.axaml.cs                   # Application code-behind
├── DesktopProgram.cs              # Application entry point
├── LoginWindow.axaml              # Login UI
├── LoginWindow.axaml.cs           # Login logic
├── MainWindow.axaml               # Main application UI
├── MainWindow.axaml.cs            # Main window logic
├── Views/
│   ├── ClientsView.axaml          # Clients management view
│   ├── ClientsView.axaml.cs       # Clients view code-behind
│   ├── AnimalsView.axaml          # Animals management view
│   ├── AnimalsView.axaml.cs       # Animals view code-behind
│   ├── AppointmentsView.axaml     # Appointments view
│   ├── AppointmentsView.axaml.cs  # Appointments view code-behind
│   ├── SettingsView.axaml         # Settings management view
│   ├── SettingsView.axaml.cs      # Settings view code-behind
│   ├── AddClientDialog.axaml      # Add client dialog
│   ├── AddClientDialog.axaml.cs   # Add client dialog code-behind
│   ├── AddAnimalDialog.axaml      # Add animal dialog
│   ├── AddAnimalDialog.axaml.cs   # Add animal dialog code-behind
│   ├── AddAppointmentDialog.axaml # Add appointment dialog
│   ├── AddAppointmentDialog.axaml.cs # Add appointment code-behind
│   ├── ChangePasswordDialog.axaml # Change password dialog
│   └── ChangePasswordDialog.axaml.cs # Change password code-behind
├── Services/
│   └── AuthService.cs             # Authentication service
├── Data/
│   └── VitaClinicDbContext.cs     # Entity Framework DbContext
├── Models/
│   └── Models.cs                  # Data models for all entities
└── VitaClinic.WebAPI.csproj       # Project file
```

### Database Schema
The application uses SQLite with the following tables:
- **Users**: System users with roles (Admin, Veterinarian, Receptionist)
- **Clients**: Pet owners with contact information
- **Animals**: Pets with species, breed, weight, vaccination records
- **Appointments**: Scheduled visits with appointment types and status
- **Veterinarians**: Staff members with specialization info
- **MedicalRecords**: Diagnosis, treatment, medication, and next checkup dates
- **Invoices**: Billing information with payment tracking
- **InvoiceItems**: Line items for invoices
- **ClinicSettings**: Clinic configuration and notification preferences

## Features

### Authentication System
1. **Secure Login**: SHA-256 password hashing
2. **User Roles**: Admin, Veterinarian, and Receptionist
3. **Session Management**: Tracks last login timestamps
4. **Default Admin**: Automatically creates admin user (username: admin, password: admin123)

### Desktop Application Features
1. **Modern UI**: Clean, professional interface with Fluent theme
2. **Sidebar Navigation**: Easy access to all sections
3. **Dashboard**: Real-time statistics showing:
   - Total clients count
   - Total animals count
   - Today's appointments
   - This week's appointments
4. **Multi-section Management**: Clients, Animals, Appointments, Medical Records, Settings
5. **Cross-platform**: Works on Windows, macOS, and Linux
6. **Responsive Design**: Purple gradient theme with modern aesthetics

### User Roles
- **Admin**: Full system access
- **Veterinarian**: Medical records and appointments
- **Receptionist**: Client and appointment management

## Running the Application

### Prerequisites
- .NET 8.0 SDK installed
- SQLite (included with Entity Framework Core)

### To Run Locally
1. Clone the repository
2. Navigate to the project directory
3. Run: `dotnet run --project VitaClinic.WebAPI.csproj`
4. The login window will appear
5. Use default credentials: **admin / admin123**

### First-Time Setup
The application automatically:
1. Creates the SQLite database (vitaclinic_desktop.db)
2. Initializes all tables
3. Creates a default admin user if none exists

### Building for Distribution
```bash
# For your current platform
dotnet publish -c Release

# For Windows
dotnet publish -c Release -r win-x64 --self-contained

# For macOS
dotnet publish -c Release -r osx-x64 --self-contained

# For Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

## Development Notes

### Adding New Users
Users can be added programmatically through the AuthService or directly via database:
```csharp
var user = new User
{
    Username = "newuser",
    PasswordHash = authService.HashPassword("password123"),
    FullName = "New User Name",
    Email = "user@vitaclinic.com",
    Role = UserRole.Receptionist,
    CreatedAt = DateTime.UtcNow,
    IsActive = true
};
context.Users.Add(user);
await context.SaveChangesAsync();
```

### Password Security
- Passwords are hashed using SHA-256
- Never store plain-text passwords
- Password hashes are stored in the Users table

### Navigation
The main window includes navigation to:
- Dashboard (overview with statistics)
- Clients Management
- Animals Management
- Appointments Scheduling
- Medical Records
- Settings Configuration

## Known Limitations

### Replit Environment
- Desktop GUI applications require a display server (X11/Wayland)
- Replit's headless environment doesn't fully support GUI applications
- The app compiles successfully but may not run in Replit's VNC viewer without additional system dependencies
- **Recommended**: Download and run locally on Windows, macOS, or Linux

### Current Implementation
- ✅ **Dashboard**: Real-time statistics from database
- ✅ **Clients Management**: View all clients, add new clients with DataGrid display
- ✅ **Animals Management**: View all animals with owner info, add new animals
- ✅ **Appointments**: View all appointments, schedule new appointments
- ✅ **Medical Records**: Information view (link to animals for records)
- ✅ **Settings**: Comprehensive user account management
  - View and update full name
  - Change username (with uniqueness validation)
  - Update email address (with format validation)
  - Secure password change with validation
  - Visual feedback with status messages
  - All changes persist to database immediately

## Future Enhancements
- Implement full CRUD operations for all management sections
- Add user management UI for admins
- Implement role-based access control in the UI
- Add invoice generation and payment processing
- Create appointment calendar view with drag-and-drop
- Add file upload for animal photos and medical documents
- Implement data export functionality (PDF reports, CSV exports)
- Add print support for invoices and medical records
- Create data backup and restore features
- Add multi-language support
- Implement search and filter capabilities
- Add reporting and analytics dashboards

## Security Best Practices
- Never commit the database file to version control
- Change default admin password immediately after first login
- Use strong passwords for all user accounts
- Regularly backup the database file
- Keep the application and dependencies updated

## Database Location
- Development: `vitaclinic_desktop.db` in the application directory
- Database file should be backed up regularly
- Database is excluded from git via .gitignore

## User Preferences
None recorded yet.

## Notes
- The application is fully cross-platform using Avalonia UI
- SQLite database file is portable across all platforms
- No web server required - runs as a standalone desktop application
- Authentication is local (no network authentication)
- All timestamps are stored in UTC format
