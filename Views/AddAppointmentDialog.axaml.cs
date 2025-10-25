using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddAppointmentDialog : Window
    {
        public AddAppointmentDialog()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var typeBox = this.FindControl<ComboBox>("TypeBox");
            var selectedType = (typeBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Checkup";
            
            var appointment = new Appointment
            {
                AnimalId = int.TryParse(this.FindControl<TextBox>("AnimalIdBox")?.Text, out var animalId) ? animalId : 0,
                VeterinarianId = int.TryParse(this.FindControl<TextBox>("VetIdBox")?.Text, out var vetId) ? vetId : 1,
                PetName = this.FindControl<TextBox>("PetNameBox")?.Text,
                OwnerName = this.FindControl<TextBox>("OwnerNameBox")?.Text,
                VeterinarianName = this.FindControl<TextBox>("VetNameBox")?.Text,
                AppointmentType = selectedType,
                AppointmentDateTime = DateTime.Now.AddDays(1),
                Status = AppointmentStatus.Confirmed
            };
            
            Close(appointment);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
