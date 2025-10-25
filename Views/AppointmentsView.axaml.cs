using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
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
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            var appointments = await context.Appointments.Include(a => a.Animal).ToListAsync();
            
            var grid = this.FindControl<DataGrid>("AppointmentsGrid");
            if (grid != null)
            {
                grid.ItemsSource = appointments;
            }
        }

        private async void AddAppointment(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAppointmentDialog();
            var owner = Window.GetTopLevel(this) as Window;
            var result = owner != null ? await dialog.ShowDialog<Appointment?>(owner) : null;
            
            if (result != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
                
                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                result.CreatedAt = DateTime.UtcNow;
                result.UpdatedAt = DateTime.UtcNow;
                context.Appointments.Add(result);
                await context.SaveChangesAsync();
                
                LoadAppointments(null, null);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowDashboard(sender, e);
        }
    }
}
