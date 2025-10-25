using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AnimalsView : UserControl
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<Animal> _animals = new ObservableCollection<Animal>();

        public AnimalsView()
        {
            InitializeComponent();
        }

        public AnimalsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            InitializeDataGrid();
            LoadAnimals(null, null);
        }

        private void InitializeDataGrid()
        {
            var grid = this.FindControl<DataGrid>("AnimalsGrid");
            if (grid != null)
            {
                grid.ItemsSource = _animals;
            }
        }

        private async void LoadAnimals(object? sender, RoutedEventArgs? e)
        {
            try
            {
                Console.WriteLine("Loading animals...");
                using var context = DatabaseHelper.CreateContext();
                var animals = await context.Animals.Include(a => a.Client).ToListAsync();
                
                Console.WriteLine($"Found {animals.Count} animals in database");
                
                _animals.Clear();
                foreach (var animal in animals)
                {
                    _animals.Add(animal);
                }
                
                Console.WriteLine($"Loaded {_animals.Count} animals to UI");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading animals: {ex.Message}");
            }
        }

        private async void AddAnimal(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddAnimalDialog();
                var owner = Window.GetTopLevel(this) as Window;
                var result = owner != null ? await dialog.ShowDialog<Animal?>(owner) : null;
                
                if (result != null)
                {
                    Console.WriteLine($"Adding new animal: {result.Name}");
                    
                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;
                    context.Animals.Add(result);
                    await context.SaveChangesAsync();
                    
                    Console.WriteLine($"Animal saved with ID: {result.Id}");
                    
                    // Reload all data from database
                    LoadAnimals(null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding animal: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
