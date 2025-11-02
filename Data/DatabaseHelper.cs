using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace VitaClinic.WebAPI.Data
{
    public static class DatabaseHelper
    {
        private static string? _dbPath;

        public static string GetDatabasePath()
        {
            if (_dbPath != null) return _dbPath;

            _dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "VitaClinic", 
                "vitaclinic_desktop.db"
            );

            var dirPath = Path.GetDirectoryName(_dbPath);
            if (dirPath != null && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            Console.WriteLine($"Database path: {_dbPath}");
            return _dbPath;
        }

        public static VitaClinicDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<VitaClinicDbContext>();
            optionsBuilder.UseSqlite($"Data Source={GetDatabasePath()}");

            var context = new VitaClinicDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
