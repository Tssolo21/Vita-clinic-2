using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AppointmentsView : UserControl
    {
        private readonly MainWindow _mainWindow;

        public AppointmentsView(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            LoadAppointments(null, null);
        }

        private async void LoadAppointments(object? sender, RoutedEventArgs? e)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
            var dirPath = Path.GetDirectoryName(dbPath);
            if (dirPath != null) Directory.CreateDirectory(dirPath);
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            var appointments = await context.Appointments.Include(a => a.Animal).ToListAsync();
            
            var grid = this.FindControl<DataGrid>("AppointmentsGrid");
            if (grid != null)
            {
                grid.ItemsSource = appointments;
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
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
                    var dirPath = Path.GetDirectoryName(dbPath);
                    if (dirPath != null) Directory.CreateDirectory(dirPath);
                    var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                    optionsBuilder.UseSqlite($"Data Source={dbPath}");
                    
                    using var context = new VitaClinicDbContext(optionsBuilder.Options);
                    context.Database.EnsureCreated();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;
                    context.Appointments.Add(result);
                    await context.SaveChangesAsync();
                    
                    LoadAppointments(null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding appointment: {ex.Message}");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowDashboard(sender, e);
        }
    }
}
