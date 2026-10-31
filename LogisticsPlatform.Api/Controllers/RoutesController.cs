using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticsPlatform.Api.Data;
using LogisticsPlatform.Api.Models;
using Route = LogisticsPlatform.Api.Models.Route;

namespace LogisticsPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoutesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Routes
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var routes = await _context.Routes
                    .Where(r => r.Status == "Active")
                    .Select(r => new
                    {
                        id = r.Id,
                        fromCity = r.Origin,
                        toCity = r.Destination,
                        price = r.PricePerSeat,
                        date = r.DepartureTime.ToString("yyyy-MM-dd"),
                        availableSeats = r.AvailableSeats,
                        driverName = r.Provider != null ? r.Provider.FullName : "უცნობი მძღოლი"
                    })
                    .ToListAsync();

                return Ok(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ბაზის შეცდომა", details = ex.Message });
            }
        }

        public class CreateRouteDto
        {
            public string FromCity { get; set; } = string.Empty;
            public string ToCity { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateTime Date { get; set; }
            public int AvailableSeats { get; set; }
            public string DriverName { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "არასწორი მონაცემები" });
            }

            try
            {
                var driverNameTrimmed = dto.DriverName?.Trim();
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.FullName.ToLower() == driverNameTrimmed.ToLower());

                if (user == null)
                {
                    user = new User
                    {
                        FullName = string.IsNullOrEmpty(driverNameTrimmed) ? "უცნობი მძღოლი" : driverNameTrimmed,
                        Email = $"{Guid.NewGuid()}@logistics.ge", // დროებითი უნიკალური იმეილი
                        PasswordHash = "N/A"
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var parsedDate = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);

                var route = new Route
                {
                    Origin = dto.FromCity,
                    Destination = dto.ToCity,
                    PricePerSeat = dto.Price,
                    DepartureTime = parsedDate,
                    AvailableSeats = dto.AvailableSeats,
                    TotalSeats = dto.AvailableSeats,
                    Status = "Active",
                    ProviderId = user.Id 
                };

                _context.Routes.Add(route);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "მარშრუტი წარმატებით დაემატა!",
                    route = new
                    {
                        id = route.Id,
                        fromCity = route.Origin,
                        toCity = route.Destination,
                        price = route.PricePerSeat,
                        date = route.DepartureTime.ToString("yyyy-MM-dd"),
                        availableSeats = route.AvailableSeats,
                        driverName = user.FullName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ბაზაში შენახვის შეცდომა", details = ex.Message });
            }
        }
    }
}