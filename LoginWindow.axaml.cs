using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.ApplicationLifetimes;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace VitaClinic.WebAPI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            var username = this.FindControl<TextBox>("UsernameBox")?.Text;
            var password = this.FindControl<TextBox>("PasswordBox")?.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Login Error",
                    Content = new TextBlock { Text = "Username and password are required", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
                return;
            }

            try
            {
                using var context = DatabaseHelper.CreateContext();
                var authService = new AuthService(context);
                await authService.CreateDefaultAdminIfNotExists();
                var user = await authService.AuthenticateAsync(username, password);

                if (user != null)
                {
                    // Login successful, open main window
                    var mainWindow = new MainWindow(user);
                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = mainWindow;
                    }
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    // Show error message
                    var errorDialog = new Window
                    {
                        Title = "Login Error",
                        Content = new TextBlock { Text = "Invalid username or password", Margin = new Avalonia.Thickness(20) },
                        SizeToContent = SizeToContent.WidthAndHeight,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    await errorDialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                // Show error message
                var errorDialog = new Window
                {
                    Title = "Login Error",
                    Content = new TextBlock { Text = $"An error occurred: {ex.Message}", Margin = new Avalonia.Thickness(20) },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await errorDialog.ShowDialog(this);
            }
        }
    }
}
