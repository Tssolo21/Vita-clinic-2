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

namespace VitaClinic.WebAPI.Views
{
    public partial class AppointmentsView : UserControl, INotifyPropertyChanged
    {
        private MainWindow? _mainWindow;
        private ObservableCollection<Appointment> _appointments = new ObservableCollection<Appointment>();

        public ObservableCollection<Appointment> Appointments => _appointments;

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AppointmentsView()
        {
            InitializeComponent();
        }

        public AppointmentsView(MainWindow mainWindow) : this()
        {
            Console.WriteLine("=== AppointmentsView Constructor START ===");
            _mainWindow = mainWindow;
            this.DataContext = this;
            this.AttachedToVisualTree += OnAttachedToVisualTree;
            Console.WriteLine("=== AppointmentsView Constructor COMPLETE ===");
        }

        private async void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            Console.WriteLine("=== OnAttachedToVisualTree START ===");
            try
            {
                InitializeAppointmentsList();
                await LoadAppointmentsAsync();
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

        private void InitializeAppointmentsList()
        {
            Console.WriteLine("=== InitializeAppointmentsList START ===");
            try
            {
                var list = this.FindControl<ItemsControl>("AppointmentsList");
                if (list != null)
                {
                    list.ItemsSource = _appointments;
                    Console.WriteLine("✓ AppointmentsList ItemsSource set successfully");
                }
                else
                {
                    Console.WriteLine("✗ ERROR: AppointmentsList not found!");
                }
                Console.WriteLine("=== InitializeAppointmentsList COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! CRASH in InitializeAppointmentsList !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async void LoadAppointments(object? sender, RoutedEventArgs? e)
        {
            await LoadAppointmentsAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            try
            {
                Console.WriteLine("Loading appointments...");
                using var context = DatabaseHelper.CreateContext();
                var appointments = await context.Appointments.Include(a => a.Animal).ToListAsync();
                
                Console.WriteLine($"Found {appointments.Count} appointments in database");
                
                // Use Dispatcher to update collection on UI thread
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _appointments.Clear();
                    foreach (var appointment in appointments)
                    {
                        _appointments.Add(appointment);
                    }
                    Console.WriteLine($"Loaded {_appointments.Count} appointments to UI ObservableCollection");

                    // Ensure ItemsControl is updated
                    var list = this.FindControl<ItemsControl>("AppointmentsList");
                    if (list != null)
                    {
                        list.ItemsSource = null;
                        list.ItemsSource = _appointments;
                        Console.WriteLine("ItemsControl ItemsSource refreshed after loading");
                    }
                    else
                    {
                        Console.WriteLine("✗ ERROR: AppointmentsList not found during reload!");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
            }
        }

        private async void AddAppointment(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine("=== AddAppointment START ===");
                var dialog = new AddAppointmentDialog();
                var owner = Window.GetTopLevel(this) as Window;
                Console.WriteLine($"Owner window: {owner?.GetType().Name ?? "null"}");

                var result = owner != null ? await dialog.ShowDialog<Appointment?>(owner) : null;
                Console.WriteLine($"Dialog result: {(result != null ? "Appointment data received" : "null/cancelled")}");

                if (result != null)
                {
                    Console.WriteLine($"Adding new appointment for: {result.PetName}");
                    Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                    using var context = DatabaseHelper.CreateContext();
                    result.CreatedAt = DateTime.UtcNow;
                    result.UpdatedAt = DateTime.UtcNow;

                    context.Appointments.Add(result);
                    Console.WriteLine("Appointment added to context");

                    var changes = await context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync returned: {changes} changes");
                    Console.WriteLine($"Appointment saved with ID: {result.Id}");

                    // Verify it was saved
                    var verify = await context.Appointments.FindAsync(result.Id);
                    Console.WriteLine($"Verification: Appointment {result.Id} {(verify != null ? "EXISTS" : "NOT FOUND")} in database");

                    // Reload all data from database
                    Console.WriteLine("Reloading appointments from database...");
                    await LoadAppointmentsAsync();
                    Console.WriteLine("=== AddAppointment COMPLETE ===");
                }
                else
                {
                    Console.WriteLine("=== AddAppointment CANCELLED ===");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! ERROR in AddAppointment !!!");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private async void EditAppointment(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string appointmentId)
            {
                try
                {
                    Console.WriteLine($"=== EditAppointment START - ID: {appointmentId} ===");

                    // Find the appointment to edit
                    var appointmentToEdit = _appointments.FirstOrDefault(a => a.Id == appointmentId);
                    if (appointmentToEdit == null)
                    {
                        Console.WriteLine($"ERROR: Appointment with ID {appointmentId} not found in current list");
                        return;
                    }

                    // Create a modified AddAppointmentDialog for editing
                    var dialog = new EditAppointmentDialog(appointmentToEdit);
                    var owner = Window.GetTopLevel(this) as Window;

                    var result = owner != null ? await dialog.ShowDialog<Appointment?>(owner) : null;
                    Console.WriteLine($"Dialog result: {(result != null ? $"Appointment data updated: {result.PetName} - {result.AppointmentType}" : "null/cancelled")}");

                    if (result != null)
                    {
                        Console.WriteLine($"Updating appointment: {result.PetName} - {result.AppointmentType}");
                        Console.WriteLine($"Database path: {DatabaseHelper.GetDatabasePath()}");

                        using var context = DatabaseHelper.CreateContext();
                        var existingAppointment = await context.Appointments.FindAsync(appointmentId);
                        if (existingAppointment != null)
                        {
                            // Update the existing appointment properties
                            existingAppointment.AnimalId = result.AnimalId;
                            existingAppointment.PetName = result.PetName;
                            existingAppointment.OwnerName = result.OwnerName;
                            existingAppointment.AppointmentType = result.AppointmentType;
                            existingAppointment.AppointmentDateTime = result.AppointmentDateTime;
                            existingAppointment.VeterinarianName = result.VeterinarianName;
                            existingAppointment.Status = result.Status;
                            existingAppointment.Description = result.Description;
                            existingAppointment.Notes = result.Notes;
                            existingAppointment.UpdatedAt = DateTime.UtcNow;

                            await context.SaveChangesAsync();
                            Console.WriteLine($"Appointment {appointmentId} updated successfully");

                            // Reload data
                            await LoadAppointmentsAsync();
                        }
                    }

                    Console.WriteLine("=== EditAppointment COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in EditAppointment !!!");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async void DeleteAppointment(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string appointmentId)
            {
                try
                {
                    Console.WriteLine($"=== DeleteAppointment START - ID: {appointmentId} ===");

                    // Find the appointment to display info in confirmation
                    var appointmentToDelete = _appointments.FirstOrDefault(a => a.Id == appointmentId);

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
                                    Text = appointmentToDelete != null
                                        ? $"Are you sure you want to delete the appointment for '{appointmentToDelete.PetName}' ({appointmentToDelete.AppointmentType})?\n\nScheduled for: {appointmentToDelete.AppointmentDateTime:g}"
                                        : $"Are you sure you want to delete appointment #{appointmentId}?",
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
                        // Delete the appointment
                        using var context = DatabaseHelper.CreateContext();
                        var appointmentToRemove = await context.Appointments.FindAsync(appointmentId.ToString());
                        if (appointmentToRemove != null)
                        {
                            context.Appointments.Remove(appointmentToRemove);
                            await context.SaveChangesAsync();
                            Console.WriteLine($"Appointment {appointmentId} deleted successfully");

                            // Reload data
                            await LoadAppointmentsAsync();
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: Appointment {appointmentId} not found in database");
                        }
                    }

                    Console.WriteLine("=== DeleteAppointment COMPLETE ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!! ERROR in DeleteAppointment !!!");
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
