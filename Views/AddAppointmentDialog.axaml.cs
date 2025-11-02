using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddAppointmentDialog : Window
    {
        public AddAppointmentDialog()
        {
            InitializeComponent();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var animalIdText = this.FindControl<TextBox>("AnimalIdBox")?.Text;
            var petNameText = this.FindControl<TextBox>("PetNameBox")?.Text;
            var appointmentTypeSelector = this.FindControl<ComboBox>("AppointmentTypeSelector");
            var dateTimeText = this.FindControl<TextBox>("AppointmentDateTimeBox")?.Text;

            if (string.IsNullOrWhiteSpace(animalIdText) || string.IsNullOrWhiteSpace(petNameText) ||
                appointmentTypeSelector?.SelectedItem == null || string.IsNullOrWhiteSpace(dateTimeText))
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Animal ID, Pet Name, Appointment Type, and Date/Time are required fields", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            var appointmentTypeText = (appointmentTypeSelector.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // Validate AnimalId exists
            if (!int.TryParse(animalIdText, out var animalId) || animalId <= 0)
            {
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Invalid Animal ID - must be a positive number", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            // Verify animal exists in database
            using var validationContext = DatabaseHelper.CreateContext();
            var animalExists = await validationContext.Animals.AnyAsync(a => a.Id == animalId.ToString());
            if (!animalExists)
            {
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = $"Animal with ID {animalId} does not exist in the database", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            // Parse appointment date time
            if (!DateTime.TryParse(dateTimeText, out var appointmentDateTime))
            {
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Invalid date/time format. Please use YYYY-MM-DD HH:MM format.", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            var appointment = new Appointment
            {
                AnimalId = animalId.ToString(),
                PetName = petNameText,
                OwnerName = this.FindControl<TextBox>("OwnerNameBox")?.Text,
                AppointmentType = appointmentTypeText,
                AppointmentDateTime = appointmentDateTime,
                VeterinarianName = this.FindControl<TextBox>("VeterinarianNameBox")?.Text,
                Status = Enum.TryParse((this.FindControl<ComboBox>("StatusSelector")?.SelectedItem as ComboBoxItem)?.Content?.ToString(), out AppointmentStatus status) ? status : AppointmentStatus.Waiting,
                Description = this.FindControl<TextBox>("DescriptionBox")?.Text,
                Notes = this.FindControl<TextBox>("NotesBox")?.Text,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Console.WriteLine($"Created appointment: {appointment.PetName} - {appointment.AppointmentType} at {appointment.AppointmentDateTime:g}");
            Close(appointment);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
