using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;

namespace ProjectBank.Server.Tests.Controllers
{
    public class NotificationControllerTests
    {
        [Fact]
        public async Task Get_returns_Notifications_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<NotificationController>>();
            var expected = Array.Empty<NotificationDetailsDto>();
            var repository = new Mock<INotificationRepository>();
            repository.Setup(m => m.GetNotificationsAsync(1)).ReturnsAsync(expected);
            var controller = new NotificationController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get(1);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
