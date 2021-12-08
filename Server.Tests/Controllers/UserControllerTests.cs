using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class UserControllerTests : TestBaseController<UserController>
    {
        [Fact]
        public async Task Create_creates_User()
        {
            // Arrange
            var toCreate = new UserCreateDto();
            var user = new UserDetailsDto("1", "John", "");
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Created, user));
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var resultObject = GetCreatedResultContent<UserDetailsDto>(result);
            Assert.Equal(user, resultObject);
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
            repository.Setup(m => m.ReadAsync("11")).ReturnsAsync(default(UserDetailsDto));
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByUserId("11");

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_User()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            var character = new UserDetailsDto("1", "Jack", "");
            repository.Setup(m => m.ReadAsync("1")).ReturnsAsync(character);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByUserId("1");

            // Assert
            Assert.Equal(character, response.Value);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.DeleteAsync("11")).ReturnsAsync(Status.NotFound);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete("11");

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var repository = new Mock<IUserRepository>();
            repository.Setup(m => m.DeleteAsync("1")).ReturnsAsync(Status.Deleted);
            var controller = new UserController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete("1");

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
