using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class ChatRepository(dbContext context) : IChatRepository
    {
        private readonly dbContext _context = context;

        public async Task<Message> SaveMessageAsync(Message message)
        {
            var sender = await _context.Users.FindAsync(message.SenderId);
            if (sender != null)
            {
                message.SenderName = sender.UserName;
            }

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetPrivateMessagesAsync(string userId1, string userId2)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId != null &&
                            ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                             (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .OrderBy(m => m.CreateData)
                .ToListAsync();
        }

        public async Task<int> MarkConversationAsReadAsync(string readerUserId, string otherUserId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == readerUserId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Count == 0)
            {
                return 0;
            }

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(string userId)
        {
            return await _context.Messages.CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }
    }
}
