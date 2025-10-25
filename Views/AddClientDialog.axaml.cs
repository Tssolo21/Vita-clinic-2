using Avalonia.Controls;
using Avalonia.Interactivity;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AddClientDialog : Window
    {
        public AddClientDialog()
        {
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var client = new Client
            {
                FirstName = this.FindControl<TextBox>("FirstNameBox")?.Text,
                LastName = this.FindControl<TextBox>("LastNameBox")?.Text,
                Email = this.FindControl<TextBox>("EmailBox")?.Text,
                Phone = this.FindControl<TextBox>("PhoneBox")?.Text,
                Address = this.FindControl<TextBox>("AddressBox")?.Text,
                Status = ClientStatus.Active
            };
            
            Close(client);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
