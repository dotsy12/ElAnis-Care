using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        Task MarkMessagesAsReadAsync(Guid chatId, string userId);
    }
}