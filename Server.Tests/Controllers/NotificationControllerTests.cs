using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;

namespace ProjectBank.Server.Tests.Controllers
{
    public class NotificationControllerTests
    {
        private Mock<ILogger<NotificationController>> logger
            = new Mock<ILogger<NotificationController>>();

        [Fact]
        public async Task Get_returns_Notifications_from_repo()
        {
            // Arrange
            var expected = Array.Empty<NotificationDetailsDto>();
            var repository = new Mock<INotificationRepository>();
            repository.Setup(m => m.GetNotificationsAsync(1)).ReturnsAsync(expected);
            var controller = new NotificationController(logger.Object, repository.Object);

            // Act
            var actual = await controller.GetByNotificationId(1);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAsync_creates_Notification()
        {
            //TODO: Implement CreateAsync unit test
        }

        [Fact]
        public async Task SeenNotification()
        {
            //TODO: Implement SeenNotification unit test
        }
    }
}
