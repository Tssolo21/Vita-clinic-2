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
            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        private async void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            InitializeDataGrid();
            await LoadClientsAsync();
        }

        private void InitializeDataGrid()
        {
            var grid = this.FindControl<DataGrid>("ClientsGrid");
            if (grid != null)
            {
                grid.ItemsSource = _clients;
                Console.WriteLine("✓ ClientsGrid ItemsSource set successfully");
            }
            else
            {
                Console.WriteLine("✗ ERROR: ClientsGrid not found!");
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
                
                // Use Dispatcher to update collection on UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _clients.Clear();
                    foreach (var client in clients)
                    {
                        _clients.Add(client);
                    }
                    Console.WriteLine($"Loaded {_clients.Count} clients to UI ObservableCollection");
                    
                    // Force grid to refresh
                    var grid = this.FindControl<DataGrid>("ClientsGrid");
                    if (grid != null)
                    {
                        var temp = grid.ItemsSource;
                        grid.ItemsSource = null;
                        grid.ItemsSource = temp;
                        Console.WriteLine("DataGrid ItemsSource refreshed");
                    }
                });
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
                Console.WriteLine("=== AddClient START ===");
                var dialog = new AddClientDialog();
                var owner = Window.GetTopLevel(this) as Window;
                Console.WriteLine($"Owner window: {owner?.GetType().Name ?? "null"}");
                
                var result = owner != null ? await dialog.ShowDialog<Client?>(owner) : null;
                Console.WriteLine($"Dialog result: {(result != null ? "Client data received" : "null/cancelled")}");
                
                if (result != null)
                {
                    Console.WriteLine($"Adding new client: {result.FirstName} {result.LastName}");
                    Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");
                    
                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;
                    result.JoinDate = DateTime.UtcNow;
                    
                    context.Clients.Add(result);
                    Console.WriteLine("Client added to context");
                    
                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");
                    Console.WriteLine($"Client saved with ID: {result.Id}");
                    
                    // Verify it was saved
                    var verify = await context.Clients.FindAsync(result.Id);
                    Console.WriteLine($"Verification: Client {result.Id} {(verify != null ? "EXISTS" : "NOT FOUND")} in database");
                    
                    // Reload all data from database
                    Console.WriteLine("Reloading clients from database...");
                    await LoadClientsAsync();
                    Console.WriteLine("=== AddClient COMPLETE ===");
                }
                else
                {
                    Console.WriteLine("=== AddClient CANCELLED ===");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! ERROR in AddClient !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
