using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticsPlatform.Api.Data;
using LogisticsPlatform.Api.Models;

namespace LogisticsPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // DTO ფრონტიდან შემოსული მონაცემების მისაღებად
        public class BookingRequestDto
        {
            public int RouteId { get; set; }
            public string GuestName { get; set; } = string.Empty;
            public int SeatsBooked { get; set; }
            public decimal TotalAmount { get; set; }
        }

        // GET: api/Bookings
        // აბრუნებს ყველა დაჯავშნილ შეკვეთას ზუსტი მოდელის ველების გამოყენებით
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Route) // ვუერთებთ Route ცხრილს
                .OrderByDescending(b => b.BookedAt)
                .Select(b => new
                {
                    id = b.Id,
                    bookingReference = b.BookingReference,
                    guestName = b.GuestName,
                    fromCity = b.Route != null ? b.Route.Origin : "უცნობია",         // Origin
                    toCity = b.Route != null ? b.Route.Destination : "უცნობია",     // Destination
                    seatsBooked = b.SeatsBooked,
                    totalAmount = b.TotalAmount,
                    status = "აქტიური",
                    date = b.Route != null ? b.Route.DepartureTime.ToString("yyyy-MM-dd") : b.BookedAt.ToString("yyyy-MM-dd") // DepartureTime
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto request)
        {
            if (request == null || request.RouteId <= 0 || request.SeatsBooked <= 0)
            {
                return BadRequest(new { message = "არასწორი დაჯავშნის მონაცემები." });
            }

            // შევამოწმოთ არსებობს თუ არა მარშრუტი
            var route = await _context.Routes.FindAsync(request.RouteId);
            if (route == null)
            {
                return NotFound(new { message = "მარშრუტი ვერ მოიძებნა." });
            }

            // შევამოწმოთ არის თუ არა საკმარისი თავისუფალი ადგილები
            if (route.AvailableSeats < request.SeatsBooked)
            {
                return BadRequest(new { message = "არ არის საკმარისი თავისუფალი ადგილები ამ რეისზე." });
            }

            // შევამციროთ ხელმისაწვდომი ადგილების რაოდენობა მარშრუტზე
            route.AvailableSeats -= request.SeatsBooked;

            // შევქმნათ ახალი ბუქინგი
            var booking = new Booking
            {
                BookingReference = "BK-" + new Random().Next(1000, 9999),
                RouteId = request.RouteId,
                GuestName = request.GuestName,
                SeatsBooked = request.SeatsBooked,
                TotalAmount = request.TotalAmount,
                BookedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "დაჯავშნა წარმატებით განხორციელდა!",
                bookingReference = booking.BookingReference,
                totalAmount = booking.TotalAmount
            });
        }
    }
}