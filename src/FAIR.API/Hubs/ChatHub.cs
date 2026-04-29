using FAIR.Application.DTOs.Chat;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FAIR.API.Hubs
{
    [Authorize]
    public class ChatHub(
        IRepositoryManager repositoryManager,
        IServiceManager serviceManager) : Hub
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly IServiceManager _serviceManager = serviceManager;

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                _serviceManager.ConnectionMappingService.AddOrUpdate(userId, Context.ConnectionId);
                await Clients.User(userId).SendAsync("Connected", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _serviceManager.ConnectionMappingService.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(MessageRequest message)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(message.ReceiverId) || string.IsNullOrWhiteSpace(message.Content))
            {
                return;
            }

            var saved = await _serviceManager.ChatService.SavePrivateMessageAsync(senderId, message);
            await Clients.User(message.ReceiverId).SendAsync("ReceivedMessage", saved);
            await Clients.User(senderId).SendAsync("MessageSent", saved);
        }

        public async Task SendMessage(MessageRequest message)
        {
            await SendPrivateMessage(message);
        }

        public async Task Typing(string receiverId, bool isTyping)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId))
            {
                return;
            }

            await Clients.User(receiverId).SendAsync("Typing", new
            {
                SenderId = senderId,
                IsTyping = isTyping,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task MarkAsRead(string otherUserId)
        {
            var readerUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(readerUserId) || string.IsNullOrWhiteSpace(otherUserId))
            {
                return;
            }

            var changed = await _repositoryManager.ChatRepository.MarkConversationAsReadAsync(readerUserId, otherUserId);
            if (changed > 0)
            {
                await Clients.User(otherUserId).SendAsync("ReadReceipt", new
                {
                    ReaderId = readerUserId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async Task<List<Message>> GetConversation(string otherUserId)
        {
            var currentUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId) || string.IsNullOrWhiteSpace(otherUserId))
            {
                return [];
            }

            return await _repositoryManager.ChatRepository.GetPrivateMessagesAsync(currentUserId, otherUserId);
        }

        public async Task<int> GetUnreadCount()
        {
            var currentUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return 0;
            }

            return await _repositoryManager.ChatRepository.GetUnreadMessagesCountAsync(currentUserId);
        }
    }
}
