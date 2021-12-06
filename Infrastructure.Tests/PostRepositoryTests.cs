using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

namespace Infrastructure.Tests
{
    public class PostRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly PostRepository _repository;

        private DateTime today = DateTime.Now;

        public PostRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            var user = new User {oid = "1", Name = "Bob" };
            context.Users.Add(user);

            var post = new Post
            {
                Id = 1,
                Title = "Math Project",
                Content = "Bla bla bla bla",
                DateAdded = today,
                User = user,
                Tags = new HashSet<Tag> { new Tag("Math") }
            };
            context.Posts.Add(post);
            context.SaveChanges();

            _context = context;
            _repository = new PostRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_creates_new_post_with_generated_id()
        {
            var post = new PostCreateDto
            {
                Title = "Biology Project",
                Content = "Bla bla bla bla",
                SupervisorOid = "1",
                Tags = new HashSet<string> { "bio", "dna", "cells" }
            };

            var created = await _repository.CreateAsync(post);

            Assert.Equal(Created, created.Item1);
            Assert.Equal(2, created.Item2.Id);
            Assert.Equal("Biology Project", created.Item2.Title);
            Assert.Equal("Bla bla bla bla", created.Item2.Content);
            Assert.Equal("1", created.Item2.SupervisorOid);
            Assert.True(created.Item2.Tags.SetEquals(new[] { "bio", "dna", "cells" }));
        }

        [Fact]
        public async Task ReadAsync_given_existing_id_returns_post()
        {
            var option = await _repository.ReadAsync(1);

            Assert.Equal(1, option.Value.Id);
            Assert.Equal("Math Project", option.Value.Title);
            Assert.Equal("Bla bla bla bla", option.Value.Content);
            Assert.Equal(today, option.Value.DateAdded);
            Assert.Equal("1", option.Value.SupervisorOid);
            Assert.Equal(1, option.Value.Tags.Count);
        }

        [Fact]
        public async Task ReadAsync_given_non_existing_id_returns_None()
        {
            var option = await _repository.ReadAsync(11);

            Assert.True(option.IsNone);
        }

        [Fact]
        public async Task DeleteAsync_given_non_existing_Id_returns_NotFound()
        {
            var response = await _repository.DeleteAsync(11);

            Assert.Equal(NotFound, response);
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. 
            // Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
