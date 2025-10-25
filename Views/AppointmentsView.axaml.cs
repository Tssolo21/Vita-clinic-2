using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AppointmentsView : UserControl
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<Appointment> _appointments = new ObservableCollection<Appointment>();

        public AppointmentsView()
        {
            InitializeComponent();
        }

        public AppointmentsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            InitializeDataGrid();
            LoadAppointments(null, null);
        }

        private void InitializeDataGrid()
        {
            var grid = this.FindControl<DataGrid>("AppointmentsGrid");
            if (grid != null)
            {
                grid.ItemsSource = _appointments;
            }
        }

        private async void LoadAppointments(object? sender, RoutedEventArgs? e)
        {
            try
            {
                Console.WriteLine("Loading appointments...");
                using var context = DatabaseHelper.CreateContext();
                var appointments = await context.Appointments.Include(a => a.Animal).ToListAsync();
                
                Console.WriteLine($"Found {appointments.Count} appointments in database");
                
                _appointments.Clear();
                foreach (var appointment in appointments)
                {
                    _appointments.Add(appointment);
                }
                
                Console.WriteLine($"Loaded {_appointments.Count} appointments to UI");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
            }
        }

        private async void AddAppointment(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddAppointmentDialog();
                var owner = Window.GetTopLevel(this) as Window;
                var result = owner != null ? await dialog.ShowDialog<Appointment?>(owner) : null;
                
                if (result != null)
                {
                    Console.WriteLine($"Adding new appointment for: {result.PetName}");
                    
                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;
                    context.Appointments.Add(result);
                    await context.SaveChangesAsync();
                    
                    Console.WriteLine($"Appointment saved with ID: {result.Id}");
                    
                    // Reload all data from database
                    LoadAppointments(null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding appointment: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
