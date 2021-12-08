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
                Comments = new List<Comment>()
                {
                    new() {Id =1, Content ="Hello",User= user,DateAdded = new DateTime(2021,12,6)}
                },
                User = user,
                Tags = new HashSet<Tag> { new Tag("Math") }
            };
            var post1 = new Post
            {
                Id = 2,
                Title = "Physics Project",
                Content = "Something about physics and stuff",
                DateAdded = today,
                User = user,
                Tags = new HashSet<Tag> { new Tag("Science"), new Tag("Physics") }
            };


            context.Posts.Add(post);
            context.Posts.Add(post1);
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
            Assert.Equal(3, created.Item2.Id);
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

        //TODO
        [Fact]
        public async Task ReadAsync_returns_all_posts()
        {   
            var actual =  await _repository.ReadAsync();

            var actual1 = actual.ElementAt(0);
            var actual2 = actual.ElementAt(1);

            Assert.Equal(1,actual1.Id);
            Assert.Equal("Math Project", actual1.Title);
            Assert.Equal("Bla bla bla bla", actual1.Content);
            Assert.Equal(today, actual1.DateAdded); 
            Assert.Equal("1", actual1.SupervisorOid);
            Assert.Equal(1, actual1.Tags.Count);

            Assert.Equal(2,actual2.Id);
            Assert.Equal("Physics Project", actual2.Title);
            Assert.Equal("Something about physics and stuff", actual2.Content);
            Assert.Equal(today, actual2.DateAdded); 
            Assert.Equal("1", actual2.SupervisorOid);
            Assert.Equal(2, actual2.Tags.Count);

        }
        //TODO
        [Fact]
        public async Task ReadAsyncBySupervisor_given_existing_supervisor_returns_posts()
        {
            var actual = await _repository.ReadAsyncBySupervisor("1");

            var actualStatus = actual.Item1;
            var actual1 = actual.Item2.ElementAt(0);
            var actual2 = actual.Item2.ElementAt(1);

            Assert.Equal(Success,actualStatus);

            Assert.Equal(1,actual1.Id);
            Assert.Equal("Math Project", actual1.Title);
            Assert.Equal("Bla bla bla bla", actual1.Content);
            Assert.Equal(today, actual1.DateAdded); 
            Assert.Equal("1", actual1.SupervisorOid);
            Assert.Equal(1, actual1.Tags.Count);

            Assert.Equal(2,actual2.Id);
            Assert.Equal("Physics Project", actual2.Title);
            Assert.Equal("Something about physics and stuff", actual2.Content);
            Assert.Equal(today, actual2.DateAdded); 
            Assert.Equal("1", actual2.SupervisorOid);
            Assert.Equal(2, actual2.Tags.Count);

        }


        [Fact]
        public async Task ReadAsyncBySupervisor_given_non_existing_id_returns_empty()
        {
            // var output = await _repository.ReadAsyncBySupervisor("11");
            // var actualStatus = output.Item1;
            // Assert.Equal(BadRequest,actualStatus);

            var i = await _repository.testUsers();
            Assert.Equal(null,i);
        }

        

        //TODO
        [Fact]
        public async Task ReadAsyncByTag_given_tag_math_returns_post()
        {
            //var actual = await _repository.ReadAsyncByTag("Math");
    
        }
        
        //TODO
        [Fact]
        public async Task ReadAsyncByTag_given_non_existing_tag_returns_none()
        {
            //var actual = await _repository.ReadAsyncByTag("Nothing");
    
        }


        [Fact]
        public async Task ReadAsyncComments_given_postid_returns_comments()
        {
            var (status, actual) = await _repository.ReadAsyncComments(1);
            var expected = new List<CommentDto>()
            {
                new(1, "Hello", new DateTime(2021,12,6),"1")

            }.AsReadOnly();
            Assert.Equal(Success, status);
            Assert.Equal(expected, actual);
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
