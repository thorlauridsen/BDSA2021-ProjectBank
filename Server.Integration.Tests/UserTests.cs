using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ProjectBank.Core;
using Xunit;

namespace ProjectBank.Server.Integration.Tests
{
    public class UserTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public UserTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Get_returns_Users()
        {
            var users = await _client.GetFromJsonAsync<UserDto[]>("/api/User");

            Assert.NotNull(users);
            Assert.True(users?.Length >= 2);
            Assert.Contains(users, u => u.Name == "Paolo");
        }

        [Fact]
        public async Task Post_returns_Created_with_location()
        {
            var user = new UserCreateDto
            {
                oid = "3",
                Name = "Rasmus",
                IsSupervisor = true
            };
            var response = await _client.PostAsJsonAsync("/api/User", user);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(new Uri("http://localhost/api/User/3"), response.Headers.Location);

            var created = await response.Content.ReadFromJsonAsync<UserDetailsDto>();

            Assert.NotNull(created);
            Assert.Equal("3", created?.oid);
            Assert.Equal("Rasmus", created?.Name);
            Assert.True(created?.IsSupervisor);
        }
    }
}
