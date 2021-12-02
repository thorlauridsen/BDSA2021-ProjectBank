using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class StudentControllerTests
    {
        private Mock<ILogger<StudentController>> logger
            = new Mock<ILogger<StudentController>>();

        [Fact]
        public async Task Create_creates_Student()
        {
            // Arrange
            var toCreate = new StudentCreateDto();
            var created = new StudentDetailsDto(1, "John");
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtRouteResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("GetByStudentId", result?.RouteName);
            Assert.Equal(KeyValuePair.Create("userId", (object?)1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task Get_returns_Students_from_repo()
        {
            // Arrange
            var expected = Array.Empty<StudentDto>();
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(StudentDetailsDto));
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByStudentId(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Student()
        {
            // Arrange
            var repository = new Mock<IStudentRepository>();
            var character = new StudentDetailsDto(1, "Jack");
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(character);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByStudentId(1);

            // Assert
            Assert.Equal(character, response.Value);
        }

        [Fact]
        public async Task Put_updates_Student()
        {
            // Arrange
            var character = new StudentUpdateDto();
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(Updated);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_id_returns_NotFound()
        {
            // Arrange
            var character = new StudentUpdateDto();
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.UpdateAsync(1, character)).ReturnsAsync(NotFound);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, character);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.DeleteAsync(11)).ReturnsAsync(Status.NotFound);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(11);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var repository = new Mock<IStudentRepository>();
            repository.Setup(m => m.DeleteAsync(1)).ReturnsAsync(Status.Deleted);
            var controller = new StudentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
