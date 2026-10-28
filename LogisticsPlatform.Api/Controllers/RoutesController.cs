using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticsPlatform.Api.Data;

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

        // GET: api/routes
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                // ვქაჩავთ მარშრუტებს და ვარქმევთ სახელს ისე, რომ ფრონტენდმა იდეალურად წაიკითხოს
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
    }
}