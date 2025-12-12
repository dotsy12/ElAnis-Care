namespace ElAnis.Entities.DTO.Chat
{
    public class ChatDetailDto
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public string OtherPartyId { get; set; } = string.Empty;
        public string OtherPartyName { get; set; } = string.Empty;
        public string? OtherPartyAvatar { get; set; }
        public bool IsOtherPartyOnline { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new();
        public int TotalMessages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}