using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using System.IO;
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
            Console.WriteLine("Loading dashboard data...");
            using var context = DatabaseHelper.CreateContext();
            
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

        public void ShowDashboard(object sender, RoutedEventArgs e)
        {
            var contentPanel = this.FindControl<StackPanel>("ContentPanel");
            if (contentPanel != null)
            {
                contentPanel.Children.Clear();

                var dashboardPanel = new StackPanel { Margin = new Avalonia.Thickness(0) };

                dashboardPanel.Children.Add(new TextBlock { Text = "Dashboard", FontSize = 28, FontWeight = Avalonia.Media.FontWeight.Bold, Margin = new Avalonia.Thickness(0, 0, 0, 20) });

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var border1 = new Border { Background = new SolidColorBrush(Color.Parse("#F8FAFC")), Padding = new Avalonia.Thickness(20), Margin = new Avalonia.Thickness(5), CornerRadius = new Avalonia.CornerRadius(8) };
                var stack1 = new StackPanel();
                var totalClientsText = new TextBlock { Name = "TotalClients", Text = "0", FontSize = 32, FontWeight = FontWeight.Bold, Foreground = new SolidColorBrush(Color.Parse("#6366F1")) };
                var totalClientsLabel = new TextBlock { Text = "Total Clients", FontSize = 14, Foreground = new SolidColorBrush(Color.Parse("#64748B")) };
                stack1.Children.Add(totalClientsText);
                stack1.Children.Add(totalClientsLabel);
                border1.Child = stack1;
                grid.Children.Add(border1);
                Grid.SetColumn(border1, 0);

                var border2 = new Border { Background = new SolidColorBrush(Color.Parse("#F8FAFC")), Padding = new Avalonia.Thickness(20), Margin = new Avalonia.Thickness(5), CornerRadius = new Avalonia.CornerRadius(8) };
                var stack2 = new StackPanel();
                var totalAnimalsText = new TextBlock { Name = "TotalAnimals", Text = "0", FontSize = 32, FontWeight = FontWeight.Bold, Foreground = new SolidColorBrush(Color.Parse("#10B981")) };
                var totalAnimalsLabel = new TextBlock { Text = "Total Animals", FontSize = 14, Foreground = new SolidColorBrush(Color.Parse("#64748B")) };
                stack2.Children.Add(totalAnimalsText);
                stack2.Children.Add(totalAnimalsLabel);
                border2.Child = stack2;
                grid.Children.Add(border2);
                Grid.SetColumn(border2, 1);

                var border3 = new Border { Background = new SolidColorBrush(Color.Parse("#F8FAFC")), Padding = new Avalonia.Thickness(20), Margin = new Avalonia.Thickness(5), CornerRadius = new Avalonia.CornerRadius(8) };
                var stack3 = new StackPanel();
                var todayAppointmentsText = new TextBlock { Name = "TodayAppointments", Text = "0", FontSize = 32, FontWeight = FontWeight.Bold, Foreground = new SolidColorBrush(Color.Parse("#F59E0B")) };
                var todayAppointmentsLabel = new TextBlock { Text = "Today's Appointments", FontSize = 14, Foreground = new SolidColorBrush(Color.Parse("#64748B")) };
                stack3.Children.Add(todayAppointmentsText);
                stack3.Children.Add(todayAppointmentsLabel);
                border3.Child = stack3;
                grid.Children.Add(border3);
                Grid.SetColumn(border3, 2);

                var border4 = new Border { Background = new SolidColorBrush(Color.Parse("#F8FAFC")), Padding = new Avalonia.Thickness(20), Margin = new Avalonia.Thickness(5), CornerRadius = new Avalonia.CornerRadius(8) };
                var stack4 = new StackPanel();
                var thisWeekText = new TextBlock { Name = "ThisWeek", Text = "0", FontSize = 32, FontWeight = FontWeight.Bold, Foreground = new SolidColorBrush(Color.Parse("#8B5CF6")) };
                var thisWeekLabel = new TextBlock { Text = "This Week", FontSize = 14, Foreground = new SolidColorBrush(Color.Parse("#64748B")) };
                stack4.Children.Add(thisWeekText);
                stack4.Children.Add(thisWeekLabel);
                border4.Child = stack4;
                grid.Children.Add(border4);
                Grid.SetColumn(border4, 3);

                dashboardPanel.Children.Add(grid);

                dashboardPanel.Children.Add(new TextBlock { Text = "Welcome to VitaClinic Desktop Application!", FontSize = 18, Margin = new Avalonia.Thickness(0, 30, 0, 10) });
                dashboardPanel.Children.Add(new TextBlock { Text = "Use the sidebar to navigate through different sections.", FontSize = 14, Foreground = new SolidColorBrush(Color.Parse("#64748B")) });

                contentPanel.Children.Add(dashboardPanel);
            }
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
                contentPanel.Children.Add(new Views.SettingsView(this, _currentUser));
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
