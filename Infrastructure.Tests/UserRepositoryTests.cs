using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

namespace Infrastructure.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            var user = new User
            {
                Oid = "1",
                Name = "Claus",
                Email = "claus@outlook.com"
            };
            context.Users.Add(user);
            context.SaveChanges();

            _context = context;
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_creates_new_user_with_generated_id_then_delete()
        {
            var user = new UserCreateDto
            {
                Oid = "2",
                Name = "Karsten",
                Email = "karsten@outlook.com"
            };

            var (status, content) = await _repository.CreateAsync(user);

            Assert.Equal("2", content?.Oid);
            Assert.Equal(user.Name, content?.Name);
            Assert.Equal(user.Email, content?.Email);

            var users = await _repository.ReadAsync();

            Assert.Collection(users,
                s => Assert.Equal(new UserDto("1", "Claus", "claus@outlook.com"), s),
                s => Assert.Equal(new UserDto("2", "Karsten", "karsten@outlook.com"), s)
            );

            var response = await _repository.DeleteAsync("2");
            Assert.Equal(Deleted, response);
        }

        [Fact]
        public async Task ReadAsync_returns_all_users()
        {
            var users = await _repository.ReadAsync();

            Assert.Collection(users,
                s => Assert.Equal(new UserDto("1", "Claus", "claus@outlook.com"), s)
            );
        }

        [Fact]
        public async Task ReadAsync_given_existing_id_returns_Some()
        {
            var option = await _repository.ReadAsync("1");
            Assert.True(option.IsSome);
        }

        [Fact]
        public async Task ReadAsync_given_non_existing_id_returns_None()
        {
            var option = await _repository.ReadAsync("11");
            Assert.True(option.IsNone);
        }

        [Fact]
        public async Task DeleteAsync_given_non_existing_Id_returns_NotFound()
        {
            var response = await _repository.DeleteAsync("11");
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
