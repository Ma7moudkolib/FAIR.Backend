using Bogus;
using FAIR.Domain.Entities.Chat;

namespace FAIR.Tests.Data.Builders
{
    /// <summary>
    /// Builder for creating Message test entities with realistic data.
    /// </summary>
    public class MessageBuilder
    {
        private Message _message;
        private static readonly Faker _faker = new Faker();

        public MessageBuilder()
        {
            _message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Content = _faker.Lorem.Paragraph(),
                SenderId = Guid.NewGuid().ToString(),
                SenderName = _faker.Name.FullName(),
                ReceiverId = Guid.NewGuid().ToString(),
                GroupId = null,
                CreateData = _faker.Date.Recent(),
                IsRead = false
            };
        }

        public MessageBuilder WithSenderId(string senderId)
        {
            _message.SenderId = senderId;
            return this;
        }

        public MessageBuilder WithReceiverId(string receiverId)
        {
            _message.ReceiverId = receiverId;
            return this;
        }

        public MessageBuilder WithContent(string content)
        {
            _message.Content = content;
            return this;
        }

        public MessageBuilder WithSenderName(string senderName)
        {
            _message.SenderName = senderName;
            return this;
        }

        public MessageBuilder WithIsRead(bool isRead)
        {
            _message.IsRead = isRead;
            return this;
        }

        public MessageBuilder WithCreatedDate(DateTime createdDate)
        {
            _message.CreateData = createdDate;
            return this;
        }

        public Message Build()
        {
            return _message;
        }
    }
}
