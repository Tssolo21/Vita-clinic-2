using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class ChangePasswordDialog : Window
    {
        private User? _currentUser;

        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        public ChangePasswordDialog(User currentUser) : this()
        {
            _currentUser = currentUser;
        }

        private string HashPassword(string password)
        {
            const string salt = "VitaClinic2025SecureSalt";
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = salt + password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null) return;

                var currentPasswordBox = this.FindControl<TextBox>("CurrentPasswordBox");
                var newPasswordBox = this.FindControl<TextBox>("NewPasswordBox");
                var confirmPasswordBox = this.FindControl<TextBox>("ConfirmPasswordBox");

                var currentPassword = currentPasswordBox?.Text?.Trim();
                var newPassword = newPasswordBox?.Text?.Trim();
                var confirmPassword = confirmPasswordBox?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(currentPassword) || 
                    string.IsNullOrWhiteSpace(newPassword) || 
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ShowError("All fields are required.");
                    return;
                }

                var currentPasswordHash = HashPassword(currentPassword);
                if (currentPasswordHash != _currentUser.PasswordHash)
                {
                    ShowError("Current password is incorrect.");
                    return;
                }

                if (newPassword.Length < 6)
                {
                    ShowError("New password must be at least 6 characters long.");
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    ShowError("New password and confirmation do not match.");
                    return;
                }

                if (currentPassword == newPassword)
                {
                    ShowError("New password must be different from current password.");
                    return;
                }

                using var context = DatabaseHelper.CreateContext();
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == _currentUser.Id);
                
                if (user != null)
                {
                    user.PasswordHash = HashPassword(newPassword);
                    await context.SaveChangesAsync();
                    
                    _currentUser.PasswordHash = user.PasswordHash;
                    
                    Close(true);
                }
                else
                {
                    ShowError("User not found in database.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error changing password: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            var errorBorder = this.FindControl<Border>("ErrorBorder");
            var errorText = this.FindControl<TextBlock>("ErrorText");

            if (errorBorder != null && errorText != null)
            {
                errorText.Text = message;
                errorBorder.IsVisible = true;
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
