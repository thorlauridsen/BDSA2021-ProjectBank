using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class CommentControllerTests
    {
        [Fact]
        public async Task Create_creates_Comment()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var toCreate = new CommentCreateDto();
            var created = new CommentDetailsDto(1, "Hello there", DateTime.Now, 1, 1);
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtRouteResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("Get", result?.RouteName);
            Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task Get_returns_Comments_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var expected = Array.Empty<CommentDto>();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(CommentDetailsDto));
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Comment()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var repository = new Mock<ICommentRepository>();
            var comment = new CommentDetailsDto(1, "Hello there", DateTime.Now, 1, 1);
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(comment);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(1);

            // Assert
            Assert.Equal(comment, response.Value);
        }

        [Fact]
        public async Task Put_updates_Comment()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var comment = new CommentUpdateDto();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.UpdateAsync(1, comment)).ReturnsAsync(Updated);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, comment);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_id_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var comment = new CommentUpdateDto();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.UpdateAsync(1, comment)).ReturnsAsync(NotFound);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, comment);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.DeleteAsync(11)).ReturnsAsync(Status.NotFound);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(11);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var logger = new Mock<ILogger<CommentController>>();
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.DeleteAsync(1)).ReturnsAsync(Status.Deleted);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
