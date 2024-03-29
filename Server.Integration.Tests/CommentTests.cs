using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ProjectBank.Core;
using Xunit;

namespace ProjectBank.Server.Integration.Tests
{
    public class CommentTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CommentTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_returns_Comments()
        {
            var comments = await _client.GetFromJsonAsync<CommentDetailsDto[]>("/api/Post/1/comments");

            Assert.NotNull(comments);
            Assert.True(comments?.Length >= 1);
            var comment = comments?.First();

            Assert.NotNull(comment);
            Assert.Equal(1, comment?.Id);
            Assert.Equal("Nice post", comment?.Content);
            Assert.Equal("2", comment?.UserOid);
        }

        [Fact]
        public async Task Post_returns_created_Comment_with_location()
        {
            var comment = new CommentCreateDto
            {
                Content = "Hello there",
                UserOid = "1",
                postid = 1
            };
            var response = await _client.PostAsJsonAsync("/api/Comment", comment);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(new Uri("http://localhost/api/Comment/1/2"), response.Headers.Location);

            var created = await response.Content.ReadFromJsonAsync<CommentDetailsDto>();

            Assert.NotNull(created);
            Assert.Equal(2, created?.Id);
            Assert.Equal("Hello there", created?.Content);
            Assert.Equal("1", created?.UserOid);
        }
    }
}
