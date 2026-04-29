using System.Security.Claims;
using FAIR.API.Hubs;
using FAIR.Application.DTOs.Chat;
using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace FAIR.Tests.Unit.Hubs
{
    public class ChatHubTests
    {
        [Fact]
        public async Task SendMessage_ShouldSaveFirst_ThenSendToRecipient()
        {
            var repoManager = new Mock<IRepositoryManager>();
            var serviceManager = new Mock<IServiceManager>();
            var chatService = new Mock<IChatService>();

            var savedMessage = new Message { SenderId = "sender-1", ReceiverId = "receiver-1", Content = "hello" };
            var sequence = new MockSequence();
            chatService.InSequence(sequence)
                .Setup(r => r.SavePrivateMessageAsync("sender-1", It.IsAny<MessageRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(savedMessage);

            var recipientProxy = new Mock<IClientProxy>();
            var senderProxy = new Mock<IClientProxy>();
            recipientProxy.InSequence(sequence)
                .Setup(p => p.SendCoreAsync("ReceivedMessage", It.IsAny<object?[]>(), default))
                .Returns(Task.CompletedTask);
            senderProxy.InSequence(sequence)
                .Setup(p => p.SendCoreAsync("MessageSent", It.IsAny<object?[]>(), default))
                .Returns(Task.CompletedTask);

            var clients = new Mock<IHubCallerClients>();
            clients.Setup(c => c.User("receiver-1")).Returns(recipientProxy.Object);
            clients.Setup(c => c.User("sender-1")).Returns(senderProxy.Object);
            serviceManager.SetupGet(s => s.ChatService).Returns(chatService.Object);

            var hub = new ChatHub(repoManager.Object, serviceManager.Object)
            {
                Clients = clients.Object,
                Context = BuildContext("sender-1")
            };

            await hub.SendMessage(new MessageRequest { ReceiverId = "receiver-1", Content = "hello" });

            chatService.Verify(r => r.SavePrivateMessageAsync("sender-1", It.IsAny<MessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            recipientProxy.Verify(p => p.SendCoreAsync("ReceivedMessage", It.IsAny<object?[]>(), default), Times.Once);
        }

        [Fact]
        public async Task Typing_ShouldOnlyTargetRecipient()
        {
            var repoManager = new Mock<IRepositoryManager>();
            var serviceManager = new Mock<IServiceManager>();

            var recipientProxy = new Mock<IClientProxy>();
            var clients = new Mock<IHubCallerClients>();
            clients.Setup(c => c.User("receiver-9")).Returns(recipientProxy.Object);

            var hub = new ChatHub(repoManager.Object, serviceManager.Object)
            {
                Clients = clients.Object,
                Context = BuildContext("sender-9")
            };

            await hub.Typing("receiver-9", true);

            recipientProxy.Verify(p => p.SendCoreAsync("Typing", It.IsAny<object?[]>(), default), Times.Once);
            clients.Verify(c => c.User(It.Is<string>(x => x != "receiver-9")), Times.Never);
        }

        [Fact]
        public async Task MarkAsRead_ShouldSendReceiptToOtherUser_WhenChanged()
        {
            var repoManager = new Mock<IRepositoryManager>();
            var serviceManager = new Mock<IServiceManager>();
            var chatRepo = new Mock<IChatRepository>();
            repoManager.SetupGet(r => r.ChatRepository).Returns(chatRepo.Object);
            chatRepo.Setup(r => r.MarkConversationAsReadAsync("reader-1", "other-1")).ReturnsAsync(1);

            var otherProxy = new Mock<IClientProxy>();
            var clients = new Mock<IHubCallerClients>();
            clients.Setup(c => c.User("other-1")).Returns(otherProxy.Object);

            var hub = new ChatHub(repoManager.Object, serviceManager.Object)
            {
                Clients = clients.Object,
                Context = BuildContext("reader-1")
            };

            await hub.MarkAsRead("other-1");

            otherProxy.Verify(p => p.SendCoreAsync("ReadReceipt", It.IsAny<object?[]>(), default), Times.Once);
        }

        private static HubCallerContext BuildContext(string userId)
        {
            var context = new Mock<HubCallerContext>();
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "Test");
            var principal = new ClaimsPrincipal(identity);
            context.SetupGet(c => c.User).Returns(principal);
            context.SetupGet(c => c.ConnectionId).Returns(Guid.NewGuid().ToString());
            return context.Object;
        }
    }
}
