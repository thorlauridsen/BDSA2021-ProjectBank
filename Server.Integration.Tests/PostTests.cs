using System;
using System.Collections.Generic;
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
    public class PostTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public PostTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_returns_Posts()
        {
            var posts = await _client.GetFromJsonAsync<PostDetailsDto[]>("/api/Post");

            Assert.NotNull(posts);
            Assert.True(posts?.Length >= 1);
            var post = posts?.First();

            Assert.NotNull(post);
            Assert.Equal(1, post?.Id);
            Assert.Equal("Biology Project", post?.Title);
            Assert.Equal("My Cool Biology Project", post?.Content);
        }

        [Fact]
        public async Task Post_returns_created_Post_with_location()
        {
            var post = new PostCreateDto
            {
                Title = "Math Project",
                Content = "My Cool Math Project",
                UserOid = "1",
                Tags = new HashSet<string>() { "Math" }
            };
            var response = await _client.PostAsJsonAsync("/api/Post", post);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(new Uri("http://localhost/api/Post/2"), response.Headers.Location);

            var created = await response.Content.ReadFromJsonAsync<PostDetailsDto>();

            Assert.NotNull(created);
            Assert.Equal(2, created?.Id);
            Assert.Equal("Math Project", created?.Title);
            Assert.Equal("My Cool Math Project", created?.Content);
            Assert.Equal("1", created?.UserOid);
            Assert.Equal("Math", created?.Tags.First());
        }
    }
}
