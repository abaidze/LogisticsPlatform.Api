namespace LogisticsPlatform.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Type { get; set; } = "TopUp"; // TopUp, Payment, Refund
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
