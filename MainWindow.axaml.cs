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
                contentPanel.Children.Add(new Views.ClientsView(this));
            }
        }

        private void ShowAnimals(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new Views.AnimalsView(this));
            }
        }

        private void ShowAppointments(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                contentPanel.Children.Add(new Views.AppointmentsView(this));
            }
        }

        private void ShowRecords(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                var recordsView = new StackPanel { Margin = new Avalonia.Thickness(20) };
                recordsView.Children.Add(new TextBlock { Text = "Medical Records", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold, Margin = new Avalonia.Thickness(0, 0, 0, 20) });
                recordsView.Children.Add(new TextBlock { Text = "View medical records for each animal from the Animals section.", FontSize = 16, Foreground = Avalonia.Media.Brushes.Gray });
                contentPanel.Children.Add(recordsView);
            }
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();
                var settingsView = new StackPanel { Margin = new Avalonia.Thickness(20) };
                settingsView.Children.Add(new TextBlock { Text = "Settings", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold, Margin = new Avalonia.Thickness(0, 0, 0, 20) });
                settingsView.Children.Add(new TextBlock { Text = $"Logged in as: {_currentUser.FullName}", FontSize = 16, Margin = new Avalonia.Thickness(0, 0, 0, 10) });
                settingsView.Children.Add(new TextBlock { Text = $"Role: {_currentUser.Role}", FontSize = 16, Margin = new Avalonia.Thickness(0, 0, 0, 10) });
                settingsView.Children.Add(new TextBlock { Text = $"Email: {_currentUser.Email}", FontSize = 16 });
                contentPanel.Children.Add(settingsView);
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
