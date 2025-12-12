namespace ElAnis.Entities.DTO.Chat
{
    public class SendMessageDto
    {
        public Guid ChatId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}