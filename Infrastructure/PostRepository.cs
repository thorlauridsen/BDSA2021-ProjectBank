using Microsoft.EntityFrameworkCore;
using ProjectBank.Infrastructure;
using static ProjectBank.Core.Status;

namespace ProjectBank.Core
{
    public class PostRepository : IPostRepository
    {
        private readonly IProjectBankContext _context;

        public PostRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<(Status, PostDetailsDto?)> CreateAsync(PostCreateDto post)
        {
            var user = await GetUserAsync(post.UserOid);

            if (user == null ||
                post.Title.Trim().Equals("") ||
                post.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }

            var entity = new Post
            {
                Title = post.Title,
                Content = post.Content,
                DateAdded = DateTime.Now,
                User = user,
                Tags = post.Tags.ToArray(),
                PostState = PostState.Active
            };
            _context.Posts.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new PostDetailsDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                DateAdded = entity.DateAdded,
                UserOid = entity.User.Oid,
                Tags = entity.Tags.ToHashSet(),
                PostState = entity.PostState,
                ViewCount = entity.ViewCount
            });
        }

        public async Task<Option<PostDetailsDto>> ReadAsync(int postId) =>
            await _context.Posts.Where(p => p.Id == postId)
                .Select(p => new PostDetailsDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    DateAdded = p.DateAdded,
                    UserOid = p.User.Oid,
                    Tags = p.Tags.ToHashSet(),
                    PostState = p.PostState,
                    ViewCount = p.ViewCount
                })
                .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<PostDetailsDto>> ReadAsync() =>
            (await _context.Posts
                .Select(p => new PostDetailsDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    DateAdded = p.DateAdded,
                    UserOid = p.User.Oid,
                    Tags = p.Tags.ToHashSet(),
                    PostState = p.PostState,
                    ViewCount = p.ViewCount
                })
                .ToListAsync())
            .AsReadOnly();


        public async Task<(Status, IReadOnlyCollection<PostDetailsDto>)> ReadAsyncBySupervisor(string userOid)
        {
            var user = await GetUserAsync(userOid);
            if (user == null)
            {
                return (NotFound, new List<PostDetailsDto>() { });
            }

            var posts = (await _context.Posts
                .Where(p => p.User.Oid == userOid)
                .Select(p => new PostDetailsDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    DateAdded = p.DateAdded,
                    UserOid = p.User.Oid,
                    Tags = p.Tags.ToHashSet(),
                    PostState = p.PostState,
                    ViewCount = p.ViewCount
                })
                .ToListAsync())
            .AsReadOnly();

            return (Success, posts);
        }

        public async Task<IReadOnlyCollection<PostDetailsDto>> ReadAsyncByTag(string tag)
        {
            var list = new List<PostDetailsDto>();
            foreach (var p in _context.Posts)
            {
                var tags = p.Tags?.ToList();

                if (tags != null)
                {
                    if (tags.Contains(tag))
                    {
                        list.Add(new PostDetailsDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Content = p.Content,
                            DateAdded = p.DateAdded,
                            UserOid = p.User.Oid,
                            Tags = p.Tags.ToHashSet(),
                            PostState = p.PostState,
                            ViewCount = p.ViewCount
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IReadOnlyCollection<CommentDetailsDto>> ReadAsyncComments(int postId)
        {
            var post = await _context.Posts.Include("Comments.User")
                                           .FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                return new List<CommentDetailsDto>() { };
            }

            return post.Comments.Select(comment => new CommentDetailsDto
            {
                Id = comment.Id,
                Content = comment.Content,
                DateAdded = comment.DateAdded,
                UserOid = comment.User.Oid

            }).ToList();
        }

        public async Task<Status> UpdateAsync(int postId, PostUpdateDto post)
        {
            var entity = await _context.Posts.FirstOrDefaultAsync(c => c.Id == post.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Title = post.Title;
            entity.Content = post.Content;
            entity.Tags = post.Tags.ToArray();
            entity.PostState = post.PostState;
            entity.ViewCount = post.ViewCount;

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(int postId)
        {
            var entity = await _context.Posts.Include("Comments").FirstOrDefaultAsync(p=> p.Id == postId);

            if (entity == null)
            {
                return NotFound;
            }

            var chats = await _context.Chats.Include("ChatUsers").Include("Post").Where(c => c.Post != null && c.Post.Id == postId).ToListAsync();
            foreach (var chat in chats)
            {
                var chatMessages = await _context.ChatMessages.Include("Chat").Where(cm => cm.Chat.Id == chat.Id).ToListAsync();
                _context.ChatMessages.RemoveRange(chatMessages);
                _context.Chats.Remove(chat);   
            }
            await _context.SaveChangesAsync();
            
            _context.Posts.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }

        public async Task<(Status, int)> IncrementViewCountAsync(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) return (NotFound, -1);

            return (Success, ++post.ViewCount);
        }

        private async Task<User?> GetUserAsync(string userOid) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Oid == userOid);
    }
}
