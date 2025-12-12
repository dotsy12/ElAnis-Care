using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.Entities.Models
{
    /// <summary>
    /// Individual message in a chat
    /// </summary>
    public class ChatMessage
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = null!;

        public string SenderId { get; set; } = null!;
        public User Sender { get; set; } = null!;

        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
    }
}