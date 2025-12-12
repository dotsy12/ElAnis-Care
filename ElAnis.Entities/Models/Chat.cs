using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.Entities.Models
{
    /// <summary>
    /// Chat conversation between user and provider for a specific service request
    /// </summary>
    public class Chat
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public Guid ServiceProviderId { get; set; }
        public ServiceProviderProfile ServiceProvider { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMessageAt { get; set; }

        // Navigation
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}