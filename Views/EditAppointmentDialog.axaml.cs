using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class EditAppointmentDialog : Window
    {
        private Appointment _appointmentToEdit;

        public EditAppointmentDialog(Appointment appointment)
        {
            InitializeComponent();
            _appointmentToEdit = appointment;
            LoadAppointmentData();
        }

        private void LoadAppointmentData()
        {
            this.FindControl<TextBox>("AnimalIdBox")!.Text = _appointmentToEdit.AnimalId.ToString();
            this.FindControl<TextBox>("PetNameBox")!.Text = _appointmentToEdit.PetName;
            this.FindControl<TextBox>("OwnerNameBox")!.Text = _appointmentToEdit.OwnerName;
            this.FindControl<TextBox>("AppointmentTypeBox")!.Text = _appointmentToEdit.AppointmentType;
            this.FindControl<TextBox>("AppointmentDateTimeBox")!.Text = _appointmentToEdit.AppointmentDateTime.ToString("yyyy-MM-dd HH:mm");
            this.FindControl<TextBox>("VeterinarianNameBox")!.Text = _appointmentToEdit.VeterinarianName;
            this.FindControl<TextBox>("StatusBox")!.Text = _appointmentToEdit.Status.ToString();
            this.FindControl<TextBox>("DescriptionBox")!.Text = _appointmentToEdit.Description;
            this.FindControl<TextBox>("NotesBox")!.Text = _appointmentToEdit.Notes;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var animalIdText = this.FindControl<TextBox>("AnimalIdBox")?.Text;
            var petNameText = this.FindControl<TextBox>("PetNameBox")?.Text;
            var appointmentTypeText = this.FindControl<TextBox>("AppointmentTypeBox")?.Text;
            var dateTimeText = this.FindControl<TextBox>("AppointmentDateTimeBox")?.Text;

            if (string.IsNullOrWhiteSpace(animalIdText) || string.IsNullOrWhiteSpace(petNameText) ||
                string.IsNullOrWhiteSpace(appointmentTypeText) || string.IsNullOrWhiteSpace(dateTimeText))
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

            // Update the appointment with new values
            _appointmentToEdit.AnimalId = animalId.ToString();
            _appointmentToEdit.PetName = petNameText;
            _appointmentToEdit.OwnerName = this.FindControl<TextBox>("OwnerNameBox")?.Text;
            _appointmentToEdit.AppointmentType = appointmentTypeText;
            _appointmentToEdit.AppointmentDateTime = appointmentDateTime;
            _appointmentToEdit.VeterinarianName = this.FindControl<TextBox>("VeterinarianNameBox")?.Text;
            _appointmentToEdit.Status = Enum.TryParse(this.FindControl<TextBox>("StatusBox")?.Text, out AppointmentStatus status) ? status : AppointmentStatus.Waiting;
            _appointmentToEdit.Description = this.FindControl<TextBox>("DescriptionBox")?.Text;
            _appointmentToEdit.Notes = this.FindControl<TextBox>("NotesBox")?.Text;
            _appointmentToEdit.UpdatedAt = DateTime.UtcNow;

            Console.WriteLine($"Updated appointment: {_appointmentToEdit.PetName} - {_appointmentToEdit.AppointmentType} at {_appointmentToEdit.AppointmentDateTime:g}");
            Close(_appointmentToEdit);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}