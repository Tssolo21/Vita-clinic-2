using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.IO;
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
            using var context = DatabaseHelper.CreateContext();
            var authService = new AuthService(context);
            await authService.CreateDefaultAdminIfNotExists();
            Console.WriteLine("Database initialized and default admin created if needed");
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

            Console.WriteLine($"Attempting login for user: {username}");
            
            using var context = DatabaseHelper.CreateContext();
            var authService = new AuthService(context);
            var user = await authService.AuthenticateAsync(username, password);

            if (user != null)
            {
                Console.WriteLine($"Login successful for: {user.FullName}");
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                Console.WriteLine("Login failed: Invalid credentials");
                if (errorMessage != null)
                {
                    errorMessage.Text = "Invalid username or password";
                    errorMessage.IsVisible = true;
                }
            }
        }
    }
}
