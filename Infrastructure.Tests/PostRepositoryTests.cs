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

            var post = new Post
            {
                Id = 1,
                Title = "Math Project",
                Content = "Bla bla bla bla",
                DateAdded = today,
                SupervisorId = 1,
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
            var today = DateTime.Now;
            var post = new PostCreateDto
            {
                Title = "Biology Project",
                Content = "Bla bla bla bla",
                DateAdded = today,
                SupervisorId = 1,
                Tags = new HashSet<string> { "bio", "dna", "cells" }
            };

            var created = await _repository.CreateAsync(post);

            Assert.Equal(2, created.Id);
            Assert.Equal("Biology Project", created.Title);
            Assert.Equal("Bla bla bla bla", created.Content);
            Assert.Equal(today, created.DateAdded);
            Assert.Equal(1, created.SupervisorId);
            Assert.True(created.Tags.SetEquals(new[] { "bio", "dna", "cells" }));
        }

        [Fact]
        public async Task ReadAsync_given_non_existing_id_returns_None()
        {
            var option = await _repository.ReadAsync(11);

            Assert.True(option.IsNone);
        }

        [Fact]
        public async Task ReadAsync_given_existing_id_returns_post()
        {
            var option = await _repository.ReadAsync(1);

            Assert.Equal(1, option.Value.Id);
            Assert.Equal("Math Project", option.Value.Title);
            Assert.Equal("Bla bla bla bla", option.Value.Content);
            Assert.Equal(today, option.Value.DateAdded);
            Assert.Equal(1, option.Value.SupervisorId);
            Assert.Equal(1, option.Value.Tags.Count);
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