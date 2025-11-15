using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Services
{
    public static class IdGenerator
    {
        private static readonly Random _random = new Random();

        public static string GenerateId(string prefix = "")
        {
            // Generate a UUID and take first 8 characters + timestamp for uniqueness
            var uuid = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
            var randomDigits = _random.Next(100, 999);

            return $"{prefix}{uuid}{timestamp}{randomDigits}";
        }

        public static string GenerateClientId() => GenerateId("CLI-");
        public static string GenerateAnimalId() => GenerateId("ANI-");
        public static string GenerateAppointmentId() => GenerateId("APT-");
        public static string GenerateInventoryId() => GenerateId("INV-");
        public static string GenerateInvoiceId() => GenerateId("INC-");
        public static string GenerateUserId() => GenerateId("USR-");
        public static string GenerateVeterinarianId() => GenerateId("VET-");
        public static string GenerateMedicalRecordId() => GenerateId("MED-");
    }
}

namespace VitaClinic.WebAPI.Services
{
    public class AuthService
    {
        private readonly VitaClinicDbContext _context;

        public AuthService(VitaClinicDbContext context)
        {
            _context = context;
        }

        public string HashPassword(string password)
        {
            const string salt = "VitaClinic2025SecureSalt";
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = salt + password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var passwordHash = HashPassword(password);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash && u.IsActive);
            
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            
            return user;
        }

        public async Task CreateDefaultAdminIfNotExists()
        {
            if (!await _context.Users.AnyAsync())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    FullName = "System Administrator",
                    Email = "admin@vitaclinic.com",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }
        }
    }
}
