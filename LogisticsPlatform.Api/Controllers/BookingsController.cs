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
                BookingReference = "BK-" + new Random().Next(1000, 9999), // გენერირდება უნიკალური რეფერენსი
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