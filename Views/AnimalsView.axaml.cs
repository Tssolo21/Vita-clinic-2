using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using VitaClinic.WebAPI.Views;

namespace VitaClinic.WebAPI.Views
{
    public partial class AnimalsView : UserControl, INotifyPropertyChanged
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<Animal> _animals = new ObservableCollection<Animal>();

        public ObservableCollection<Animal> Animals => _animals;

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AnimalsView()
        {
            InitializeComponent();
        }

        public AnimalsView(MainWindow mainWindow) : this()
        {
            Console.WriteLine("=== AnimalsView Constructor START ===");
            _mainWindow = mainWindow;
            this.DataContext = this;
            this.AttachedToVisualTree += OnAttachedToVisualTree;
            Console.WriteLine("=== AnimalsView Constructor COMPLETE ===");
        }

        private async void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            Console.WriteLine("=== OnAttachedToVisualTree START ===");
            try
            {
                InitializeAnimalsList();
                await LoadAnimalsAsync();
                Console.WriteLine("=== OnAttachedToVisualTree COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! CRASH in OnAttachedToVisualTree !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void InitializeAnimalsList()
        {
            Console.WriteLine("=== InitializeAnimalsList START ===");
            try
            {
                var list = this.FindControl<ItemsControl>("AnimalsList");
                if (list != null)
                {
                    list.ItemsSource = _animals;
                    Console.WriteLine("✓ AnimalsList ItemsSource set successfully");
                }
                else
                {
                    Console.WriteLine("✗ ERROR: AnimalsList not found!");
                }
                Console.WriteLine("=== InitializeAnimalsList COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! CRASH in InitializeAnimalsList !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async void LoadAnimals(object? sender, RoutedEventArgs? e)
        {
            await LoadAnimalsAsync();
        }

        private async Task LoadAnimalsAsync()
        {
            try
            {
                Console.WriteLine("Loading animals...");
                using var context = DatabaseHelper.CreateContext();
                var animals = await context.Animals.Include(a => a.Client).ToListAsync();
                
                Console.WriteLine($"Found {animals.Count} animals in database");
                
                // Use Dispatcher to update collection on UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _animals.Clear();
                    foreach (var animal in animals)
                    {
                        _animals.Add(animal);
                    }
                    Console.WriteLine($"Loaded {_animals.Count} animals to UI ObservableCollection");

                    // Ensure ItemsControl is updated
                    var list = this.FindControl<ItemsControl>("AnimalsList");
                    if (list != null)
                    {
                        list.ItemsSource = null;
                        list.ItemsSource = _animals;
                        Console.WriteLine("ItemsControl ItemsSource refreshed after loading");
                    }
                    else
                    {
                        Console.WriteLine("✗ ERROR: AnimalsList not found during reload!");
                    }
                });
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
                Console.WriteLine("=== AddAnimal START ===");
                var dialog = new AddAnimalDialog();
                var owner = Window.GetTopLevel(this) as Window;
                Console.WriteLine($"Owner window: {owner?.GetType().Name ?? "null"}");

                var result = owner != null ? await dialog.ShowDialog<AddAnimalDialog.AnimalWithMedicalRecord?>(owner) : null;
                Console.WriteLine($"Dialog result: {(result != null ? $"Animal data received: {result.Animal.Name}" : "null/cancelled")}");

                if (result != null)
                {
                    var animal = result.Animal;
                    var initialMedicalRecord = result.InitialMedicalRecord;

                    Console.WriteLine($"Adding new animal: {animal.Name} ({animal.Species}) for client {animal.ClientId}");
                    if (initialMedicalRecord != null)
                    {
                        Console.WriteLine($"With initial medical record: Diagnosis '{initialMedicalRecord.Diagnosis}'");
                    }
                    Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                    using var context = DatabaseHelper.CreateContext();
                    context.Animals.Add(animal);
                    Console.WriteLine("Animal added to context");

                    // If there's an initial medical record, link it to the animal after saving
                    if (initialMedicalRecord != null)
                    {
                        // We need to save the animal first to get its ID
                        await context.SaveChangesAsync();
                        initialMedicalRecord.AnimalId = animal.Id;
                        context.MedicalRecords.Add(initialMedicalRecord);
                        Console.WriteLine("Initial medical record added to context");
                    }

                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");
                    Console.WriteLine($"Animal saved with ID: {animal.Id}");
                    if (initialMedicalRecord != null)
                    {
                        Console.WriteLine($"Medical record saved with ID: {initialMedicalRecord.Id}");
                    }

                    // Verify it was saved
                    var verify = await context.Animals.Include(a => a.Client).FirstOrDefaultAsync(a => a.Id == animal.Id);
                    Console.WriteLine($"Verification: Animal {animal.Id} {(verify != null ? $"EXISTS - {verify.Name}" : "NOT FOUND")} in database");

                    // Reload all data from database
                    Console.WriteLine("Reloading animals from database...");
                    await LoadAnimalsAsync();
                    Console.WriteLine("=== AddAnimal COMPLETE ===");
                }
                else
                {
                    Console.WriteLine("=== AddAnimal CANCELLED ===");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! ERROR in AddAnimal !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async void EditAnimal(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int animalId)
            {
                try
                {
                    Console.WriteLine($"=== EditAnimal START - ID: {animalId} ===");

                    // Find the animal to edit
                    var animalToEdit = _animals.FirstOrDefault(a => a.Id == animalId);
                    if (animalToEdit == null)
                    {
                        Console.WriteLine($"ERROR: Animal with ID {animalId} not found in current list");
                        return;
                    }

                    // Create a modified AddAnimalDialog for editing
                    var dialog = new EditAnimalDialog(animalToEdit);
                    var owner = Window.GetTopLevel(this) as Window;

                    var result = owner != null ? await dialog.ShowDialog<Animal?>(owner) : null;
                    Console.WriteLine($"Dialog result: {(result != null ? $"Animal data updated: {result.Name}" : "null/cancelled")}");

                    if (result != null)
                    {
                        Console.WriteLine($"Updating animal: {result.Name} ({result.Species})");
                        Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                        using var context = DatabaseHelper.CreateContext();
                        var existingAnimal = await context.Animals.FindAsync(animalId);
                        if (existingAnimal != null)
                        {
                            // Update the existing animal properties
                            existingAnimal.Name = result.Name;
                            existingAnimal.Species = result.Species;
                            existingAnimal.Breed = result.Breed;
                            existingAnimal.Gender = result.Gender;
                            existingAnimal.Weight = result.Weight;
                            existingAnimal.VaccinationRecords = result.VaccinationRecords;
                            existingAnimal.Allergies = result.Allergies;
                            existingAnimal.UpdatedAt = DateTime.UtcNow;

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Animal {animalId} updated successfully");

                            // Reload data
                            await LoadAnimalsAsync();
                        }
                    }

                    Console.WriteLine("=== EditAnimal COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in EditAnimal !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int animalId)
            {
                try
                {
                    Console.WriteLine($"=== DeleteAnimal START - ID: {animalId} ===");

                    // Find the animal to display info in confirmation
                    var animalToDelete = _animals.FirstOrDefault(a => a.Id == animalId);

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
                                    Text = animalToDelete != null
                                        ? $"Are you sure you want to delete animal '{animalToDelete.Name}' ({animalToDelete.Species})?\n\nThis will also delete all associated medical records and appointments."
                                        : $"Are you sure you want to delete animal #{animalId}?\n\nThis will also delete all associated medical records and appointments.",
                                    Margin = new Avalonia.Thickness(0, 0, 0, 20),
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                                },
                                new StackPanel
                                {
                                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                    Children =
                                    {
                                        new Button
                                        {
                                            Content = "Cancel",
                                            Padding = new Avalonia.Thickness(20, 10),
                                            Margin = new Avalonia.Thickness(0, 0, 10, 0),
                                            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(148, 163, 184)),
                                            Foreground = Avalonia.Media.Brushes.White
                                        },
                                        new Button
                                        {
                                            Content = "Delete",
                                            Padding = new Avalonia.Thickness(20, 10),
                                            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(239, 68, 68)),
                                            Foreground = Avalonia.Media.Brushes.White
                                        }
                                    }
                                }
                            }
                        },
                        SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight,
                        WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                    };

                    var owner = Window.GetTopLevel(this) as Window;
                    var confirmResult = owner != null ? await confirmDialog.ShowDialog<bool?>(owner) : null;

                    if (confirmResult == true)
                    {
                        // Delete the animal and all related records
                        using var context = DatabaseHelper.CreateContext();
                        var animalToRemove = await context.Animals
                            .Include(a => a.MedicalRecords)
                            .Include(a => a.Appointments)
                            .FirstOrDefaultAsync(a => a.Id == animalId);

                        if (animalToRemove != null)
                        {
                            // Remove related records first (cascade delete should handle this, but being explicit)
                            context.MedicalRecords.RemoveRange(animalToRemove.MedicalRecords);
                            context.Appointments.RemoveRange(animalToRemove.Appointments);
                            context.Animals.Remove(animalToRemove);

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Animal {animalId} and all related records deleted successfully");

                            // Reload data
                            await LoadAnimalsAsync();
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Animal {animalId} not found in database");
                        }
                    }

                    Console.WriteLine("=== DeleteAnimal COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in DeleteAnimal !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
