using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;

namespace VitaClinic.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly VitaClinicDbContext _context;

        public AnalyticsController(VitaClinicDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            var totalClients = await _context.Clients.CountAsync();
            var activeClients = await _context.Clients.CountAsync(c => c.Status == Models.ClientStatus.Active);
            var totalAnimals = await _context.Animals.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();
            
            var today = DateTime.Today;
            var todayAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDateTime.Date == today);
            
            var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDateTime >= thisWeekStart && a.AppointmentDateTime < thisWeekStart.AddDays(7));

            var thisMonthStart = new DateTime(today.Year, today.Month, 1);
            var monthAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDateTime >= thisMonthStart && a.AppointmentDateTime < thisMonthStart.AddMonths(1));

            var invoices = await _context.Invoices.ToListAsync();
            var totalRevenue = invoices.Sum(i => i.PaidAmount);
            var pendingRevenue = invoices
                .Where(i => i.Status == Models.InvoiceStatus.Pending)
                .Sum(i => i.TotalAmount - i.PaidAmount);

            return new
            {
                totalClients,
                activeClients,
                totalAnimals,
                totalAppointments,
                todayAppointments,
                weekAppointments,
                monthAppointments,
                totalRevenue,
                pendingRevenue
            };
        }

        [HttpGet("appointments-by-type")]
        public async Task<ActionResult<object>> GetAppointmentsByType()
        {
            var appointmentsByType = await _context.Appointments
                .GroupBy(a => a.AppointmentType)
                .Select(g => new
                {
                    type = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            return appointmentsByType;
        }

        [HttpGet("appointments-by-status")]
        public async Task<ActionResult<object>> GetAppointmentsByStatus()
        {
            var appointmentsByStatus = await _context.Appointments
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count()
                })
                .ToListAsync();

            return appointmentsByStatus;
        }

        [HttpGet("species-distribution")]
        public async Task<ActionResult<object>> GetSpeciesDistribution()
        {
            var speciesDistribution = await _context.Animals
                .GroupBy(a => a.Species)
                .Select(g => new
                {
                    species = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            return speciesDistribution;
        }

        [HttpGet("monthly-appointments")]
        public async Task<ActionResult<object>> GetMonthlyAppointments()
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            
            var monthlyAppointments = await _context.Appointments
                .Where(a => a.AppointmentDateTime >= sixMonthsAgo)
                .GroupBy(a => new { a.AppointmentDateTime.Year, a.AppointmentDateTime.Month })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    count = g.Count()
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.month)
                .ToListAsync();

            return monthlyAppointments;
        }

        [HttpGet("revenue-by-month")]
        public async Task<ActionResult<object>> GetRevenueByMonth()
        {
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= sixMonthsAgo)
                .ToListAsync();
                
            var monthlyRevenue = invoices
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    revenue = g.Sum(i => i.PaidAmount)
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.month)
                .ToList();

            return monthlyRevenue;
        }
    }
}
