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

            var user = new User { oid = "1", Name = "Bob" };
            var userNoPosts = new User { oid = "6", Name = "Carl" };

            context.Users.Add(userNoPosts);
            context.Users.Add(user);

            var comment = new Comment
            {
                Id = 1,
                Content = "Hello",
                User = user,
                DateAdded = new DateTime(2021, 12, 6)
            };

            var post1 = new Post
            {
                Id = 1,
                Title = "Math Project",
                Content = "Bla bla bla bla",
                DateAdded = today,
                Comments = new List<Comment>() { comment },
                User = user,
                Tags = new string[] { "Math" },
                PostState = PostState.Active,
                ViewCount = 22
            };
            var post2 = new Post
            {
                Id = 2,
                Title = "Physics Project",
                Content = "Something about physics and stuff",
                DateAdded = today,
                User = user,
                Tags = new string[] { "Science", "Physics" },
                PostState = PostState.Active,
                ViewCount = 13
            };

            context.Posts.Add(post1);
            context.Posts.Add(post2);
            context.SaveChanges();

            _context = context;
            _repository = new PostRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_creates_new_post_with_generated_id_then_update_then_delete()
        {
            var post = new PostCreateDto
            {
                Title = "Biology Project",
                Content = "Bla bla bla bla",
                SupervisorOid = "1",
                Tags = new HashSet<string> { "bio", "dna", "cells" }
            };

            var (status, content) = await _repository.CreateAsync(post);

            Assert.Equal(Created, status);
            Assert.NotNull(content);
            Assert.Equal(3, content?.Id);
            Assert.Equal("Biology Project", content?.Title);
            Assert.Equal("Bla bla bla bla", content?.Content);
            Assert.Equal("1", content?.SupervisorOid);
            Assert.True(content?.Tags.SetEquals(new[] { "bio", "dna", "cells" }));
            Assert.Equal(PostState.Active, content?.PostState);
            Assert.Equal(0, content?.ViewCount);

            var updatePost = new PostUpdateDto
            {
                Id = 3,
                Title = "Biology Project 2",
                Content = "New content but archived",
                SupervisorOid = "1",
                Tags = new HashSet<string> { "bio", "dna", "cells" },
                PostState = PostState.Archived,
                ViewCount = 1
            };

            var updateStatus = await _repository.UpdateAsync(3, updatePost);
            Assert.Equal(Updated, updateStatus);

            var option = await _repository.ReadAsync(3);

            Assert.Equal("Biology Project 2", option.Value.Title);
            Assert.Equal("New content but archived", option.Value.Content);
            Assert.Equal(PostState.Archived, option.Value.PostState);
            Assert.Equal(1, option.Value.ViewCount);

            var response = await _repository.DeleteAsync(3);
            Assert.Equal(Deleted, response);
        }

        [Fact]
        public async Task CreateAsync_without_content_returns_BadRequest()
        {
            var post = new PostCreateDto
            {
                Title = "",
                Content = "",
                SupervisorOid = "1",
                Tags = new HashSet<string> { "bio", "dna", "cells" }
            };
            var (status, content) = await _repository.CreateAsync(post);

            Assert.Equal(BadRequest, status);
            Assert.Null(content);
        }

        [Fact]
        public async Task CreateAsync_non_existing_user_returns_BadRequest()
        {
            var post = new PostCreateDto
            {
                Title = "Biology Project",
                Content = "Bla bla bla bla",
                SupervisorOid = "111111",
                Tags = new HashSet<string> { "bio", "dna", "cells" }
            };
            var (status, content) = await _repository.CreateAsync(post);

            Assert.Equal(BadRequest, status);
            Assert.Null(content);
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
            Assert.Equal(PostState.Active, option.Value.PostState);
            Assert.Equal(22, option.Value.ViewCount);
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
            var posts = await _repository.ReadAsync();

            Assert.NotNull(posts);
            Assert.Equal(2, posts.Count());

            var post1 = posts.ElementAt(0);
            var post2 = posts.ElementAt(1);

            Assert.Equal(1, post1.Id);
            Assert.Equal("Math Project", post1.Title);
            Assert.Equal("Bla bla bla bla", post1.Content);
            Assert.Equal(today, post1.DateAdded);
            Assert.Equal("1", post1.SupervisorOid);
            Assert.Equal(1, post1.Tags.Count);
            Assert.Equal(PostState.Active, post1.PostState);
            Assert.Equal(22, post1.ViewCount);

            Assert.Equal(2, post2.Id);
            Assert.Equal("Physics Project", post2.Title);
            Assert.Equal("Something about physics and stuff", post2.Content);
            Assert.Equal(today, post2.DateAdded);
            Assert.Equal("1", post2.SupervisorOid);
            Assert.Equal(2, post2.Tags.Count);
            Assert.Equal(PostState.Active, post2.PostState);
            Assert.Equal(13, post2.ViewCount);
        }

        //TODO
        [Fact]
        public async Task ReadAsyncBySupervisor_given_existing_supervisor_returns_posts()
        {
            var (status, content) = await _repository.ReadAsyncBySupervisor("1");

            Assert.NotNull(content);

            var post1 = content.ElementAt(0);
            var post2 = content.ElementAt(1);

            Assert.Equal(1, post1.Id);
            Assert.Equal("Math Project", post1.Title);
            Assert.Equal("Bla bla bla bla", post1.Content);
            Assert.Equal(today, post1.DateAdded);
            Assert.Equal("1", post1.SupervisorOid);
            Assert.Equal(1, post1.Tags.Count);
            Assert.Equal(PostState.Active, post1.PostState);
            Assert.Equal(22, post1.ViewCount);

            Assert.Equal(2, post2.Id);
            Assert.Equal("Physics Project", post2.Title);
            Assert.Equal("Something about physics and stuff", post2.Content);
            Assert.Equal(today, post2.DateAdded);
            Assert.Equal("1", post2.SupervisorOid);
            Assert.Equal(2, post2.Tags.Count);
            Assert.Equal(PostState.Active, post2.PostState);
            Assert.Equal(13, post2.ViewCount);
        }

        [Fact]
        public async Task ReadAsyncBySupervisor_given_non_existing_id_returns_null()
        {
            var (status, content) = await _repository.ReadAsyncBySupervisor("11");
            Assert.Equal(NotFound, status);
            Assert.Empty(content);
        }

        [Fact]
        public async Task ReadAsyncBySupervisor_given_existing_id_but_no_posts_returns_empty()
        {
            var (status, content) = await _repository.ReadAsyncBySupervisor("6");
            Assert.Equal(Success, status);
            Assert.Empty(content);
        }

        [Fact]
        public async Task ReadAsyncByTag_given_tag_math_returns_post()
        {
            var actual = (await _repository.ReadAsyncByTag("Math")).ElementAt(0);

            Assert.Equal(1, actual.Id);
            Assert.Equal("Math Project", actual.Title);
            Assert.Equal("Bla bla bla bla", actual.Content);
            Assert.Equal(today, actual.DateAdded);
            Assert.Equal("1", actual.SupervisorOid);
            Assert.Equal(1, actual.Tags.Count);
        }

        //TODO
        [Fact]
        public async Task ReadAsyncByTag_given_non_existing_tag_returns_none()
        {
            //var actual = await _repository.ReadAsyncByTag("Nothing");
        }

        [Fact]
        public async Task ReadAsyncComments_given_postId_returns_comments()
        {
            var response = await _repository.ReadAsyncComments(1);
            var comment = new CommentDto(1, "Hello", new DateTime(2021, 12, 6), "1");
            var expected = new List<CommentDto>() { comment }.AsReadOnly();
            Assert.Equal(expected, response);
        }

        [Fact]
        public async Task ReadAsyncComments_given_non_existing_Id_returns_comments()
        {
            var actual = await _repository.ReadAsyncComments(11);
            Assert.Empty(actual);
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
