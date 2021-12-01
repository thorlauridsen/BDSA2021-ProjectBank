using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class SupervisorControllerTests
    {
        [Fact]
        public async Task Create_creates_Supervisor()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var toCreate = new SupervisorCreateDto();
            var created = new SupervisorDetailsDto(1, "John");
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtRouteResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("Get", result?.RouteName);
            Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task Get_returns_Supervisors_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var expected = Array.Empty<SupervisorDto>();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.ReadAsync(42)).ReturnsAsync(default(SupervisorDetailsDto));
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(42);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Supervisor()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var repository = new Mock<ISupervisorRepository>();
            var character = new SupervisorDetailsDto(1, "Jack");
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(character);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(1);

            // Assert
            Assert.Equal(character, response.Value);
        }

        [Fact]
        public async Task Put_updates_Supervisor()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var character = new SupervisorUpdateDto();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(Updated);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_id_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var character = new SupervisorUpdateDto();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(NotFound);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.DeleteAsync(42)).ReturnsAsync(Status.NotFound);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(42);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var logger = new Mock<ILogger<SupervisorController>>();
            var repository = new Mock<ISupervisorRepository>();
            repository.Setup(m => m.DeleteAsync(1)).ReturnsAsync(Status.Deleted);
            var controller = new SupervisorController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
