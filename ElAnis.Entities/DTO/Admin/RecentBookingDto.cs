namespace ElAnis.Entities.DTO.Admin
{
    public class RecentBookingDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Shift { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty; // "completed", "pending", "cancelled"
        public string CategoryName { get; set; } = string.Empty;
    }
}