namespace LogisticsPlatform.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string BookingReference { get; set; } = string.Empty; // მაგ: BK-9921

        // თუ რეგისტრირებული მომხმარებელია:
        public Guid? PassengerId { get; set; }
        public User? Passenger { get; set; }

        // თუ სტუმარია (Guest Mode):
        public string GuestName { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;

        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;

        public int SeatsBooked { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}
