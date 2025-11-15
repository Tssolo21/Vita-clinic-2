using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class ClientsAndAnimalsView : UserControl
    {
        private readonly Window _parentWindow;

        public ObservableCollection<Client> Clients { get; set; } = new();
        public ObservableCollection<Animal> Animals { get; set; } = new();
        public ObservableCollection<CombinedRecord> CombinedRecords { get; set; } = new();

        public class CombinedRecord
        {
            public string? Icon { get; set; }
            public string? Title { get; set; }
            public string? SubTitle { get; set; }
            public string? Details { get; set; }
            public string? Type { get; set; } // "Client" or "Animal"
        }

        public ClientsAndAnimalsView(Window parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            DataContext = this;
            LoadData();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadData()
        {
            try
            {
                using var context = DatabaseHelper.CreateContext();

                // Load clients
                var clients = context.Clients.ToList();
                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }

                // Load animals with client information
                var animals = context.Animals.Include(a => a.Client).ToList();
                Animals.Clear();
                foreach (var animal in animals)
                {
                    Animals.Add(animal);
                }

                // Create combined records
                CombinedRecords.Clear();

                // Add clients to combined view
                foreach (var client in clients)
                {
                    CombinedRecords.Add(new CombinedRecord
                    {
                        Icon = "👤",
                        Title = $"{client.FirstName} {client.LastName}",
                        SubTitle = client.Email,
                        Details = $"Phone: {client.Phone} | Pets: {client.Animals?.Count ?? 0}",
                        Type = "Client"
                    });
                }

                // Add animals to combined view
                foreach (var animal in animals)
                {
                    CombinedRecords.Add(new CombinedRecord
                    {
                        Icon = "🐾",
                        Title = animal.Name,
                        SubTitle = $"{animal.Species} - {animal.Breed}",
                        Details = $"Owner: {animal.Client?.FirstName} {animal.Client?.LastName} | Weight: {animal.Weight}kg",
                        Type = "Animal"
                    });
                }

                UpdateResultsCount();
            }
            catch (System.Exception ex)
            {
                // Handle error gracefully
                System.Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void UpdateResultsCount()
        {
            var resultsCount = this.FindControl<TextBlock>("ResultsCount");
            if (resultsCount != null)
            {
                if (ClientsPanel?.IsVisible == true)
                {
                    resultsCount.Text = $"Showing {Clients.Count} clients";
                }
                else if (AnimalsPanel?.IsVisible == true)
                {
                    resultsCount.Text = $"Showing {Animals.Count} animals";
                }
                else
                {
                    resultsCount.Text = $"Showing {CombinedRecords.Count} combined records";
                }
            }
        }

        private void ShowClients(object sender, RoutedEventArgs e)
        {
            ClientsPanel.IsVisible = true;
            AnimalsPanel.IsVisible = false;
            CombinedPanel.IsVisible = false;
            UpdateResultsCount();
        }

        private void ShowAnimals(object sender, RoutedEventArgs e)
        {
            ClientsPanel.IsVisible = false;
            AnimalsPanel.IsVisible = true;
            CombinedPanel.IsVisible = false;
            UpdateResultsCount();
        }

        private void ShowCombined(object sender, RoutedEventArgs e)
        {
            ClientsPanel.IsVisible = false;
            AnimalsPanel.IsVisible = false;
            CombinedPanel.IsVisible = true;
            UpdateResultsCount();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (_parentWindow is MainWindow mainWindow)
            {
                mainWindow.ShowDashboard(sender, e);
            }
        }

        private void AddClient(object sender, RoutedEventArgs e)
        {
            var dialog = new AddClientDialog();
            dialog.ShowDialog(_parentWindow);
            dialog.Closed += (s, args) => LoadData();
        }

        private void AddAnimal(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAnimalDialog();
            dialog.ShowDialog(_parentWindow);
            dialog.Closed += (s, args) => LoadData();
        }

        private void EditClient(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string clientIdStr && int.TryParse(clientIdStr, out int clientId))
            {
                using var context = DatabaseHelper.CreateContext();
                var client = context.Clients.Find(clientId);
                if (client != null)
                {
                    var dialog = new EditClientDialog(client);
                    dialog.ShowDialog(_parentWindow);
                    dialog.Closed += (s, args) => LoadData();
                }
            }
        }

        private void EditAnimal(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string animalId)
            {
                using var context = DatabaseHelper.CreateContext();
                var animal = context.Animals.Include(a => a.Client).FirstOrDefault(a => a.Id == animalId);
                if (animal != null)
                {
                    var dialog = new EditAnimalDialog(animal);
                    dialog.ShowDialog(_parentWindow);
                    dialog.Closed += (s, args) => LoadData();
                }
            }
        }

        private void DeleteClient(object sender, RoutedEventArgs e)
        {
            // Simple confirmation dialog using Avalonia's built-in
            var dialog = new Window
            {
                Title = "Confirm Delete",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = "Are you sure you want to delete this client?", Margin = new Thickness(20) },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(20),
                            Children =
                            {
                                new Button { Content = "Yes", Margin = new Thickness(0,0,10,0), Tag = "Yes" },
                                new Button { Content = "No", Tag = "No" }
                            }
                        }
                    }
                },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var button = sender as Button;
            if (button?.Tag is string clientIdStr && int.TryParse(clientIdStr, out int clientId))
            {
                dialog.FindControl<Button>("Yes").Click += async (s, args) =>
                {
                    dialog.Close();
                    try
                    {
                        using var context = DatabaseHelper.CreateContext();
                        var client = context.Clients.Find(clientId);
                        if (client != null)
                        {
                            context.Clients.Remove(client);
                            await context.SaveChangesAsync();
                            LoadData();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        var errorDialog = new Window
                        {
                            Title = "Error",
                            Content = new TextBlock { Text = $"Error deleting client: {ex.Message}", Margin = new Thickness(20) },
                            SizeToContent = SizeToContent.WidthAndHeight,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        await errorDialog.ShowDialog(_parentWindow);
                    }
                };

                dialog.FindControl<Button>("No").Click += (s, args) => dialog.Close();
                dialog.ShowDialog(_parentWindow);
            }
        }

        private void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            // Simple confirmation dialog using Avalonia's built-in
            var dialog = new Window
            {
                Title = "Confirm Delete",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = "Are you sure you want to delete this animal?", Margin = new Thickness(20) },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Margin = new Thickness(20),
                            Children =
                            {
                                new Button { Content = "Yes", Margin = new Thickness(0,0,10,0), Tag = "Yes" },
                                new Button { Content = "No", Tag = "No" }
                            }
                        }
                    }
                },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var button = sender as Button;
            if (button?.Tag is string animalIdStr && int.TryParse(animalIdStr, out int animalId))
            {
                dialog.FindControl<Button>("Yes").Click += async (s, args) =>
                {
                    dialog.Close();
                    try
                    {
                        using var context = DatabaseHelper.CreateContext();
                        var animal = context.Animals.Find(animalId);
                        if (animal != null)
                        {
                            context.Animals.Remove(animal);
                            await context.SaveChangesAsync();
                            LoadData();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        var errorDialog = new Window
                        {
                            Title = "Error",
                            Content = new TextBlock { Text = $"Error deleting animal: {ex.Message}", Margin = new Thickness(20) },
                            SizeToContent = SizeToContent.WidthAndHeight,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        await errorDialog.ShowDialog(_parentWindow);
                    }
                };

                dialog.FindControl<Button>("No").Click += (s, args) => dialog.Close();
                dialog.ShowDialog(_parentWindow);
            }
        }
    }
}