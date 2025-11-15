using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class EditClientDialog : Window
    {
        private readonly Client _client;

        public EditClientDialog(Client client)
        {
            InitializeComponent();
            _client = client;

            // Populate fields with existing data
            this.FindControl<TextBox>("FirstNameBox")!.Text = client.FirstName;
            this.FindControl<TextBox>("LastNameBox")!.Text = client.LastName;
            this.FindControl<TextBox>("EmailBox")!.Text = client.Email;
            this.FindControl<TextBox>("PhoneBox")!.Text = client.Phone;
            this.FindControl<TextBox>("AddressBox")!.Text = client.Address;
            this.FindControl<TextBox>("CityBox")!.Text = client.City;
            this.FindControl<TextBox>("StateBox")!.Text = client.State;
            this.FindControl<TextBox>("ZipCodeBox")!.Text = client.ZipCode;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Update existing client
            _client.FirstName = this.FindControl<TextBox>("FirstNameBox")?.Text;
            _client.LastName = this.FindControl<TextBox>("LastNameBox")?.Text;
            _client.Email = this.FindControl<TextBox>("EmailBox")?.Text;
            _client.Phone = this.FindControl<TextBox>("PhoneBox")?.Text;
            _client.Address = this.FindControl<TextBox>("AddressBox")?.Text;
            _client.City = this.FindControl<TextBox>("CityBox")?.Text;
            _client.State = this.FindControl<TextBox>("StateBox")?.Text;
            _client.ZipCode = this.FindControl<TextBox>("ZipCodeBox")?.Text;

            Close(_client);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
