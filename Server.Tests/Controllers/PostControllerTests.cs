using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProjectBank.Core;
using ProjectBank.Server.Controllers;
using Xunit;
using static ProjectBank.Core.Status;

namespace ProjectBank.Server.Tests.Controllers
{
    public class PostControllerTests
    {
        [Fact]
        public async Task Create_creates_Post()
        {
            // Arrange
            var logger = new Mock<ILogger<PostController>>();
            var toCreate = new PostCreateDto();
            var created = new PostDetailsDto
            (
                1,
                "Biology Project",
                "My Cool Biology Project",
                DateTime.Now,
                1,
                new HashSet<string>() { "Biology" }
            );
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync(created);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var result = await controller.Post(toCreate) as CreatedAtRouteResult;

            // Assert
            Assert.Equal(created, result?.Value);
            Assert.Equal("Get", result?.RouteName);
            Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
        }

        [Fact]
        public async Task Get_returns_Posts_from_repo()
        {
            // Arrange
            var logger = new Mock<ILogger<PostController>>();
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
            var logger = new Mock<ILogger<PostController>>();
            var repository = new Mock<IPostRepository>();
            repository.Setup(m => m.ReadAsync(11)).ReturnsAsync(default(PostDetailsDto));
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(11);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Get_given_existing_returns_Post()
        {
            // Arrange
            var logger = new Mock<ILogger<PostController>>();
            var repository = new Mock<IPostRepository>();
            var post = new PostDetailsDto
            (
                1,
                "Biology Project",
                "My Cool Biology Project",
                DateTime.Now,
                1,
                new HashSet<string>() { "Biology" }
            );
            repository.Setup(m => m.ReadAsync(1)).ReturnsAsync(post);
            var controller = new PostController(logger.Object, repository.Object);

            // Act
            var response = await controller.Get(1);

            // Assert
            Assert.Equal(post, response.Value);
        }

        [Fact]
        public async Task Put_updates_Post()
        {
            // Arrange
            var logger = new Mock<ILogger<PostController>>();
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
            var logger = new Mock<ILogger<PostController>>();
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
            var logger = new Mock<ILogger<PostController>>();
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
            var logger = new Mock<ILogger<PostController>>();
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
