using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;
using VitaClinic.WebAPI.Services;

namespace VitaClinic.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly VitaClinicDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public AppointmentsController(VitaClinicDbContext context, IEmailService emailService, ISmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments.Include(a => a.Animal).ThenInclude(a => a.Client).ToListAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.Include(a => a.Animal).ThenInclude(a => a.Client).FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // GET: api/Appointments/date/2024-10-20
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByDate(DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.Animal)
                .ThenInclude(a => a.Client)
                .Where(a => a.AppointmentDateTime.Date == date.Date)
                .ToListAsync();
        }

        // GET: api/Appointments/today
        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetTodayAppointments()
        {
            var today = DateTime.Today;
            return await _context.Appointments
                .Include(a => a.Animal)
                .ThenInclude(a => a.Client)
                .Where(a => a.AppointmentDateTime.Date == today)
                .ToListAsync();
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Load the animal and client details for notifications
            var appointmentWithDetails = await _context.Appointments
                .Include(a => a.Animal)
                    .ThenInclude(a => a!.Client)
                .FirstOrDefaultAsync(a => a.Id == appointment.Id);

            if (appointmentWithDetails?.Animal?.Client != null)
            {
                var client = appointmentWithDetails.Animal.Client;
                var animal = appointmentWithDetails.Animal;

                // Check clinic settings for notification preferences
                var settings = await _context.ClinicSettings.FirstOrDefaultAsync();
                
                // Send email notification if enabled
                if (settings?.EmailNotificationsEnabled == true && !string.IsNullOrEmpty(client.Email))
                {
                    _ = _emailService.SendAppointmentConfirmationAsync(
                        client.Email,
                        $"{client.FirstName} {client.LastName}",
                        animal.Name ?? "your pet",
                        appointmentWithDetails.AppointmentDateTime,
                        appointmentWithDetails.AppointmentType ?? "checkup"
                    );
                }

                // Send SMS notification if enabled
                if (settings?.SmsNotificationsEnabled == true && !string.IsNullOrEmpty(client.Phone))
                {
                    _ = _smsService.SendAppointmentReminderSmsAsync(
                        client.Phone,
                        client.FirstName ?? "Client",
                        animal.Name ?? "your pet",
                        appointmentWithDetails.AppointmentDateTime
                    );
                }
            }

            return CreatedAtAction("GetAppointment", new { id = appointment.Id }, appointment);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}