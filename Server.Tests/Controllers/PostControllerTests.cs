using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class PostControllerTests : TestBaseController<PostController>
    {
        [Fact]
        public async Task Create_creates_Post()
        {
            // Arrange
            var toCreate = new PostCreateDto();
            var created = new PostDetailsDto
            (
                1,
                "Biology Project",
                "My Cool Biology Project",
                DateTime.Now,
                "1",
                new HashSet<string>() { "Biology" }
            );
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Created, created));
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var resultObject = GetCreatedResultContent(result);
            Assert.Equal(created, resultObject);
        }

        [Fact]
        public async Task Get_returns_Posts_from_repo()
        {
            // Arrange
            var expected = Array.Empty<PostDto>();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.ReadAsync()).ReturnsAsync(expected);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var actual = await controller.Get();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Get_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(PostDetailsDto));
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByPostId(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Post()
        {
            // Arrange
            var repository = new Mock<IPostRepository>();
            var post = new PostDetailsDto
            (
                1,
                "Biology Project",
                "My Cool Biology Project",
                DateTime.Now,
                "1",
                new HashSet<string>() { "Biology" }
            );
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(post);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByPostId(1);

            // Assert
            Assert.Equal(post, response.Value);
        }


        // FIXME
        //[Fact]
        //public async Task GetBySupervisor_given_existing_returns_Post()
        //{
        //    // Arrange
        //    var expected = Array.Empty<PostDto>();
        //    var repository = new Mock<IPostRepository>();
        //    repository.Setup(m => m.ReadAsyncBySupervisor("1")).ReturnsAsync(expected);
        //    var controller = new PostController(logger.Object, repository.Object);

        //    // Act
        //    var response = await controller.GetBySupervisor("1");

        //    // Assert
        //    Assert.Equal(expected, response);
        //}

        [Fact]
        public async Task GetByTag_given_existing_returns_Post()
        {
            // Arrange
            var expected = Array.Empty<PostDto>();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.ReadAsyncByTag("Math")).ReturnsAsync(expected);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.GetByTag("Math");

            // Assert
            Assert.Equal(expected, response);
        }

        [Fact]
        public async Task GetComments_given_existing_postId_returns_Comments()
        {
            // Arrange
            var expected = Array.Empty<CommentDto>();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.ReadAsyncComments(1)).ReturnsAsync(expected);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var result = await controller.GetComments(1);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Put_updates_Post()
        {
            // Arrange
            var post = new PostUpdateDto();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.UpdateAsync(1, post)).ReturnsAsync(Updated);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, post);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Put_given_unknown_id_returns_NotFound()
        {
            // Arrange
            var post = new PostUpdateDto();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.UpdateAsync(1, post)).ReturnsAsync(NotFound);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Put(1, post);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_non_existing_returns_NotFound()
        {
            // Arrange
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.DeleteAsync(11)).ReturnsAsync(Status.NotFound);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(11);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Delete_given_existing_returns_NoContent()
        {
            // Arrange
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.DeleteAsync(1)).ReturnsAsync(Status.Deleted);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}
