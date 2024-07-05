    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLserverFinale.Models;

namespace SQLserverFinale
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

        // GET: api/Pot
        [HttpGet]
        public ActionResult<IEnumerable<Pot>> GetPot()
        {
            return _context.Pot.ToList();
        }

        // GET: api/Pot/1/1
        [HttpGet("{UserId}/{PotId}")]
        public async Task<ActionResult<Pot>> GetPotAsync(int UserId,int PotId)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId); 
            if (pot == null)
            {
                return NotFound();
            }
            return pot;
        }

        // POST: api/Pot
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
        [HttpPut("{UserId}/{PotId}/{PlantName}")]
        public async Task<ActionResult<Pot>> UpdateUserAsync(int UserId,int PotId , string PlantName)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (pot == null)
            {
                NotFound();
            }

            pot.PlantName = PlantName;

            _context.Pot.Update(pot);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        // PUT: api/Pot/{UserId}/{PotId}
        [HttpPut("{UserId}/{PotId}")]
        public async Task<IActionResult> UpdatePotAsync(int UserId, int PotId, Pot updatedPot)
        {
            if (UserId != updatedPot.UserId || PotId != updatedPot.PotId)
            {
                return BadRequest();
            }

            var existingPot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (existingPot == null)
            {
                return NotFound();
            }

            // Update all properties
            existingPot.PotName = updatedPot.PotName;
            existingPot.PotType = updatedPot.PotType;
            existingPot.PlantName = updatedPot.PlantName;
            existingPot.PumpStatus = updatedPot.PumpStatus;
            existingPot.GreenHouseStatus = updatedPot.GreenHouseStatus;
            existingPot.GreenHouseTemperature = updatedPot.GreenHouseTemperature;
            existingPot.GreenHouseHumidity = updatedPot.GreenHouseHumidity;
            existingPot.GreenHousePressure = updatedPot.GreenHousePressure;
            existingPot.PotPotassium = updatedPot.PotPotassium;
            existingPot.PotPhospor = updatedPot.PotPhospor;
            existingPot.PotNitrogen = updatedPot.PotNitrogen;

            _context.Pot.Update(existingPot);
            await _context.SaveChangesAsync();

            return NoContent();
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
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        public UserController(UserContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUser()
        {
            return _context.User.ToList();
        }

        // GET: api/User/1
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            Console.WriteLine(user.UserId);
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        [HttpPut("{UserId}/{UserPassword}")]
        public async Task<ActionResult<User>> UpdateUserAsync(int UserId, string UserPassword)
        {
            var user = _context.User.Find(UserId);
            if(user == null)
            {
                NotFound();
            }

            user.UserPassword = UserPassword;

            _context.User.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
            
        }

        [HttpDelete("{id}")]
        
        public ActionResult<User> DelPlant(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user); // Remove the plant from the context
            _context.SaveChanges(); // Save changes to persist the deletion

            return NoContent(); // Return a 204 No Content response
        }
    }
}
