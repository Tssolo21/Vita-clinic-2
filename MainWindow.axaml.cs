using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            
            var welcomeText = this.FindControl<TextBlock>("WelcomeText");
            if (welcomeText != null)
            {
                welcomeText.Text = $"Welcome, {_currentUser.FullName}";
            }
            
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");
            
            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            
            var totalClients = await context.Clients.CountAsync();
            var totalAnimals = await context.Animals.CountAsync();
            var today = DateTime.Today;
            var todayAppointments = await context.Appointments
                .Where(a => a.AppointmentDateTime.Date == today)
                .CountAsync();
            var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
            var thisWeekAppointments = await context.Appointments
                .Where(a => a.AppointmentDateTime >= thisWeekStart && a.AppointmentDateTime < thisWeekStart.AddDays(7))
                .CountAsync();
            
            var totalClientsText = this.FindControl<TextBlock>("TotalClients");
            var totalAnimalsText = this.FindControl<TextBlock>("TotalAnimals");
            var todayAppointmentsText = this.FindControl<TextBlock>("TodayAppointments");
            var thisWeekText = this.FindControl<TextBlock>("ThisWeek");
            
            if (totalClientsText != null) totalClientsText.Text = totalClients.ToString();
            if (totalAnimalsText != null) totalAnimalsText.Text = totalAnimals.ToString();
            if (todayAppointmentsText != null) todayAppointmentsText.Text = todayAppointments.ToString();
            if (thisWeekText != null) thisWeekText.Text = thisWeekAppointments.ToString();
        }

        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private void ShowClients(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new TextBlock { Text = "Clients Management", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold });
                contentPanel.Children.Add(new TextBlock { Text = "Client management features coming soon...", FontSize = 16, Margin = new Avalonia.Thickness(0, 20, 0, 0) });
            }
        }

        private void ShowAnimals(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new TextBlock { Text = "Animals Management", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold });
                contentPanel.Children.Add(new TextBlock { Text = "Animal management features coming soon...", FontSize = 16, Margin = new Avalonia.Thickness(0, 20, 0, 0) });
            }
        }

        private void ShowAppointments(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new TextBlock { Text = "Appointments", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold });
                contentPanel.Children.Add(new TextBlock { Text = "Appointment management features coming soon...", FontSize = 16, Margin = new Avalonia.Thickness(0, 20, 0, 0) });
            }
        }

        private void ShowRecords(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new TextBlock { Text = "Medical Records", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold });
                contentPanel.Children.Add(new TextBlock { Text = "Medical records features coming soon...", FontSize = 16, Margin = new Avalonia.Thickness(0, 20, 0, 0) });
            }
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new TextBlock { Text = "Settings", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold });
                contentPanel.Children.Add(new TextBlock { Text = "Settings features coming soon...", FontSize = 16, Margin = new Avalonia.Thickness(0, 20, 0, 0) });
            }
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
