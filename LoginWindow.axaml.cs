using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Services;

namespace VitaClinic.WebAPI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            
            var authService = new AuthService(context);
            await authService.CreateDefaultAdminIfNotExists();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = this.FindControl<TextBox>("UsernameTextBox")?.Text;
            var password = this.FindControl<TextBox>("PasswordTextBox")?.Text;
            var errorMessage = this.FindControl<TextBlock>("ErrorMessage");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                if (errorMessage != null)
                {
                    errorMessage.Text = "Please enter username and password";
                    errorMessage.IsVisible = true;
                }
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            var authService = new AuthService(context);
            var user = await authService.AuthenticateAsync(username, password);

            if (user != null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                if (errorMessage != null)
                {
                    errorMessage.Text = "Invalid username or password";
                    errorMessage.IsVisible = true;
                }
            }
        }
    }
}
