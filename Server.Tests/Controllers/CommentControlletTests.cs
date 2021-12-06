using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class CommentControllerTests : TestBaseController<CommentController>
    {

        [Fact]
        public async Task Create_creates_Comment()
        {
            // Arrange
            var toCreate = new CommentCreateDto();
            var comment = new CommentDetailsDto(1, "Hello there", DateTime.Now, "1", 1);
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Created, comment));
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var resultObject = GetResultContent<CommentDetailsDto>(result);
            Assert.Equal(comment, resultObject);
        }

        [Fact]
        public async Task Get_returns_Comments_from_repo()
        {
            // Arrange
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
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(CommentDetailsDto));
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByCommentId(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Comment()
        {
            // Arrange
            var repository = new Mock<ICommentRepository>();
            var comment = new CommentDetailsDto(1, "Hello there", DateTime.Now, "1", 1);
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(comment);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByCommentId(1);

            // Assert
            Assert.Equal(comment, response.Value);
        }

        [Fact]
        public async Task Put_updates_Comment()
        {
            // Arrange
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
