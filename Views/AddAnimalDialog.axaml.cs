using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddAnimalDialog : Window
    {
        public AddAnimalDialog()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var animal = new Animal
            {
                ClientId = int.TryParse(this.FindControl<TextBox>("ClientIdBox")?.Text, out var clientId) ? clientId : 0,
                Name = this.FindControl<TextBox>("NameBox")?.Text,
                Species = this.FindControl<TextBox>("SpeciesBox")?.Text,
                Breed = this.FindControl<TextBox>("BreedBox")?.Text,
                Gender = this.FindControl<TextBox>("GenderBox")?.Text,
                Weight = decimal.TryParse(this.FindControl<TextBox>("WeightBox")?.Text, out var weight) ? weight : 0,
                DateOfBirth = DateTime.UtcNow.AddYears(-1)
            };
            
            Close(animal);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
