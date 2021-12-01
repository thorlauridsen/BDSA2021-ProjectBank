using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;

namespace ProjectBank.Server.Tests.Controllers
{
    public class ChatControllerTests
    {
        [Fact]
        public async Task Get_given_chatId_returns_ChatMessages_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<ChatController>>();
            var expected = Array.Empty<ChatMessageDto>();
            var repository = new Mock<IChatRepository>();
            repository.Setup(m => m.ReadSpecificChatAsync(1)).ReturnsAsync(expected);
            var controller = new ChatController(logger.Object, repository.Object);

            // Act
            var actual = await controller.GetChatMessages(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_userId_returns_ChatDetails_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<ChatController>>();
            var expected = Array.Empty<ChatDetailsDto>();
            var repository = new Mock<IChatRepository>();
            repository.Setup(m => m.ReadAllChatsAsync(1)).ReturnsAsync(expected);
            var controller = new ChatController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get(1);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
