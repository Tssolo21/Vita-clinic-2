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

        public ObservableCollection<Client> Clients => _clients;

        public ClientsView()
        {
            InitializeComponent();
        }

        public ClientsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            this.DataContext = this;
            InitializeClientsList();
            _ = LoadClientsAsync(); // Fire and forget for async
        }

        private void InitializeClientsList()
        {
            var list = this.FindControl<ItemsControl>("ClientsList");
            if (list != null)
            {
                list.ItemsSource = _clients;
                Console.WriteLine("✓ ClientsList ItemsSource set successfully");
            }
            else
            {
                Console.WriteLine("✗ ERROR: ClientsList not found!");
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
                foreach (var client in clients)
                {
                    Console.WriteLine($"Client in DB - ID: {client.Id}, Name: {client.FirstName} {client.LastName}");
                }

                // Use Dispatcher to update collection on UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _clients.Clear();
                    foreach (var client in clients)
                    {
                        _clients.Add(client);
                        Console.WriteLine($"Added to UI collection - ID: {client.Id}, Name: {client.FirstName} {client.LastName}");
                    }
                    Console.WriteLine($"Loaded {_clients.Count} clients to UI ObservableCollection");
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

                    Console.WriteLine("Calling SaveChangesAsync for client creation...");
                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");

                    // After SaveChanges, the entity should have the auto-generated ID
                    Console.WriteLine($"Client saved with ID: {result.Id}");

                    // Verify the ID in database
                    var savedClient = await context.Clients.FindAsync(result.Id);
                    Console.WriteLine($"Verification: Saved client ID in database: {savedClient?.Id}");

                    // Check current sequence value
                    try
                    {
                        var connection = context.Database.GetDbConnection();
                        await connection.OpenAsync();
                        using var command = connection.CreateCommand();
                        command.CommandText = "SELECT seq FROM sqlite_sequence WHERE name = 'Clients'";
                        var seqResult = await command.ExecuteScalarAsync();
                        Console.WriteLine($"Current Clients sequence value after save: {seqResult ?? "NULL (no sequence entry)"}");
                        await connection.CloseAsync();
                    }
                    catch (Exception seqEx)
                    {
                        Console.WriteLine($"Could not check sequence value: {seqEx.Message}");
                    }

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

        private async void EditClient(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int clientId)
            {
                try
                {
                    Console.WriteLine($"=== EditClient START - ID: {clientId} ===");

                    // Find the client to edit
                    var clientToEdit = _clients.FirstOrDefault(c => c.Id == clientId);
                    if (clientToEdit == null)
                    {
                        Console.WriteLine($"ERROR: Client with ID {clientId} not found in current list");
                        return;
                    }

                    // Create an AddClientDialog for editing (we'll need to modify it to support editing)
                    var dialog = new AddClientDialog(clientToEdit);
                    var owner = Window.GetTopLevel(this) as Window;

                    var result = owner != null ? await dialog.ShowDialog<Client?>(owner) : null;
                    Console.WriteLine($"Dialog result: {(result != null ? $"Client data updated: {result.FirstName} {result.LastName}" : "null/cancelled")}");

                    if (result != null)
                    {
                        Console.WriteLine($"Updating client: {result.FirstName} {result.LastName}");
                        Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                        using var context = DatabaseHelper.CreateContext();
                        var existingClient = await context.Clients.FindAsync(clientId);
                        if (existingClient != null)
                        {
                            // Update the existing client properties
                            existingClient.FirstName = result.FirstName;
                            existingClient.LastName = result.LastName;
                            existingClient.Email = result.Email;
                            existingClient.Phone = result.Phone;
                            existingClient.Address = result.Address;
                            existingClient.City = result.City;
                            existingClient.State = result.State;
                            existingClient.ZipCode = result.ZipCode;
                            existingClient.Status = result.Status;
                            existingClient.UpdatedAt = DateTime.UtcNow;

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Client {clientId} updated successfully");

                            // Reload data
                            await LoadClientsAsync();
                        }
                    }

                    Console.WriteLine("=== EditClient COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in EditClient !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async void DeleteClient(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"DeleteClient called with sender: {sender?.GetType().Name}");
            if (sender is Button btn)
            {
                Console.WriteLine($"Button Tag: {btn.Tag} (Type: {btn.Tag?.GetType().Name})");
                if (btn.Tag is int clientIdValue)
                {
                    Console.WriteLine($"Extracted clientId: {clientIdValue}");
                }
                else
                {
                    Console.WriteLine("ERROR: Button Tag is not an int!");
                    return;
                }
            }
            else
            {
                Console.WriteLine("ERROR: Sender is not a Button!");
                return;
            }

            if (sender is Button button && button.Tag is int clientId)
            {
                try
                {
                    Console.WriteLine($"=== DeleteClient START - ID: {clientId} ===");

                    // Find the client to display info in confirmation
                    var clientToDelete = _clients.FirstOrDefault(c => c.Id == clientId);

                    // Create buttons
                    var cancelButton = new Button
                    {
                        Content = "Cancel",
                        Padding = new Avalonia.Thickness(20, 10),
                        Margin = new Avalonia.Thickness(0, 0, 10, 0),
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(148, 163, 184)),
                        Foreground = Avalonia.Media.Brushes.White
                    };

                    var deleteButton = new Button
                    {
                        Content = "Delete",
                        Padding = new Avalonia.Thickness(20, 10),
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(239, 68, 68)),
                        Foreground = Avalonia.Media.Brushes.White
                    };

                    // Show confirmation dialog
                    var confirmDialog = new Window
                    {
                        Title = "Confirm Deletion",
                        Content = new StackPanel
                        {
                            Margin = new Avalonia.Thickness(20),
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = clientToDelete != null
                                        ? $"Are you sure you want to delete client '{clientToDelete.FirstName} {clientToDelete.LastName}'?\n\nThis will also delete all associated animals and their records."
                                        : $"Are you sure you want to delete client #{clientId}?\n\nThis will also delete all associated animals and their records.",
                                    Margin = new Avalonia.Thickness(0, 0, 0, 20),
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                                },
                                new StackPanel
                                {
                                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                    Children = { cancelButton, deleteButton }
                                }
                            }
                        },
                        SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight,
                        WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                    };

                    var owner = Window.GetTopLevel(this) as Window;

                    // Set up event handlers for the buttons
                    bool? confirmResult = null;
                    cancelButton.Click += (s, e) => {
                        confirmResult = false;
                        confirmDialog.Close();
                    };
                    deleteButton.Click += (s, e) => {
                        confirmResult = true;
                        confirmDialog.Close();
                    };

                    await confirmDialog.ShowDialog(owner);

                    Console.WriteLine($"Confirm result: {confirmResult}");
                    if (confirmResult == true)
                    {
                        Console.WriteLine($"Starting database deletion for client {clientId}");
                        // Delete the client and all related records
                        using var context = DatabaseHelper.CreateContext();
                        var clientToRemove = await context.Clients
                            .Include(c => c.Animals)
                            .ThenInclude(a => a.MedicalRecords)
                            .Include(c => c.Animals)
                            .ThenInclude(a => a.Appointments)
                            .FirstOrDefaultAsync(c => c.Id == clientId);

                        Console.WriteLine($"Client to remove: {(clientToRemove != null ? $"{clientToRemove.FirstName} {clientToRemove.LastName}" : "null")}");

                        if (clientToRemove != null)
                        {
                            Console.WriteLine($"Removing client {clientId} from context");
                            context.Clients.Remove(clientToRemove);

                            Console.WriteLine("Calling SaveChangesAsync for deletion...");
                            var deleteChanges = await context.SaveChangesAsync();
                            Console.WriteLine($"SaveChangesAsync returned {deleteChanges} changes for deletion");

                            if (deleteChanges > 0)
                            {
                                Console.WriteLine($"Client {clientId} and all related records deleted successfully");

                                // Check if clients table is now empty and reset sequence
                                var remainingClients = await context.Clients.CountAsync();
                                Console.WriteLine($"Remaining clients count: {remainingClients}");

                                if (remainingClients == 0)
                                {
                                    // Reset the auto-increment sequence for Clients table
                                    try
                                    {
                                        await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name = 'Clients'");
                                        Console.WriteLine("Auto-increment sequence for Clients table reset to start from 1");
                                    }
                                    catch (Exception resetEx)
                                    {
                                        Console.WriteLine($"Warning: Could not reset auto-increment sequence: {resetEx.Message}");
                                    }
                                }

                                // Reload data
                                Console.WriteLine("Reloading client data...");
                                await LoadClientsAsync();
                            }
                            else
                            {
                                Console.WriteLine("WARNING: SaveChangesAsync returned 0 changes for deletion!");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Client {clientId} not found in database");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User cancelled deletion");
                    }

                    Console.WriteLine("=== DeleteClient COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in DeleteClient !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
