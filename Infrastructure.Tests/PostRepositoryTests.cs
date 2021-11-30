using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;

namespace Infrastructure.Tests
{
    public class PostRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly PostRepository _repository;
        //private readonly SupervisorRepository _supervisorRepository;

        public PostRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();


            //Add test stuff to database here
            //context.


            //context.SaveChanges();

            _context = context;
            _repository = new PostRepository(_context);


        }

        [Fact]
        public async Task CreateAsync_creates_new_post_with_generated_id()
        {
            var today = DateTime.Now;
            var post = new PostCreateDto
            {
                Title = "Biology project",
                Content = "Bla bla bla bla",
                DateAdded = today,
                SupervisorId = 1,
                Tags = new HashSet<string> { "bio", "dna", "cells" }

            };

            var created = await _repository.CreateAsync(post);

            Assert.Equal(1, created.Id);
            Assert.Equal("Biology project", created.Title);
            Assert.Equal("Bla bla bla bla", created.Content);
            Assert.Equal(today, created.DateAdded);
            Assert.Equal(1, created.SupervisorId);
            Assert.True(created.Tags.SetEquals(new[] { "bio", "dna", "cells" }));
        }
        [Fact]
        public async Task ReadAsync_returns_all_characters()
        {
            var posts = await _repository.ReadAsync();

            // Assert.Collection(posts,
            //     //character => Assert.Equal(new CharacterDto(1, "Clark", "Kent", "Superman"), character),

            // );
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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}