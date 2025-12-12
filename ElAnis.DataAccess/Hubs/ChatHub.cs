using ElAnis.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IUnitOfWork unitOfWork, ILogger<ChatHub> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await base.OnConnectedAsync();
                return;
            }

            var connection = new UserConnection
            {
                UserId = userId,
                ConnectionId = Context.ConnectionId,
                ConnectedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            await _unitOfWork.UserConnections.AddAsync(connection);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("✅ User {UserId} connected with ConnectionId {ConnectionId}", userId, Context.ConnectionId);

            await Clients.Others.SendAsync("UserOnline", userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var connection = await _unitOfWork.UserConnections
                    .FindSingleAsync(c => c.ConnectionId == Context.ConnectionId);

                if (connection != null)
                {
                    _unitOfWork.UserConnections.Delete(connection);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("❌ User {UserId} disconnected", userId);

                    var hasOtherConnections = await _unitOfWork.UserConnections
                        .AnyAsync(c => c.UserId == userId);

                    if (!hasOtherConnections)
                    {
                        await Clients.Others.SendAsync("UserOffline", userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            _logger.LogInformation("👥 ConnectionId {ConnectionId} joined chat {ChatId}", Context.ConnectionId, chatId);
        }

        public async Task LeaveChat(string chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
            _logger.LogInformation("👋 ConnectionId {ConnectionId} left chat {ChatId}", Context.ConnectionId, chatId);
        }

        public async Task SendTyping(string chatId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Clients.OthersInGroup(chatId).SendAsync("UserTyping", userId);
        }

        public async Task StopTyping(string chatId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Clients.OthersInGroup(chatId).SendAsync("UserStoppedTyping", userId);
        }
    }
}