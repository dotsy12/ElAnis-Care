using ElAnis.DataAccess.Hubs; // ✅ التغيير الوحيد
using ElAnis.Entities.DTO.Chat;
using ElAnis.Entities.Models;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatService> _logger;
        private readonly ResponseHandler _responseHandler;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(
            IUnitOfWork unitOfWork,
            ILogger<ChatService> logger,
            ResponseHandler responseHandler,
            IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _responseHandler = responseHandler;
            _hubContext = hubContext;
        }

        public async Task<Response<ChatDto>> CreateOrGetChatAsync(Guid serviceRequestId, ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<ChatDto>("User not authenticated");

                var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(serviceRequestId);
                if (serviceRequest == null)
                    return _responseHandler.NotFound<ChatDto>("Service request not found");

                if (serviceRequest.Status < ServiceRequestStatus.Accepted)
                    return _responseHandler.BadRequest<ChatDto>("Chat is only available for accepted requests");

                var isUser = serviceRequest.UserId == userId;
                var providerProfile = await _unitOfWork.ServiceProviderProfiles.FindSingleAsync(p => p.UserId == userId);
                var isProvider = providerProfile != null && serviceRequest.ServiceProviderId == providerProfile.Id;

                if (!isUser && !isProvider)
                    return _responseHandler.Forbidden<ChatDto>("You are not authorized to access this chat");

                var chat = await _unitOfWork.Chats.GetChatByServiceRequestIdAsync(serviceRequestId);

                if (chat == null)
                {
                    // ✅ استخدم الـ Fully Qualified Name
                    chat = new ElAnis.Entities.Models.Chat
                    {
                        ServiceRequestId = serviceRequestId,
                        UserId = serviceRequest.UserId,
                        ServiceProviderId = serviceRequest.ServiceProviderId!.Value,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Chats.AddAsync(chat);
                    await _unitOfWork.CompleteAsync();

                    chat = await _unitOfWork.Chats.GetChatByServiceRequestIdAsync(serviceRequestId);

                    _logger.LogInformation("💬 Chat created for ServiceRequest {RequestId}", serviceRequestId);
                }

                var otherPartyId = isUser ? chat.ServiceProvider.UserId : chat.UserId;
                var otherPartyName = isUser
                    ? $"{chat.ServiceProvider.User.FirstName} {chat.ServiceProvider.User.LastName}"
                    : $"{chat.User.FirstName} {chat.User.LastName}";
                var isOnline = await IsUserOnlineAsync(otherPartyId);

                var chatDto = new ChatDto
                {
                    Id = chat.Id,
                    ServiceRequestId = chat.ServiceRequestId,
                    ServiceRequestDescription = serviceRequest.Description,
                    OtherPartyId = otherPartyId,
                    OtherPartyName = otherPartyName,
                    OtherPartyAvatar = isUser ? chat.ServiceProvider.User.ProfilePicture : chat.User.ProfilePicture,
                    IsOtherPartyOnline = isOnline,
                    LastMessage = chat.Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Message,
                    LastMessageAt = chat.LastMessageAt,
                    UnreadCount = await _unitOfWork.Chats.GetUnreadMessageCountAsync(chat.Id, userId),
                    CreatedAt = chat.CreatedAt
                };

                return _responseHandler.Success(chatDto, "Chat retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateOrGetChatAsync");
                return _responseHandler.ServerError<ChatDto>("Error processing chat");
            }
        }

        public async Task<Response<List<ChatDto>>> GetUserChatsAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<List<ChatDto>>("User not authenticated");

                var isProvider = userClaims.IsInRole("Provider");
                List<ElAnis.Entities.Models.Chat> chats; // ✅ Fully Qualified

                if (isProvider)
                {
                    var providerProfile = await _unitOfWork.ServiceProviderProfiles.FindSingleAsync(p => p.UserId == userId);
                    if (providerProfile == null)
                        return _responseHandler.NotFound<List<ChatDto>>("Provider profile not found");

                    chats = await _unitOfWork.Chats.GetProviderChatsAsync(providerProfile.Id);
                }
                else
                {
                    chats = await _unitOfWork.Chats.GetUserChatsAsync(userId);
                }

                var chatDtos = new List<ChatDto>();

                foreach (var chat in chats)
                {
                    var otherPartyId = isProvider ? chat.UserId : chat.ServiceProvider.UserId;
                    var otherPartyName = isProvider
                        ? $"{chat.User.FirstName} {chat.User.LastName}"
                        : $"{chat.ServiceProvider.User.FirstName} {chat.ServiceProvider.User.LastName}";
                    var isOnline = await IsUserOnlineAsync(otherPartyId);

                    chatDtos.Add(new ChatDto
                    {
                        Id = chat.Id,
                        ServiceRequestId = chat.ServiceRequestId,
                        ServiceRequestDescription = chat.ServiceRequest.Description,
                        OtherPartyId = otherPartyId,
                        OtherPartyName = otherPartyName,
                        OtherPartyAvatar = isProvider ? chat.User.ProfilePicture : chat.ServiceProvider.User.ProfilePicture,
                        IsOtherPartyOnline = isOnline,
                        LastMessage = chat.Messages?.FirstOrDefault()?.Message,
                        LastMessageAt = chat.LastMessageAt,
                        UnreadCount = await _unitOfWork.Chats.GetUnreadMessageCountAsync(chat.Id, userId),
                        CreatedAt = chat.CreatedAt
                    });
                }

                return _responseHandler.Success(chatDtos, "Chats retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserChatsAsync");
                return _responseHandler.ServerError<List<ChatDto>>("Error retrieving chats");
            }
        }

        public async Task<Response<ChatDetailDto>> GetChatMessagesAsync(Guid chatId, int page, int pageSize, ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<ChatDetailDto>("User not authenticated");

                var chat = await _unitOfWork.Chats.GetChatWithMessagesAsync(chatId, page, pageSize);
                if (chat == null)
                    return _responseHandler.NotFound<ChatDetailDto>("Chat not found");

                var isUser = chat.UserId == userId;
                var providerProfile = await _unitOfWork.ServiceProviderProfiles.FindSingleAsync(p => p.UserId == userId);
                var isProvider = providerProfile != null && chat.ServiceProviderId == providerProfile.Id;

                if (!isUser && !isProvider)
                    return _responseHandler.Forbidden<ChatDetailDto>("You are not authorized to access this chat");

                var otherPartyId = isUser ? chat.ServiceProvider.UserId : chat.UserId;
                var otherPartyName = isUser
                    ? $"{chat.ServiceProvider.User.FirstName} {chat.ServiceProvider.User.LastName}"
                    : $"{chat.User.FirstName} {chat.User.LastName}";
                var isOnline = await IsUserOnlineAsync(otherPartyId);

                var messageDtos = new List<ChatMessageDto>();
                foreach (var m in chat.Messages.OrderBy(x => x.SentAt))
                {
                    var sender = await _unitOfWork.Users.GetByIdAsync(m.SenderId);
                    messageDtos.Add(new ChatMessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown",
                        Message = m.Message,
                        IsRead = m.IsRead,
                        SentAt = m.SentAt,
                        ReadAt = m.ReadAt,
                        IsMine = m.SenderId == userId
                    });
                }

                var totalMessages = await _unitOfWork.ChatMessages.CountAsync(m => m.ChatId == chatId);

                var chatDetailDto = new ChatDetailDto
                {
                    Id = chat.Id,
                    ServiceRequestId = chat.ServiceRequestId,
                    OtherPartyId = otherPartyId,
                    OtherPartyName = otherPartyName,
                    OtherPartyAvatar = isUser ? chat.ServiceProvider.User.ProfilePicture : chat.User.ProfilePicture,
                    IsOtherPartyOnline = isOnline,
                    Messages = messageDtos,
                    TotalMessages = totalMessages,
                    Page = page,
                    PageSize = pageSize
                };

                return _responseHandler.Success(chatDetailDto, "Messages retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetChatMessagesAsync");
                return _responseHandler.ServerError<ChatDetailDto>("Error retrieving messages");
            }
        }

        public async Task<Response<ChatMessageDto>> SendMessageAsync(SendMessageDto dto, ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<ChatMessageDto>("User not authenticated");

                var chat = await _unitOfWork.Chats.GetByIdAsync(dto.ChatId);
                if (chat == null)
                    return _responseHandler.NotFound<ChatMessageDto>("Chat not found");

                var isUser = chat.UserId == userId;
                var providerProfile = await _unitOfWork.ServiceProviderProfiles.FindSingleAsync(p => p.UserId == userId);
                var isProvider = providerProfile != null && chat.ServiceProviderId == providerProfile.Id;

                if (!isUser && !isProvider)
                    return _responseHandler.Forbidden<ChatMessageDto>("You are not authorized to send messages in this chat");

                var message = new ChatMessage
                {
                    ChatId = dto.ChatId,
                    SenderId = userId,
                    Message = dto.Message.Trim(),
                    IsRead = false,
                    SentAt = DateTime.UtcNow
                };

                await _unitOfWork.ChatMessages.AddAsync(message);

                chat.LastMessageAt = message.SentAt;
                _unitOfWork.Chats.Update(chat);

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("💬 Message sent in chat {ChatId} by user {UserId}", dto.ChatId, userId);

                var sender = await _unitOfWork.Users.GetByIdAsync(userId);

                var messageDto = new ChatMessageDto
                {
                    Id = message.Id,
                    SenderId = userId,
                    SenderName = $"{sender?.FirstName} {sender?.LastName}",
                    Message = message.Message,
                    IsRead = false,
                    SentAt = message.SentAt,
                    IsMine = true
                };

                await _hubContext.Clients.Group(dto.ChatId.ToString()).SendAsync("ReceiveMessage", messageDto);

                return _responseHandler.Success(messageDto, "Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessageAsync");
                return _responseHandler.ServerError<ChatMessageDto>("Error sending message");
            }
        }

        public async Task<Response<string>> MarkMessagesAsReadAsync(Guid chatId, ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<string>("User not authenticated");

                var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);
                if (chat == null)
                    return _responseHandler.NotFound<string>("Chat not found");

                var isUser = chat.UserId == userId;
                var providerProfile = await _unitOfWork.ServiceProviderProfiles.FindSingleAsync(p => p.UserId == userId);
                var isProvider = providerProfile != null && chat.ServiceProviderId == providerProfile.Id;

                if (!isUser && !isProvider)
                    return _responseHandler.Forbidden<string>("You are not authorized");

                await _unitOfWork.ChatMessages.MarkMessagesAsReadAsync(chatId, userId);
                await _unitOfWork.CompleteAsync();

                await _hubContext.Clients.Group(chatId.ToString()).SendAsync("MessagesRead", userId);

                return _responseHandler.Success<string>(null, "Messages marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MarkMessagesAsReadAsync");
                return _responseHandler.ServerError<string>("Error marking messages as read");
            }
        }

        public async Task<bool> IsUserOnlineAsync(string userId)
        {
            return await _unitOfWork.UserConnections.AnyAsync(c => c.UserId == userId);
        }
    }
}