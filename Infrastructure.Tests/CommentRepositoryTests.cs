using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
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
            builder.EnableSensitiveDataLogging();
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            var supervisor = new User() { oid = "1", IsSupervisor = true, Name = "bo" };
            var student = new User() { oid = "2", IsSupervisor = false, Name = "alice" };

            var post = new Post() { Id = 1 , Title = "test title", Content = "test", DateAdded = DateTime.Now, User = supervisor};
            var post2 = new Post() { Id = 2 , Title = "test title", Content = "test", DateAdded = DateTime.Now, User = supervisor};
            context.Posts.Add(post);
            context.Posts.Add(post2);
            var comment1 = new Comment { Id = 1, User = student, DateAdded = DateTime.Now, Content = "Hey"};
            post.Comments.Add(comment1);
            var comment2 = new Comment { Id = 2, User = supervisor, DateAdded = DateTime.Now, Content = "hi"};
            post2.Comments.Add(comment2);

            /*
            context.Comments.Add(comment1);
            context.Comments.Add(comment2);*/

            context.SaveChanges();

            _context = context;
            _repository = new CommentRepository(_context);
        }

        [Fact]
        public async Task ReadAsync_given_existing_id_returns_Comment()
        {
            var option = await _repository.ReadAsync(1,1);

            Assert.True(option.IsSome);
        }

        [Fact]
        public async Task ReadAsync_given_non_existing_id_returns_None()
        {
            var option = await _repository.ReadAsync(1,11);

            Assert.True(option.IsNone);
        }

        [Fact]
        public async void CreateAsync_given_commentCreateDto()
        {
            var comment = new CommentCreateDto { UserId = "2", Content = "some question", postid = 1};

            var (option, actual) = await _repository.CreateAsync(comment);
            
            Assert.Equal(Created, option);
            Assert.Equal(3, actual.Id);

        }

        [Fact]
        public async void CreateAsync_given_unknown_PostId_returns_bad_request()
        {
            var comment = new CommentCreateDto { UserId = "2", Content = "some question", postid = 14134314};

            var (option, actual) = await _repository.CreateAsync(comment);
            
            Assert.Equal(BadRequest, option);
        }
        
        [Fact]
        public async void CreateAsync_given_no_PostId_returns_bad_request()
        {
            var comment = new CommentCreateDto { UserId = "2", Content = "some question"};

            var (option, actual) = await _repository.CreateAsync(comment);
            
            Assert.Equal(BadRequest, option);
        }
        
        /*[Fact]
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
        }*/

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
