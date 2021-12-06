using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class NotificationControllerTests : TestBaseController<NotificationController>
    {
        [Fact]
        public async Task Get_returns_Notifications_from_repo()
        {
            // Arrange
            var expected = Array.Empty<NotificationDetailsDto>();
            var repository = new Mock<INotificationRepository>();
            repository.Setup(m => m.GetNotificationsAsync("1")).ReturnsAsync(expected);
            var controller = new NotificationController(logger.Object, repository.Object);

            // Act
            var actual = await controller.GetNotificationByUserId("1");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAsync_creates_Notification()
        {
            //TODO: Implement CreateAsync unit test
            // Arrange
            var toCreate = new NotificationCreateDto();
            var notification = new NotificationDetailsDto
            {
                Id = 1,
                Title = "Important Notification!",
                Content = "Hello remember to follow",
                Timestamp = DateTime.Now,
                Link = "https://google.com"
            };
            var repository = new Mock<INotificationRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Created, notification));
            var controller = new NotificationController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var resultObject = GetResultContent<NotificationDetailsDto>(result);
            Assert.Equal(notification, resultObject);
        }

        [Fact]
        public async Task SeenNotification()
        {
            //TODO: Implement SeenNotification unit test
        }
    }
}
