using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly MainWindow _mainWindow;

        public ClientsView(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            LoadClients(null, null);
        }

        private async void LoadClients(object? sender, RoutedEventArgs? e)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            var clients = await context.Clients.ToListAsync();
            
            var grid = this.FindControl<DataGrid>("ClientsGrid");
            if (grid != null)
            {
                grid.ItemsSource = clients;
            }
        }

        private async void AddClient(object sender, RoutedEventArgs e)
        {
            var dialog = new AddClientDialog();
            var owner = Window.GetTopLevel(this) as Window;
            var result = owner != null ? await dialog.ShowDialog<Client?>(owner) : null;
            
            if (result != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
                
                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                result.CreatedAt = DateTime.UtcNow;
                result.UpdatedAt = DateTime.UtcNow;
                result.JoinDate = DateTime.UtcNow;
                context.Clients.Add(result);
                await context.SaveChangesAsync();
                
                LoadClients(null, null);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowDashboard(sender, e);
        }
    }
}
