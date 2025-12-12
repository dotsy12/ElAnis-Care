namespace ElAnis.Entities.DTO.Chat
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public string ServiceRequestDescription { get; set; } = string.Empty;
        public string OtherPartyId { get; set; } = string.Empty;
        public string OtherPartyName { get; set; } = string.Empty;
        public string? OtherPartyAvatar { get; set; }
        public bool IsOtherPartyOnline { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}