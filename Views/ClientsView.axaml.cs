using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Client> _clients = new ObservableCollection<Client>();

        public ClientsView()
        {
            InitializeComponent();
        }

        public ClientsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            InitializeDataGrid();
            _ = LoadClientsAsync();
        }

        private void InitializeDataGrid()
        {
            var grid = this.FindControl<DataGrid>("ClientsGrid");
            if (grid != null)
            {
                grid.ItemsSource = _clients;
            }
        }

        private async void LoadClients(object? sender, RoutedEventArgs? e)
        {
            await LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            try
            {
                Console.WriteLine("Loading clients...");
                using var context = DatabaseHelper.CreateContext();
                var clients = await context.Clients.ToListAsync();
                
                Console.WriteLine($"Found {clients.Count} clients in database");
                
                _clients.Clear();
                foreach (var client in clients)
                {
                    _clients.Add(client);
                }
                
                Console.WriteLine($"Loaded {_clients.Count} clients to UI");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading clients: {ex.Message}");
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
                    Console.WriteLine($"Adding new client: {result.FirstName} {result.LastName}");
                    
                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;
                    result.JoinDate = DateTime.UtcNow;
                    context.Clients.Add(result);
                    await context.SaveChangesAsync();
                    
                    Console.WriteLine($"Client saved with ID: {result.Id}");
                    
                    // Reload all data from database
                    await LoadClientsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding client: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
