using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddAnimalDialog : Window
    {
        public AddAnimalDialog()
        {
            InitializeComponent();
            _ = LoadClientsForSelector();
        }

        private async Task LoadClientsForSelector()
        {
            try
            {
                using var context = DatabaseHelper.CreateContext();
                var clients = await context.Clients.ToListAsync();

                var selector = this.FindControl<ComboBox>("ClientSelector");
                if (selector != null)
                {
                    selector.ItemsSource = clients;
                    selector.DisplayMemberBinding = new Avalonia.Data.Binding("FirstName") { StringFormat = "{0} {1} (ID: {2})" };
                    selector.SelectedValueBinding = new Avalonia.Data.Binding("Id");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading clients for selector: {ex.Message}");
            }
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var clientSelector = this.FindControl<ComboBox>("ClientSelector");
            var nameText = this.FindControl<TextBox>("NameBox")?.Text;
            var speciesText = this.FindControl<TextBox>("SpeciesBox")?.Text;

            if (clientSelector?.SelectedItem == null || string.IsNullOrWhiteSpace(nameText) || string.IsNullOrWhiteSpace(speciesText))
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Client selection, Name, and Species are required fields", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            var selectedClient = clientSelector.SelectedItem as Client;
            var clientId = selectedClient!.Id;

            var animal = new Animal
            {
                ClientId = clientId,
                Name = nameText,
                Species = speciesText,
                Breed = this.FindControl<TextBox>("BreedBox")?.Text,
                Gender = this.FindControl<TextBox>("GenderBox")?.Text,
                Weight = decimal.TryParse(this.FindControl<TextBox>("WeightBox")?.Text, out var weight) ? weight : 0,
                VaccinationRecords = this.FindControl<TextBox>("VaccinationBox")?.Text,
                Allergies = this.FindControl<TextBox>("AllergiesBox")?.Text,
                DateOfBirth = DateTime.UtcNow.AddYears(-1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create initial medical record if diagnosis is provided
            var initialDiagnosis = this.FindControl<TextBox>("InitialDiagnosisBox")?.Text;
            var initialTreatment = this.FindControl<TextBox>("InitialTreatmentBox")?.Text;

            MedicalRecord? initialMedicalRecord = null;
            if (!string.IsNullOrWhiteSpace(initialDiagnosis))
            {
                initialMedicalRecord = new MedicalRecord
                {
                    Diagnosis = initialDiagnosis,
                    Treatment = initialTreatment,
                    RecordDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            // Create a composite object to return both animal and initial medical record
            var result = new AnimalWithMedicalRecord
            {
                Animal = animal,
                InitialMedicalRecord = initialMedicalRecord
            };

            Console.WriteLine($"Created animal: {animal.Name} ({animal.Species}) for client {animal.ClientId}");
            if (initialMedicalRecord != null)
            {
                Console.WriteLine($"With initial medical record: Diagnosis '{initialMedicalRecord.Diagnosis}'");
            }
            Close(result);
        }

        // Helper class to return both animal and medical record
        public class AnimalWithMedicalRecord
        {
            public Animal Animal { get; set; } = null!;
            public MedicalRecord? InitialMedicalRecord { get; set; }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
