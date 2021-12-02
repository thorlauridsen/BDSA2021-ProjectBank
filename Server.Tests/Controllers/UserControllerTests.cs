using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class UserControllerTests
    {
        private Mock<ILogger<UserController>> logger
            = new Mock<ILogger<UserController>>();

        [Fact]
        public async Task Create_creates_User()
        {
            // Arrange
            var toCreate = new UserCreateDto();
            var created = new UserDetailsDto(1, "John");
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtRouteResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("Get", result?.RouteName);
            Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task Get_returns_Users_from_repo()
        {
            // Arrange
            var expected = Array.Empty<UserDto>();
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(UserDetailsDto));
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_User()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            var character = new UserDetailsDto(1, "Jack");
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(character);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(1);

            // Assert
            Assert.Equal(character, response.Value);
        }

        [Fact]
        public async Task Put_updates_User()
        {
            // Arrange
            var character = new UserUpdateDto();
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(Updated);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_id_returns_NotFound()
        {
            // Arrange
            var character = new UserUpdateDto();
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(NotFound);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.DeleteAsync(11)).ReturnsAsync(Status.NotFound);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(11);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.DeleteAsync(1)).ReturnsAsync(Status.Deleted);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
