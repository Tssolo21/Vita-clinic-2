using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class ClientsView : UserControl
    {
        private MainWindow? _mainWindow;

        public ClientsView()
        {
            InitializeComponent();
        }

        public ClientsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            LoadClients(null, null);
        }

        private async void LoadClients(object? sender, RoutedEventArgs? e)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
            var dirPath = Path.GetDirectoryName(dbPath);
            if (dirPath != null) Directory.CreateDirectory(dirPath);
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            var clients = await context.Clients.ToListAsync();
            
            var grid = this.FindControl<DataGrid>("ClientsGrid");
            if (grid != null)
            {
                grid.ItemsSource = clients;
            }
        }

        private async void AddClient(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddClientDialog();
                var owner = Window.GetTopLevel(this) as Window;
                var result = owner != null ? await dialog.ShowDialog<Client?>(owner) : null;
                
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
                    result.JoinDate = DateTime.UtcNow;
                    context.Clients.Add(result);
                    await context.SaveChangesAsync();
                    
                    LoadClients(null, null);
                }
            }
            catch (Exception ex)
            {
                // Log the error or show a message
                Console.WriteLine($"Error adding client: {ex.Message}");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
