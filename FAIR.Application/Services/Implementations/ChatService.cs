using FAIR.Application.DTOs.Chat;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Interfaces;

namespace FAIR.Application.Services.Implementations
{
    public class ChatService(IRepositoryManager repositoryManager) : IChatService
    {
        public async Task<Message> SavePrivateMessageAsync(string senderId, MessageRequest message, CancellationToken cancellationToken = default)
        {
            var sender = await repositoryManager.UserRepository.GetAnyByIdAsync(senderId, cancellationToken);
            var dbMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = message.ReceiverId,
                SenderName = sender?.UserName,
                IsRead = false,
                Content = message.Content,
                CreateData = DateTime.UtcNow
            };

            return await repositoryManager.ChatRepository.SaveMessageAsync(dbMessage);
        }
    }
}
