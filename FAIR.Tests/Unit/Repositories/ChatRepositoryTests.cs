using FluentAssertions;
using FAIR.Domain.Entities.Chat;
using FAIR.Infrastructure.Repository;
using FAIR.Tests.Common;
using FAIR.Tests.Data.Builders;
using FAIR.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FAIR.Tests.Unit.Repositories
{
    /// <summary>
    /// Unit tests for ChatRepository.
    /// Tests message operations, conversation filtering, and read status management.
    /// </summary>
    public class ChatRepositoryTests : RepositoryTestBase
    {
        private ChatRepository _repository;

        public ChatRepositoryTests(InMemoryDbContextFixture dbFixture, MapperFixture mapperFixture)
            : base(dbFixture, mapperFixture)
        {
        }

        private ChatRepository GetRepository() => new ChatRepository(DbContext);

        [Fact]
        public async Task SaveMessageAsync_WithValidMessage_ShouldPersistMessage()
        {
            // Arrange
            _repository = GetRepository();
            var sender = new AthleteBuilder().Build();
            var receiver = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(sender, receiver);
            await DbContext.SaveChangesAsync();

            var message = new MessageBuilder()
                .WithSenderId(sender.Id)
                .WithReceiverId(receiver.Id)
                .WithContent("Test message")
                .Build();

            // Act
            var result = await _repository.SaveMessageAsync(message);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Content.Should().Be("Test message");
            result.SenderId.Should().Be(sender.Id);
            result.ReceiverId.Should().Be(receiver.Id);
            result.SenderName.Should().Be(sender.UserName);
        }

        [Fact]
        public async Task SaveMessageAsync_ShouldPopulateSenderName()
        {
            // Arrange
            _repository = GetRepository();
            var sender = new AthleteBuilder().WithUsername("alice").Build();
            await DbContext.Athletes.AddAsync(sender);
            await DbContext.SaveChangesAsync();

            var message = new MessageBuilder()
                .WithSenderId(sender.Id)
                .WithReceiverId(Guid.NewGuid().ToString())
                .WithContent("Hello")
                .Build();
            message.SenderName = null; // Ensure it starts as null

            // Act
            var result = await _repository.SaveMessageAsync(message);

            // Assert
            result.SenderName.Should().Be("alice");
        }

        [Fact]
        public async Task SaveMessageAsync_WithMissingSender_ShouldNotSetSenderName()
        {
            // Arrange
            _repository = GetRepository();
            var message = new MessageBuilder()
                .WithSenderId(Guid.NewGuid().ToString())
                .WithReceiverId(Guid.NewGuid().ToString())
                .WithContent("Orphan message")
                .Build();
            message.SenderName = null;

            // Act
            var result = await _repository.SaveMessageAsync(message);

            // Assert
            result.SenderName.Should().BeNull();
        }

        [Fact]
        public async Task GetPrivateMessagesAsync_ShouldReturnConversation()
        {
            // Arrange
            _repository = GetRepository();
            var user1 = new AthleteBuilder().Build();
            var user2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(user1, user2);
            await DbContext.SaveChangesAsync();

            var msg1 = new MessageBuilder().WithSenderId(user1.Id).WithReceiverId(user2.Id).WithContent("Message 1").Build();
            var msg2 = new MessageBuilder().WithSenderId(user2.Id).WithReceiverId(user1.Id).WithContent("Message 2").Build();
            await DbContext.Messages.AddRangeAsync(msg1, msg2);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPrivateMessagesAsync(user1.Id, user2.Id);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(m => 
            {
                m.Should().Match<Message>(x => 
                    (x.SenderId == user1.Id && x.ReceiverId == user2.Id) ||
                    (x.SenderId == user2.Id && x.ReceiverId == user1.Id));
            });
        }

        [Fact]
        public async Task GetPrivateMessagesAsync_ShouldExcludeGroupMessages()
        {
            // Arrange
            _repository = GetRepository();
            var user1 = new AthleteBuilder().Build();
            var user2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(user1, user2);
            await DbContext.SaveChangesAsync();

            var privateMsg = new MessageBuilder().WithSenderId(user1.Id).WithReceiverId(user2.Id).Build();
            var groupMsg = new MessageBuilder().WithSenderId(user1.Id).WithReceiverId(null).Build(); // Group message has no receiver
            await DbContext.Messages.AddRangeAsync(privateMsg, groupMsg);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPrivateMessagesAsync(user1.Id, user2.Id);

            // Assert
            result.Should().ContainSingle();
            result[0].Id.Should().Be(privateMsg.Id);
        }

        [Fact]
        public async Task GetPrivateMessagesAsync_ShouldReturnInChronologicalOrder()
        {
            // Arrange
            _repository = GetRepository();
            var user1 = new AthleteBuilder().Build();
            var user2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(user1, user2);
            await DbContext.SaveChangesAsync();

            var msg1 = new MessageBuilder()
                .WithSenderId(user1.Id)
                .WithReceiverId(user2.Id)
                .WithCreatedDate(DateTime.UtcNow.AddMinutes(-2))
                .Build();
            var msg2 = new MessageBuilder()
                .WithSenderId(user2.Id)
                .WithReceiverId(user1.Id)
                .WithCreatedDate(DateTime.UtcNow)
                .Build();
            await DbContext.Messages.AddRangeAsync(msg1, msg2);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPrivateMessagesAsync(user1.Id, user2.Id);

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(msg1.Id);
            result[1].Id.Should().Be(msg2.Id);
        }

        [Fact]
        public async Task GetPrivateMessagesAsync_WithNonExistentUsers_ShouldReturnEmpty()
        {
            // Arrange
            _repository = GetRepository();

            // Act
            var result = await _repository.GetPrivateMessagesAsync(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task MarkConversationAsReadAsync_ShouldMarkUnreadMessagesAsRead()
        {
            // Arrange
            _repository = GetRepository();
            var reader = new AthleteBuilder().Build();
            var other = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(reader, other);
            await DbContext.SaveChangesAsync();

            var msg1 = new MessageBuilder().WithSenderId(other.Id).WithReceiverId(reader.Id).WithIsRead(false).Build();
            var msg2 = new MessageBuilder().WithSenderId(other.Id).WithReceiverId(reader.Id).WithIsRead(false).Build();
            await DbContext.Messages.AddRangeAsync(msg1, msg2);
            await DbContext.SaveChangesAsync();

            // Act
            var count = await _repository.MarkConversationAsReadAsync(reader.Id, other.Id);

            // Assert
            count.Should().Be(2);
            var updated = await DbContext.Messages.Where(m => m.SenderId == other.Id && m.ReceiverId == reader.Id).ToListAsync();
            updated.Should().AllSatisfy(m => m.IsRead.Should().BeTrue());
        }

        [Fact]
        public async Task MarkConversationAsReadAsync_ShouldNotMarkOtherConversations()
        {
            // Arrange
            _repository = GetRepository();
            var reader = new AthleteBuilder().Build();
            var other1 = new AthleteBuilder().Build();
            var other2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(reader, other1, other2);
            await DbContext.SaveChangesAsync();

            var msg1 = new MessageBuilder().WithSenderId(other1.Id).WithReceiverId(reader.Id).WithIsRead(false).Build();
            var msg2 = new MessageBuilder().WithSenderId(other2.Id).WithReceiverId(reader.Id).WithIsRead(false).Build();
            await DbContext.Messages.AddRangeAsync(msg1, msg2);
            await DbContext.SaveChangesAsync();

            // Act
            await _repository.MarkConversationAsReadAsync(reader.Id, other1.Id);

            // Assert
            var other1Msgs = await DbContext.Messages.Where(m => m.SenderId == other1.Id && m.ReceiverId == reader.Id).ToListAsync();
            var other2Msgs = await DbContext.Messages.Where(m => m.SenderId == other2.Id && m.ReceiverId == reader.Id).ToListAsync();

            other1Msgs.Should().AllSatisfy(m => m.IsRead.Should().BeTrue());
            other2Msgs.Should().AllSatisfy(m => m.IsRead.Should().BeFalse());
        }

        [Fact]
        public async Task MarkConversationAsReadAsync_WithNoUnreadMessages_ShouldReturnZero()
        {
            // Arrange
            _repository = GetRepository();
            var reader = new AthleteBuilder().Build();
            var other = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(reader, other);
            await DbContext.SaveChangesAsync();

            var message = new MessageBuilder().WithSenderId(other.Id).WithReceiverId(reader.Id).WithIsRead(true).Build();
            await DbContext.Messages.AddAsync(message);
            await DbContext.SaveChangesAsync();

            // Act
            var count = await _repository.MarkConversationAsReadAsync(reader.Id, other.Id);

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public async Task GetUnreadMessagesCountAsync_ShouldReturnOnlyUnread()
        {
            // Arrange
            _repository = GetRepository();
            var user = new AthleteBuilder().Build();
            var other1 = new AthleteBuilder().Build();
            var other2 = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(user, other1, other2);
            await DbContext.SaveChangesAsync();

            var unreadMsg1 = new MessageBuilder().WithSenderId(other1.Id).WithReceiverId(user.Id).WithIsRead(false).Build();
            var unreadMsg2 = new MessageBuilder().WithSenderId(other2.Id).WithReceiverId(user.Id).WithIsRead(false).Build();
            var readMsg = new MessageBuilder().WithSenderId(other1.Id).WithReceiverId(user.Id).WithIsRead(true).Build();
            await DbContext.Messages.AddRangeAsync(unreadMsg1, unreadMsg2, readMsg);
            await DbContext.SaveChangesAsync();

            // Act
            var count = await _repository.GetUnreadMessagesCountAsync(user.Id);

            // Assert
            count.Should().Be(2);
        }

        [Fact]
        public async Task GetUnreadMessagesCountAsync_ShouldNotCountOtherUsersMessages()
        {
            // Arrange
            _repository = GetRepository();
            var user1 = new AthleteBuilder().Build();
            var user2 = new AthleteBuilder().Build();
            var other = new AthleteBuilder().Build();
            await DbContext.Athletes.AddRangeAsync(user1, user2, other);
            await DbContext.SaveChangesAsync();

            var msg1 = new MessageBuilder().WithSenderId(other.Id).WithReceiverId(user1.Id).WithIsRead(false).Build();
            var msg2 = new MessageBuilder().WithSenderId(other.Id).WithReceiverId(user2.Id).WithIsRead(false).Build();
            await DbContext.Messages.AddRangeAsync(msg1, msg2);
            await DbContext.SaveChangesAsync();

            // Act
            var count1 = await _repository.GetUnreadMessagesCountAsync(user1.Id);
            var count2 = await _repository.GetUnreadMessagesCountAsync(user2.Id);

            // Assert
            count1.Should().Be(1);
            count2.Should().Be(1);
        }

        [Fact]
        public async Task GetUnreadMessagesCountAsync_WithNoMessages_ShouldReturnZero()
        {
            // Arrange
            _repository = GetRepository();
            var user = new AthleteBuilder().Build();
            await DbContext.Athletes.AddAsync(user);
            await DbContext.SaveChangesAsync();

            // Act
            var count = await _repository.GetUnreadMessagesCountAsync(user.Id);

            // Assert
            count.Should().Be(0);
        }
    }
}
