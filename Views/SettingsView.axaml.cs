using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class SettingsView : UserControl
    {
        private MainWindow? _mainWindow;
        private User? _currentUser;

        public SettingsView()
        {
            InitializeComponent();
        }

        public SettingsView(MainWindow mainWindow, User currentUser) : this()
        {
            _mainWindow = mainWindow;
            _currentUser = currentUser;
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_currentUser == null) return;

            var fullNameText = this.FindControl<TextBlock>("FullNameText");
            var usernameText = this.FindControl<TextBlock>("UsernameText");
            var emailText = this.FindControl<TextBlock>("EmailText");
            var roleText = this.FindControl<TextBlock>("RoleText");
            var lastLoginText = this.FindControl<TextBlock>("LastLoginText");

            if (fullNameText != null) fullNameText.Text = _currentUser.FullName ?? "N/A";
            if (usernameText != null) usernameText.Text = _currentUser.Username ?? "N/A";
            if (emailText != null) emailText.Text = _currentUser.Email ?? "N/A";
            if (roleText != null) roleText.Text = _currentUser.Role.ToString();
            if (lastLoginText != null) 
                lastLoginText.Text = _currentUser.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "Never";
        }

        private void UpdateProfile(object sender, RoutedEventArgs e)
        {
            ShowStatus("Use the sections below to update your profile information individually.", true);
        }

        private async void SaveFullName(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null) return;

                var newFullNameBox = this.FindControl<TextBox>("NewFullNameBox");
                var newFullName = newFullNameBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(newFullName))
                {
                    ShowStatus("Please enter a valid full name.", false);
                    return;
                }

                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
                var dirPath = Path.GetDirectoryName(dbPath);
                if (dirPath != null) Directory.CreateDirectory(dirPath);
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite($"Data Source={dbPath}");

                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.Id);
                
                if (user != null)
                {
                    user.FullName = newFullName;
                    await context.SaveChangesAsync();
                    
                    _currentUser.FullName = newFullName;
                    LoadUserData();
                    
                    if (newFullNameBox != null) newFullNameBox.Text = "";
                    ShowStatus($"Full name updated successfully to '{newFullName}'!", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error updating full name: {ex.Message}", false);
            }
        }

        private async void SaveUsername(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null) return;

                var newUsernameBox = this.FindControl<TextBox>("NewUsernameBox");
                var newUsername = newUsernameBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    ShowStatus("Please enter a valid username.", false);
                    return;
                }

                if (newUsername.Length < 3)
                {
                    ShowStatus("Username must be at least 3 characters long.", false);
                    return;
                }

                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
                var dirPath = Path.GetDirectoryName(dbPath);
                if (dirPath != null) Directory.CreateDirectory(dirPath);
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite($"Data Source={dbPath}");

                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                
                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == newUsername);
                if (existingUser != null && existingUser.Id != _currentUser.Id)
                {
                    ShowStatus("Username already exists. Please choose a different username.", false);
                    return;
                }
                
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.Id);
                
                if (user != null)
                {
                    user.Username = newUsername;
                    await context.SaveChangesAsync();
                    
                    _currentUser.Username = newUsername;
                    LoadUserData();
                    
                    if (newUsernameBox != null) newUsernameBox.Text = "";
                    ShowStatus($"Username updated successfully to '{newUsername}'!", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error updating username: {ex.Message}", false);
            }
        }

        private async void SaveEmail(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null) return;

                var newEmailBox = this.FindControl<TextBox>("NewEmailBox");
                var newEmail = newEmailBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(newEmail))
                {
                    ShowStatus("Please enter a valid email address.", false);
                    return;
                }

                if (!newEmail.Contains("@") || !newEmail.Contains("."))
                {
                    ShowStatus("Please enter a valid email format.", false);
                    return;
                }

                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VitaClinic", "vitaclinic_desktop.db");
                var dirPath = Path.GetDirectoryName(dbPath);
                if (dirPath != null) Directory.CreateDirectory(dirPath);
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite($"Data Source={dbPath}");

                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.Id);
                
                if (user != null)
                {
                    user.Email = newEmail;
                    await context.SaveChangesAsync();
                    
                    _currentUser.Email = newEmail;
                    LoadUserData();
                    
                    if (newEmailBox != null) newEmailBox.Text = "";
                    ShowStatus($"Email updated successfully to '{newEmail}'!", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error updating email: {ex.Message}", false);
            }
        }

        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null) return;

                var dialog = new ChangePasswordDialog(_currentUser);
                var owner = Window.GetTopLevel(this) as Window;
                var result = owner != null ? await dialog.ShowDialog<bool>(owner) : false;
                
                if (result)
                {
                    ShowStatus("Password changed successfully!", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error: {ex.Message}", false);
            }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            var statusBorder = this.FindControl<Border>("StatusBorder");
            var statusText = this.FindControl<TextBlock>("StatusText");

            if (statusBorder != null && statusText != null)
            {
                statusText.Text = message;
                statusBorder.IsVisible = true;
                
                if (isSuccess)
                {
                    statusBorder.Background = new SolidColorBrush(Color.Parse("#DBEAFE"));
                    statusBorder.BorderBrush = new SolidColorBrush(Color.Parse("#3B82F6"));
                    statusText.Foreground = new SolidColorBrush(Color.Parse("#1E40AF"));
                }
                else
                {
                    statusBorder.Background = new SolidColorBrush(Color.Parse("#FEE2E2"));
                    statusBorder.BorderBrush = new SolidColorBrush(Color.Parse("#EF4444"));
                    statusText.Foreground = new SolidColorBrush(Color.Parse("#991B1B"));
                }

                Task.Delay(5000).ContinueWith(_ =>
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        if (statusBorder != null)
                        {
                            statusBorder.IsVisible = false;
                        }
                    });
                });
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow?.ShowDashboard(sender, e);
        }
    }
}
