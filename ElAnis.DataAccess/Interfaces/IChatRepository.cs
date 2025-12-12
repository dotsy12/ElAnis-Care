using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<Chat?> GetChatByServiceRequestIdAsync(Guid serviceRequestId);
        Task<Chat?> GetChatWithMessagesAsync(Guid chatId, int page = 1, int pageSize = 50);
        Task<List<Chat>> GetUserChatsAsync(string userId);
        Task<List<Chat>> GetProviderChatsAsync(Guid providerId);
        Task<int> GetUnreadMessageCountAsync(Guid chatId, string userId);
    }
}