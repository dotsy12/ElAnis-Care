namespace ElAnis.Entities.Models
{
    /// <summary>
    /// Tracks active SignalR connections for online status
    /// </summary>
    public class UserConnection
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
    public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
}
}