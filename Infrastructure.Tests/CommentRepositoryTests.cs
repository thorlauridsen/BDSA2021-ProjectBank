using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

namespace Infrastructure.Tests
{
    public class CommentRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            Comment comment1 = new Comment { Id = 1, Content = "Hey" };
            Comment comment2 = new Comment { Id = 2, Content = "Die in a hole" };

            context.Comments.Add(comment1);
            context.Comments.Add(comment2);

            context.SaveChanges();

            _context = context;
            _repository = new CommentRepository(_context);
        }

        [Fact]
        public async Task ReadAsync_given_existing_id_returns_Comment()
        {
            var option = await _repository.ReadAsync(1);

            Assert.True(option.IsSome);
        }

        [Fact]
        public async Task ReadAsync_given_non_existing_id_returns_None()
        {
            var option = await _repository.ReadAsync(11);

            Assert.True(option.IsNone);
        }

        [Fact]
        public async Task DeleteAsync_given_existing_Id_returns_Deleted()
        {
            var response = await _repository.DeleteAsync(2);

            Assert.Equal(Deleted, response);
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
