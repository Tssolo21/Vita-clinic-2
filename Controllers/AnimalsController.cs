using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaClinic.WebAPI.Data;
using VitaClinic.WebAPI.Models;

namespace VitaClinic.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly VitaClinicDbContext _context;

        public AnimalsController(VitaClinicDbContext context)
        {
            _context = context;
        }

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return await _context.Animals.Include(a => a.Client).ToListAsync();
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            var animal = await _context.Animals.Include(a => a.Client).FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        // GET: api/Animals/search/max
        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<Animal>>> SearchAnimals(string searchTerm)
        {
            return await _context.Animals
                .Include(a => a.Client)
                .Where(a => a.Name.Contains(searchTerm) ||
                           a.Species.Contains(searchTerm) ||
                           a.Breed.Contains(searchTerm) ||
                           a.Client.FirstName.Contains(searchTerm) ||
                           a.Client.LastName.Contains(searchTerm))
                .ToListAsync();
        }

        // GET: api/Animals/client/5
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimalsByClient(int clientId)
        {
            return await _context.Animals
                .Include(a => a.Client)
                .Where(a => a.ClientId == clientId)
                .ToListAsync();
        }

        // PUT: api/Animals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, Animal animal)
        {
            if (id != animal.Id)
            {
                return BadRequest();
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
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

        // POST: api/Animals
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
        {
            animal.CreatedAt = DateTime.UtcNow;
            animal.UpdatedAt = DateTime.UtcNow;

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimal", new { id = animal.Id }, animal);
        }

        // DELETE: api/Animals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}