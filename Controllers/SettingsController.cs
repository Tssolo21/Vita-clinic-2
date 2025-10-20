using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly VitaClinicDbContext _context;

        public SettingsController(VitaClinicDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ClinicSettings>> GetSettings()
        {
            var settings = await _context.ClinicSettings.FirstOrDefaultAsync();
            
            if (settings == null)
            {
                // Create default settings if none exist
                settings = new ClinicSettings
                {
                    ClinicName = "VitaClinic Veterinary Hospital",
                    Address = "123 Pet Care Street",
                    Phone = "(555) 123-4567",
                    Email = "info@vitaclinic.com",
                    EmailNotificationsEnabled = false,
                    SmsNotificationsEnabled = false,
                    AppointmentReminderHours = 24,
                    BusinessHours = "Mon-Fri: 8:00 AM - 6:00 PM, Sat: 9:00 AM - 2:00 PM",
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.ClinicSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return settings;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings(ClinicSettings settings)
        {
            var existingSettings = await _context.ClinicSettings.FirstOrDefaultAsync();
            
            if (existingSettings == null)
            {
                settings.UpdatedAt = DateTime.UtcNow;
                _context.ClinicSettings.Add(settings);
            }
            else
            {
                existingSettings.ClinicName = settings.ClinicName;
                existingSettings.Address = settings.Address;
                existingSettings.Phone = settings.Phone;
                existingSettings.Email = settings.Email;
                existingSettings.Website = settings.Website;
                existingSettings.Logo = settings.Logo;
                existingSettings.EmailNotificationsEnabled = settings.EmailNotificationsEnabled;
                existingSettings.SmsNotificationsEnabled = settings.SmsNotificationsEnabled;
                existingSettings.AppointmentReminderHours = settings.AppointmentReminderHours;
                existingSettings.BusinessHours = settings.BusinessHours;
                existingSettings.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
