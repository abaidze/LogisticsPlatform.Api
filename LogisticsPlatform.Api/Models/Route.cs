namespace LogisticsPlatform.Api.Models
{
    public class Route
    {
            public int Id { get; set; }
            public string Origin { get; set; } = string.Empty;        // საიდან
            public string Destination { get; set; } = string.Empty;   // სად
            public DateTime DepartureTime { get; set; }

            public int TotalSeats { get; set; }
            public int AvailableSeats { get; set; }
            public decimal PricePerSeat { get; set; }
            public string Status { get; set; } = "Active";            // Active, Completed, Cancelled

            // Foreign Key
            public Guid ProviderId { get; set; }
            public User Provider { get; set; } = null!;

            public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
