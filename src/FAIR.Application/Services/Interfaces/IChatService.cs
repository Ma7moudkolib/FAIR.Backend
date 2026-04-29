using FAIR.Application.DTOs.Chat;
using FAIR.Domain.Entities.Chat;

namespace FAIR.Application.Services.Interfaces
{
    public interface IChatService
    {
        Task<Message> SavePrivateMessageAsync(string senderId, MessageRequest message, CancellationToken cancellationToken = default);
    }
}
