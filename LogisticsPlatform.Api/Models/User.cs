namespace LogisticsPlatform.Api.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // RBAC: "Driver", "Company", "Individual"
        public string Role { get; set; } = "Individual";

        public decimal Balance { get; set; } = 0.0m;
        public string? CompanyName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Route> CreatedRoutes { get; set; } = new List<Route>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
