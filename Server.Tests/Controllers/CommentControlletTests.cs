using Microsoft.AspNetCore.Mvc;
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
            var comment = new CommentDetailsDto
            {
                Id = 1,
                Content = "Hello there",
                DateAdded = DateTime.Now,
                UserOid = "1"
            };
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Created, comment));
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var resultObject = GetCreatedResultContent(result);
            Assert.Equal(comment, resultObject);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.ReadAsync(1, 11)).ReturnsAsync(default(CommentDetailsDto));
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByCommentId(1, 11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Comment()
        {
            // Arrange
            var repository = new Mock<ICommentRepository>();
            var comment = new CommentDetailsDto
            {
                Id = 1,
                Content = "Hello there",
                DateAdded = DateTime.Now,
                UserOid = "1"
            };
            repository.Setup(m => m.ReadAsync(1, 1)).ReturnsAsync(comment);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByCommentId(1, 1);

            // Assert
            Assert.Equal(comment, response.Value);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.DeleteAsync(1, 11)).ReturnsAsync(Status.NotFound);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1, 11);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var repository = new Mock<ICommentRepository>();
            repository.Setup(m => m.DeleteAsync(1, 1)).ReturnsAsync(Status.Deleted);
            var controller = new CommentController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1, 1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
