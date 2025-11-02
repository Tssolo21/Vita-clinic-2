using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class MedicalRecordsView : UserControl, INotifyPropertyChanged
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<MedicalRecord> _medicalRecords = new ObservableCollection<MedicalRecord>();

        public ObservableCollection<MedicalRecord> MedicalRecords => _medicalRecords;

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MedicalRecordsView()
        {
            InitializeComponent();
        }

        public MedicalRecordsView(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
            this.DataContext = this;
            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        private async void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            InitializeMedicalRecordsList();
            await LoadMedicalRecordsAsync();
        }

        private void InitializeMedicalRecordsList()
        {
            var list = this.FindControl<ItemsControl>("MedicalRecordsList");
            if (list != null)
            {
                list.ItemsSource = _medicalRecords;
                Console.WriteLine("✓ MedicalRecordsList ItemsSource set successfully");
            }
            else
            {
                Console.WriteLine("✗ ERROR: MedicalRecordsList not found!");
            }
        }

        private async void LoadMedicalRecords(object? sender, RoutedEventArgs? e)
        {
            await LoadMedicalRecordsAsync();
        }

        private async Task LoadMedicalRecordsAsync()
        {
            try
            {
                Console.WriteLine("Loading medical records...");
                using var context = DatabaseHelper.CreateContext();
                var medicalRecords = await context.MedicalRecords.Include(m => m.Animal).ToListAsync();
                
                Console.WriteLine($"Found {medicalRecords.Count} medical records in database");
                
                // Use Dispatcher to update collection on UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _medicalRecords.Clear();
                    foreach (var record in medicalRecords)
                    {
                        _medicalRecords.Add(record);
                    }
                    Console.WriteLine($"Loaded {_medicalRecords.Count} medical records to UI ObservableCollection");

                    // Ensure ItemsControl is updated
                    var list = this.FindControl<ItemsControl>("MedicalRecordsList");
                    if (list != null)
                    {
                        list.ItemsSource = null;
                        list.ItemsSource = _medicalRecords;
                        Console.WriteLine("ItemsControl ItemsSource refreshed after loading");
                    }
                    else
                    {
                        Console.WriteLine("✗ ERROR: MedicalRecordsList not found during reload!");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical records: {ex.Message}");
            }
        }

        private async void AddMedicalRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine("=== AddMedicalRecord START ===");
                var dialog = new AddMedicalRecordDialog();
                var owner = Window.GetTopLevel(this) as Window;
                Console.WriteLine($"Owner window: {owner?.GetType().Name ?? "null"}");

                var result = owner != null ? await dialog.ShowDialog<MedicalRecord?>(owner) : null;
                Console.WriteLine($"Dialog result: {(result != null ? $"Medical record data received: Diagnosis '{result.Diagnosis}'" : "null/cancelled")}");

                if (result != null)
                {
                    // Validate that AnimalId exists before saving
                    using var validationContext = DatabaseHelper.CreateContext();
                    var animalExists = await validationContext.Animals.AnyAsync(a => a.Id == result.AnimalId);
                    if (!animalExists)
                    {
                        Console.WriteLine($"ERROR: Animal with ID {result.AnimalId} does not exist!");
                        // You might want to show a user-friendly error dialog here
                        return;
                    }

                    Console.WriteLine($"Adding medical record: Diagnosis '{result.Diagnosis}' for animal {result.AnimalId}");
                    Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                    using var context = DatabaseHelper.CreateContext();
                    // Note: CreatedAt, UpdatedAt, and RecordDate are now set in the dialog
                    context.MedicalRecords.Add(result);
                    Console.WriteLine("Medical record added to context");

                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");
                    Console.WriteLine($"Medical record saved with ID: {result.Id}");

                    // Verify it was saved
                    var verify = await context.MedicalRecords.Include(mr => mr.Animal).FirstOrDefaultAsync(mr => mr.Id == result.Id);
                    Console.WriteLine($"Verification: Medical record {result.Id} {(verify != null ? $"EXISTS - Diagnosis '{verify.Diagnosis}'" : "NOT FOUND")} in database");

                    // Reload all data from database
                    Console.WriteLine("Reloading medical records from database...");
                    await LoadMedicalRecordsAsync();
                    Console.WriteLine("=== AddMedicalRecord COMPLETE ===");
                }
                else
                {
                    Console.WriteLine("=== AddMedicalRecord CANCELLED ===");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! ERROR in AddMedicalRecord !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async void EditMedicalRecord(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string recordId)
            {
                try
                {
                    Console.WriteLine($"=== EditMedicalRecord START - ID: {recordId} ===");

                    // Find the medical record
                    var recordToEdit = _medicalRecords.FirstOrDefault(r => r.Id == recordId);
                    if (recordToEdit == null)
                    {
                        Console.WriteLine($"ERROR: Medical record with ID {recordId} not found in current list");
                        return;
                    }

                    // Create edit dialog (for now, we'll use the add dialog but pre-populate it)
                    // TODO: Create a dedicated EditMedicalRecordDialog
                    var dialog = new AddMedicalRecordDialog();
                    var owner = Window.GetTopLevel(this) as Window;

                    // For now, show the add dialog - in a full implementation, we'd create an edit dialog
                    var result = owner != null ? await dialog.ShowDialog<MedicalRecord?>(owner) : null;

                    if (result != null)
                    {
                        // Update the existing record
                        using var context = DatabaseHelper.CreateContext();
                        var existingRecord = await context.MedicalRecords.FindAsync(recordId);
                        if (existingRecord != null)
                        {
                            existingRecord.Diagnosis = result.Diagnosis;
                            existingRecord.Treatment = result.Treatment;
                            existingRecord.Medication = result.Medication;
                            existingRecord.Notes = result.Notes;
                            existingRecord.NextCheckupDate = result.NextCheckupDate;
                            existingRecord.UpdatedAt = DateTime.UtcNow;

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Medical record {recordId} updated successfully");

                            // Reload data
                            await LoadMedicalRecordsAsync();
                        }
                    }

                    Console.WriteLine("=== EditMedicalRecord COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in EditMedicalRecord !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async void DeleteMedicalRecord(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string recordId)
            {
                try
                {
                    Console.WriteLine($"=== DeleteMedicalRecord START - ID: {recordId} ===");

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
                                    Text = $"Are you sure you want to delete medical record #{recordId}?",
                                    Margin = new Avalonia.Thickness(0, 0, 0, 20)
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
                        // Delete the record
                        using var context = DatabaseHelper.CreateContext();
                        var recordToDelete = await context.MedicalRecords.FindAsync(recordId);
                        if (recordToDelete != null)
                        {
                            context.MedicalRecords.Remove(recordToDelete);
                            await context.SaveChangesAsync();
                            Console.WriteLine($"Medical record {recordId} deleted successfully");

                            // Reload data
                            await LoadMedicalRecordsAsync();
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Medical record {recordId} not found in database");
                        }
                    }

                    Console.WriteLine("=== DeleteMedicalRecord COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in DeleteMedicalRecord !!!");
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
