using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddMedicalRecordDialog : Window
    {
        public AddMedicalRecordDialog()
        {
            InitializeComponent();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var animalIdText = this.FindControl<TextBox>("AnimalIdBox")?.Text;
            var diagnosisText = this.FindControl<TextBox>("DiagnosisBox")?.Text;

            if (string.IsNullOrWhiteSpace(animalIdText) || string.IsNullOrWhiteSpace(diagnosisText))
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Animal ID and Diagnosis are required fields", Margin = new Avalonia.Thickness(20) },
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
            var animalExists = await validationContext.Animals.AnyAsync(a => a.Id == animalId);
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

            // Parse next checkup date if provided
            DateTime? nextCheckupDate = null;
            var nextCheckupText = this.FindControl<TextBox>("NextCheckupBox")?.Text;
            if (!string.IsNullOrWhiteSpace(nextCheckupText))
            {
                if (DateTime.TryParse(nextCheckupText, out var parsedDate))
                {
                    nextCheckupDate = parsedDate;
                }
                else
                {
                    var errorDialog = new Window
                    {
                        Title = "Validation Error",
                        Content = new TextBlock { Text = "Invalid date format for Next Checkup Date. Use YYYY-MM-DD format.", Margin = new Avalonia.Thickness(20) },
                        SizeToContent = SizeToContent.WidthAndHeight,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    await errorDialog.ShowDialog(this);
                    return;
                }
            }

            var medicalRecord = new MedicalRecord
            {
                AnimalId = animalId,
                Diagnosis = diagnosisText,
                Treatment = this.FindControl<TextBox>("TreatmentBox")?.Text,
                Medication = this.FindControl<TextBox>("MedicationBox")?.Text,
                Notes = this.FindControl<TextBox>("NotesBox")?.Text,
                NextCheckupDate = nextCheckupDate,
                RecordDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Console.WriteLine($"Created medical record: Diagnosis '{medicalRecord.Diagnosis}' for animal {medicalRecord.AnimalId}");
            Close(medicalRecord);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}