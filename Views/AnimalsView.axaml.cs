using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Views
{
    public partial class AnimalsView : UserControl
    {
        private readonly MainWindow _mainWindow;

        public AnimalsView(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            LoadAnimals(null, null);
        }

        private async void LoadAnimals(object? sender, RoutedEventArgs? e)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");

            using var context = new VitaClinicDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
            var animals = await context.Animals.Include(a => a.Client).ToListAsync();

            var grid = this.FindControl<DataGrid>("AnimalsGrid");
            if (grid != null)
            {
                grid.ItemsSource = animals;
            }
        }

        private async void AddAnimal(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAnimalDialog();
            var owner = Window.GetTopLevel(this) as Window;
            var result = owner != null ? await dialog.ShowDialog<Animal?>(owner) : null;
            
            if (result != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
                optionsBuilder.UseSqlite("Data Source=vitaclinic_desktop.db");

                using var context = new VitaClinicDbContext(optionsBuilder.Options);
                context.Database.EnsureCreated();
                result.CreatedAt = DateTime.UtcNow;
                result.UpdatedAt = DateTime.UtcNow;
                context.Animals.Add(result);
                await context.SaveChangesAsync();

                LoadAnimals(null, null);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowDashboard(sender, e);
        }
    }
}
