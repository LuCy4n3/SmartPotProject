    using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLserverFinale.Models;
using System.Diagnostics;

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
        public ActionResult<IEnumerable<Plant>> GetPlants()
        {
            return _context.Plant.ToList();
        }

        // GET: api/Plant/1
        [HttpGet("{PlantName}")]
        public async Task<IActionResult> SearchPlantName(string PlantName)
        {
            
            var plant = await _context.SearchRecords(PlantName);

            // Check if any records were found
            if (plant == null || plant.Count == 0)
            {
                return NotFound("No matching records found.");
            }

            
            return Ok(plant);
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
        [HttpPost] // create a new plant
        public async Task<IActionResult> CreatePlant(Plant plant)
        {
            if (plant == null)
            {
                return BadRequest();
            }
            _context.Plant.Add(plant);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlants), new { id = plant.PlantId }, plant);
        }
        [HttpDelete("{id}")] // id has to be plantName
        public ActionResult<Plant> DelPlant(string id)
        {
            var plant = _context.Plant.Find(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plant.Remove(plant); 
            _context.SaveChanges(); 

            return NoContent(); 
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
        public ActionResult<IEnumerable<Pot>> GetPots() // get all pots
        {
            return _context.Pot.ToList();
        }

        // GET: api/Pot/1/1
        [HttpGet("{UserId}/{PotId}")] // get specific pot 
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
        public async Task<IActionResult> CreatePot(Pot pot) // create new pot
        {
            if (pot == null)
            {
                return BadRequest();
            }
            Console.WriteLine(pot.PlantName);
            _context.Pot.Add(pot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPots), new { id = pot.PotId }, pot);
        }
        [HttpPut("{UserId}/{PotId}/ExtTemp:{temp}&&ExtHum:{hum}")] // update ext temp and hum used with pcb
        public async Task<ActionResult<Pot>> UpdatePlantExtTempAndHumAsync(int UserId, int PotId, double temp, double hum)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (pot == null)
            {
                NotFound();
            }

            pot.GreenHouseTemperature = temp;

            pot.GreenHouseHumidity = hum;

            _context.Pot.Update(pot);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        [HttpPut("{UserId}/{PotId}/PlantName:{PlantName}")] // change plant name field
        public async Task<ActionResult<Pot>> UpdatePlantNameAsync(int UserId,int PotId , string PlantName)
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
        [HttpPut("{UserId}/{PotId}/PumpStatus:{PumpStatus}")] // turn on pump
        public async Task<ActionResult<Pot>> UpdatePumpStatusAsync(int UserId, int PotId, bool PumpStatus)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (pot == null)
            {
                NotFound();
            }

            pot.PumpStatus = PumpStatus;

            _context.Pot.Update(pot);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        [HttpPut("{UserId}/{PotId}/HasCamera:{HasCamera}")] // used like type field(add on)
        public async Task<ActionResult<Pot>> UpdateCameraAsync(int UserId, int PotId, bool HasCamera)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (pot == null)
            {
                NotFound();
            }

            pot.HasCamera = HasCamera;

            _context.Pot.Update(pot);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        [HttpPut("{UserId}/{PotId}/PictReq:{PictReq}")] // used with the camera add on
        public async Task<ActionResult<Pot>> UpdatePictReqAsync(int UserId, int PotId, bool PictReq)
        {
            var pot = await _context.Pot.FirstOrDefaultAsync(p => p.UserId == UserId && p.PotId == PotId);
            if (pot == null)
            {
                NotFound();
            }
            if(PictReq!=null && pot!=null)
                pot.PictReq = PictReq;

            _context.Pot.Update(pot);
            await _context.SaveChangesAsync();

            return NoContent();

        }
        // PUT: api/Pot/{UserId}/{PotId}
        [HttpPut("{UserId}/{PotId}")]
        public async Task<IActionResult> UpdatePotAsync(int UserId, int PotId, Pot updatedPot) // used only for debugging 
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
            if (updatedPot.PotName != null) existingPot.PotName = updatedPot.PotName;
            if (updatedPot.PotType != null) existingPot.PotType = updatedPot.PotType;
            if (updatedPot.PlantName != null) existingPot.PlantName = updatedPot.PlantName;
            if (updatedPot.PumpStatus != null) existingPot.PumpStatus = updatedPot.PumpStatus;
            if (updatedPot.GreenHouseStatus != null) existingPot.GreenHouseStatus = updatedPot.GreenHouseStatus;
            if (updatedPot.GreenHouseTemperature.HasValue) existingPot.GreenHouseTemperature = updatedPot.GreenHouseTemperature.Value;
            if (updatedPot.GreenHouseHumidity.HasValue) existingPot.GreenHouseHumidity = updatedPot.GreenHouseHumidity.Value;
            if (updatedPot.GreenHousePressure.HasValue) existingPot.GreenHousePressure = updatedPot.GreenHousePressure.Value;
            if (updatedPot.PotPotassium.HasValue) existingPot.PotPotassium = updatedPot.PotPotassium.Value;
            if (updatedPot.PotPhospor.HasValue) existingPot.PotPhospor = updatedPot.PotPhospor.Value;
            if (updatedPot.PotNitrogen.HasValue) existingPot.PotNitrogen = updatedPot.PotNitrogen.Value;

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

            _context.Pot.Remove(pot); 
            _context.SaveChanges(); 

            return NoContent(); 
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
        public ActionResult<IEnumerable<User>> GetUsers()
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
        [HttpPut("{UserId}/{UserName}/{UserPassword}")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            Console.WriteLine(user.UserId);
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsers), new { id = user.UserId }, user);
        }
        [HttpPost("{UserId}/{UserPassword}")]
        public async Task<ActionResult<User>> UpdateUserAsync(int UserId,string UserName, string UserPassword)
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
    [Route("api/[controller]")] // for uploading or getting the uploaded images work in progress
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image file was uploaded.");

            var directoryPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var filePath = Path.Combine("wwwroot/images", image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { FilePath = filePath });
        }
        // GET: api/image/{imageName}
        [HttpGet("{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var filePath = Path.Combine("wwwroot", "images", imageName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

           var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg");
        }
    }
    /*[ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        [HttpGet("start-stream")]
        public IActionResult StartStream()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = "-i udp://0.0.0.0:3000 -c:v copy -f hls -hls_time 2 -hls_list_size 5 -hls_flags delete_segments wwwroot/video/stream.m3u8",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process ffmpegProcess = Process.Start(processInfo);
            return Ok("Stream started");
        }
    }*/
}
