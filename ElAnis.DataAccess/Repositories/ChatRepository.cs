using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        public ChatRepository(AuthContext context) : base(context) { }

        public async Task<Chat?> GetChatByServiceRequestIdAsync(Guid serviceRequestId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.ServiceProvider).ThenInclude(sp => sp.User)
                .Include(c => c.ServiceRequest)
                .FirstOrDefaultAsync(c => c.ServiceRequestId == serviceRequestId);
        }

        public async Task<Chat?> GetChatWithMessagesAsync(Guid chatId, int page = 1, int pageSize = 50)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.ServiceProvider).ThenInclude(sp => sp.User)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Skip((page - 1) * pageSize).Take(pageSize))
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.ServiceProvider).ThenInclude(sp => sp.User)
                .Include(c => c.ServiceRequest)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Chat>> GetProviderChatsAsync(Guid providerId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.ServiceRequest)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => c.ServiceProviderId == providerId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(Guid chatId, string userId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead)
                .CountAsync();
        }
    }
}