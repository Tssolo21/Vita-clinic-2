using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;
using VitaClinic.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddClientDialog : Window
    {
        private Client? _clientToEdit;

        public AddClientDialog()
        {
            InitializeComponent();
            Title = "Add New Client";
            _ = LoadClientsForSelector();
        }

        public AddClientDialog(Client clientToEdit) : this()
        {
            _clientToEdit = clientToEdit;
            Title = "Edit Client";

            // Populate fields with existing data
            this.FindControl<TextBox>("FirstNameBox")!.Text = clientToEdit.FirstName;
            this.FindControl<TextBox>("LastNameBox")!.Text = clientToEdit.LastName;
            this.FindControl<TextBox>("EmailBox")!.Text = clientToEdit.Email;
            this.FindControl<TextBox>("PhoneBox")!.Text = clientToEdit.Phone;
            this.FindControl<TextBox>("AddressBox")!.Text = clientToEdit.Address;
            this.FindControl<TextBox>("CityBox")!.Text = clientToEdit.City;
            this.FindControl<TextBox>("StateBox")!.Text = clientToEdit.State;
            this.FindControl<TextBox>("ZipCodeBox")!.Text = clientToEdit.ZipCode;
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

        private void Save(object sender, RoutedEventArgs e)
        {
            if (_clientToEdit != null)
            {
                // Update existing client
                _clientToEdit.FirstName = this.FindControl<TextBox>("FirstNameBox")?.Text;
                _clientToEdit.LastName = this.FindControl<TextBox>("LastNameBox")?.Text;
                _clientToEdit.Email = this.FindControl<TextBox>("EmailBox")?.Text;
                _clientToEdit.Phone = this.FindControl<TextBox>("PhoneBox")?.Text;
                _clientToEdit.Address = this.FindControl<TextBox>("AddressBox")?.Text;
                _clientToEdit.City = this.FindControl<TextBox>("CityBox")?.Text;
                _clientToEdit.State = this.FindControl<TextBox>("StateBox")?.Text;
                _clientToEdit.ZipCode = this.FindControl<TextBox>("ZipCodeBox")?.Text;

                Close(_clientToEdit);
            }
            else
            {
                // Create new client
                var client = new Client
                {
                    FirstName = this.FindControl<TextBox>("FirstNameBox")?.Text,
                    LastName = this.FindControl<TextBox>("LastNameBox")?.Text,
                    Email = this.FindControl<TextBox>("EmailBox")?.Text,
                    Phone = this.FindControl<TextBox>("PhoneBox")?.Text,
                    Address = this.FindControl<TextBox>("AddressBox")?.Text,
                    City = this.FindControl<TextBox>("CityBox")?.Text,
                    State = this.FindControl<TextBox>("StateBox")?.Text,
                    ZipCode = this.FindControl<TextBox>("ZipCodeBox")?.Text,
                    Status = ClientStatus.Active
                };

                Close(client);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
