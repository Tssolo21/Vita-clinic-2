using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class EditAnimalDialog : Window
    {
        private Animal _animalToEdit;

        public EditAnimalDialog(Animal animal)
        {
            InitializeComponent();
            _animalToEdit = animal;
            LoadAnimalData();
        }

        private void LoadAnimalData()
        {
            this.FindControl<TextBox>("ClientIdBox")!.Text = _animalToEdit.ClientId.ToString();
            this.FindControl<TextBox>("NameBox")!.Text = _animalToEdit.Name;
            this.FindControl<TextBox>("SpeciesBox")!.Text = _animalToEdit.Species;
            this.FindControl<TextBox>("BreedBox")!.Text = _animalToEdit.Breed;
            this.FindControl<TextBox>("GenderBox")!.Text = _animalToEdit.Gender;
            this.FindControl<TextBox>("WeightBox")!.Text = _animalToEdit.Weight.ToString();
            this.FindControl<TextBox>("VaccinationBox")!.Text = _animalToEdit.VaccinationRecords;
            this.FindControl<TextBox>("AllergiesBox")!.Text = _animalToEdit.Allergies;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            var clientIdText = this.FindControl<TextBox>("ClientIdBox")?.Text;
            var nameText = this.FindControl<TextBox>("NameBox")?.Text;
            var speciesText = this.FindControl<TextBox>("SpeciesBox")?.Text;

            if (string.IsNullOrWhiteSpace(clientIdText) || string.IsNullOrWhiteSpace(nameText) || string.IsNullOrWhiteSpace(speciesText))
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Client ID, Name, and Species are required fields", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            // Validate ClientId exists
            if (!int.TryParse(clientIdText, out var clientId) || clientId <= 0)
            {
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = "Invalid Client ID - must be a positive number", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            // Verify client exists in database
            using var validationContext = DatabaseHelper.CreateContext();
            var clientExists = await validationContext.Clients.AnyAsync(c => c.Id == clientId.ToString());
            if (!clientExists)
            {
                var errorDialog = new Window
                {
                    Title = "Validation Error",
                    Content = new TextBlock { Text = $"Client with ID {clientId} does not exist in the database", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            // Update the animal with new values
            _animalToEdit.ClientId = clientId.ToString();
            _animalToEdit.Name = nameText;
            _animalToEdit.Species = speciesText;
            _animalToEdit.Breed = this.FindControl<TextBox>("BreedBox")?.Text;
            _animalToEdit.Gender = this.FindControl<TextBox>("GenderBox")?.Text;
            _animalToEdit.Weight = decimal.TryParse(this.FindControl<TextBox>("WeightBox")?.Text, out var weight) ? weight : 0;
            _animalToEdit.VaccinationRecords = this.FindControl<TextBox>("VaccinationBox")?.Text;
            _animalToEdit.Allergies = this.FindControl<TextBox>("AllergiesBox")?.Text;
            _animalToEdit.UpdatedAt = DateTime.UtcNow;

            Console.WriteLine($"Updated animal: {_animalToEdit.Name} ({_animalToEdit.Species}) for client {_animalToEdit.ClientId}");
            Close(_animalToEdit);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}