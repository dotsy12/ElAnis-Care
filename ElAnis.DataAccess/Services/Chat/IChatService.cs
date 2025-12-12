using ElAnis.Entities.DTO.Chat;
using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Chat
{
    public interface IChatService
    {
        Task<Response<ChatDto>> CreateOrGetChatAsync(Guid serviceRequestId, ClaimsPrincipal userClaims);
        Task<Response<List<ChatDto>>> GetUserChatsAsync(ClaimsPrincipal userClaims);
        Task<Response<ChatDetailDto>> GetChatMessagesAsync(Guid chatId, int page, int pageSize, ClaimsPrincipal userClaims);
        Task<Response<ChatMessageDto>> SendMessageAsync(SendMessageDto dto, ClaimsPrincipal userClaims);
        Task<Response<string>> MarkMessagesAsReadAsync(Guid chatId, ClaimsPrincipal userClaims);
        Task<bool> IsUserOnlineAsync(string userId);
    }
}