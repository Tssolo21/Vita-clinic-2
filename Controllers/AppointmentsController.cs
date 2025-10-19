using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly VitaClinicDbContext _context;

        public AppointmentsController(VitaClinicDbContext context)
        {
            _context = context;
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