using ElAnis.DataAccess.Services.Chat;
using ElAnis.Entities.DTO.Chat;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Chat Controller - Real-time messaging between users and providers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ResponseHandler _responseHandler;

        public ChatController(IChatService chatService, ResponseHandler responseHandler)
        {
            _chatService = chatService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// Create or get chat for a service request (Available after request is Accepted)
        /// </summary>
        /// <param name="serviceRequestId">Service Request ID</param>
        /// <returns>Chat details</returns>
        /// <response code="200">Chat retrieved successfully</response>
        /// <response code="400">Chat only available for accepted requests</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Not authorized to access this chat</response>
        /// <response code="404">Service request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("create-or-get/{serviceRequestId}")]
        [ProducesResponseType(typeof(Response<ChatDto>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CreateOrGetChat(Guid serviceRequestId)
        {
            if (serviceRequestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid service request ID"));

            var response = await _chatService.CreateOrGetChatAsync(serviceRequestId, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all chats for the current user (User or Provider)
        /// </summary>
        /// <returns>List of user's chats with last message and unread count</returns>
        /// <response code="200">Chats retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("my-chats")]
        [ProducesResponseType(typeof(Response<List<ChatDto>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetMyChats()
        {
            var response = await _chatService.GetUserChatsAsync(User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get messages for a specific chat with pagination
        /// </summary>
        /// <param name="chatId">Chat ID</param>
        /// <param name="page">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 50, max 100)</param>
        /// <returns>Chat messages with pagination</returns>
        /// <response code="200">Messages retrieved successfully</response>
        /// <response code="400">Invalid chat ID or pagination parameters</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Not authorized to access this chat</response>
        /// <response code="404">Chat not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{chatId}/messages")]
        [ProducesResponseType(typeof(Response<ChatDetailDto>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetChatMessages(
            Guid chatId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (chatId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid chat ID"));

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            var response = await _chatService.GetChatMessagesAsync(chatId, page, pageSize, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Send a message in a chat
        /// </summary>
        /// <param name="dto">Message data</param>
        /// <returns>Sent message</returns>
        /// <response code="200">Message sent successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Not authorized to send messages in this chat</response>
        /// <response code="404">Chat not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("send-message")]
        [ProducesResponseType(typeof(Response<ChatMessageDto>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            if (dto.ChatId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid chat ID"));

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(_responseHandler.BadRequest<object>("Message cannot be empty"));

            var response = await _chatService.SendMessageAsync(dto, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Mark all unread messages in a chat as read
        /// </summary>
        /// <param name="chatId">Chat ID</param>
        /// <returns>Confirmation</returns>
        /// <response code="200">Messages marked as read</response>
        /// <response code="400">Invalid chat ID</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Not authorized to access this chat</response>
        /// <response code="404">Chat not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{chatId}/mark-read")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> MarkMessagesAsRead(Guid chatId)
        {
            if (chatId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid chat ID"));

            var response = await _chatService.MarkMessagesAsReadAsync(chatId, User);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
