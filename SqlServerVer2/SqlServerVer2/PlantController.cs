using Microsoft.AspNetCore.Mvc;
using SqlServerVer2.Models;

namespace SqlServerVer2
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly PlantContext _context;
        public PlantController(PlantContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public ActionResult<IEnumerable<Plant>> GetPlant()
        {
            return _context.Plant.ToList();
        }

        // GET: api/Plant/1
        [HttpGet("{PlantName}")]
        public async Task<IActionResult> SearchRecords(string PlantName)
        {
            // Call the stored procedure from your Entity Framework Core context
            var matchingRecords = await _context.SearchRecords(PlantName);

            // Check if any records were found
            if (matchingRecords == null || matchingRecords.Count == 0)
            {
                return NotFound("No matching records found.");
            }

            // Return the matching records as a response
            return Ok(matchingRecords);
        }
       /* [HttpGet("{id}")]
        public ActionResult<Plant> GetPlant(int id)
        {
            var plant = _context.Plant.Find(id);
            if (plant == null)
            {
                return NotFound();
            }
            return plant;
        }*/

        // POST: api/plant
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Plant plant)
        {
            if (plant == null)
            {
                return BadRequest();
            }
            _context.Plant.Add(plant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlant), new { id = plant.PlantId }, plant);
        }
        [HttpDelete("{id}")]
        public ActionResult<Plant> DelPlant(int id)
        {
            var plant = _context.Plant.Find(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plant.Remove(plant); // Remove the plant from the context
            _context.SaveChanges(); // Save changes to persist the deletion

            return NoContent(); // Return a 204 No Content response
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PotController : ControllerBase
    {
        private readonly PotContext _context;
        public PotController(PotContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public ActionResult<IEnumerable<Pot>> GetPot()
        {
            return _context.Pot.ToList();
        }

        // GET: api/Plant/1
        [HttpGet("{id}")]
        public ActionResult<Pot> GetPot(int id)
        {
            var plant = _context.Pot.Find(id);
            if (plant == null)
            {
                return NotFound();
            }
            return plant;
        }

        // POST: api/plant
        [HttpPost]
        public async Task<IActionResult> CreatePot(Pot pot)
        {
            if (pot == null)
            {
                return BadRequest();
            }
            Console.WriteLine(pot.PlantName);
            _context.Pot.Add(pot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPot), new { id = pot.PotId }, pot);
        }
        [HttpDelete("{id}")]
        public ActionResult<Pot> DelPlant(int id)
        {
            var pot = _context.Pot.Find(id);
            if (pot == null)
            {
                return NotFound();
            }

            _context.Pot.Remove(pot); // Remove the plant from the context
            _context.SaveChanges(); // Save changes to persist the deletion

            return NoContent(); // Return a 204 No Content response
        }
    }
}
