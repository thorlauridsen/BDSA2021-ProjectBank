using ProjectBank.Infrastructure;

namespace ProjectBank.Core
{
    public class PostRepository : IPostRepository
    {
        private readonly IProjectBankContext _context;

        public PostRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<PostDetailsDto> CreateAsync(PostCreateDto post)
        {
            var entity = new Post
            {
                Title = post.Title,
                Content = post.Content,
                DateAdded = DateTime.Now,
                User = await GetUserAsync(post.SupervisorId),
                Tags = await GetTagsAsync(post.Tags).ToListAsync()
            };

            _context.Posts.Add(entity);

            await _context.SaveChangesAsync();

            return new PostDetailsDto(
                entity.Id,
                entity.Title,
                entity.Content,
                entity.DateAdded,
                entity.User.Id,
                entity.Tags.Select(t => t.Name).ToHashSet()
            );
        }

        public async Task<Option<PostDetailsDto>> ReadAsync(int postId)
        {
            var posts = from p in _context.Posts
                        where p.Id == postId
                        select new PostDetailsDto(
                            p.Id,
                            p.Title,
                            p.Content,
                            p.DateAdded,
                            p.User.Id,
                            p.Tags.Select(t => t.Name).ToHashSet()
                        );

            return await posts.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<PostDto>> ReadAsync() =>
            (await _context.Posts
                           .Select(p => new PostDto(
                                p.Id,
                                p.Title,
                                p.Content,
                                p.DateAdded,
                                p.User.Id,
                                p.Tags.Select(t => t.Name).ToHashSet()
                            ))
                           .ToListAsync())
                           .AsReadOnly();
        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncBySupervisor(int userId) =>
            (await _context.Posts
                           .Select(p => new PostDto(
                                p.Id,
                                p.Title,
                                p.Content,
                                p.DateAdded,
                                p.User.Id,
                                p.Tags.Select(t => t.Name).ToHashSet()
                            ))
                           .Where(p => p.SupervisorId == userId)
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncByTag(string tag) =>
            (await _context.Posts
                           .Select(p => new PostDto(
                                p.Id,
                                p.Title,
                                p.Content,
                                p.DateAdded,
                                p.User.Id,
                                p.Tags.Select(t => t.Name).ToHashSet()
                            ))
                           .Where(p => p.Tags.Contains(tag))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<IReadOnlyCollection<CommentDto>> ReadAsyncComments(int postId)
        {
            var comments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();

            var result = new List<CommentDto>();
            foreach (var comment in comments)
            {
                result.Add(new CommentDto(
                    comment.Id,
                    comment.Content,
                    comment.DateAdded,
                    comment.UserId,
                    comment.PostId
                ));
            }
            return result;
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
            entity.Tags = await GetTagsAsync(post.Tags).ToListAsync();

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(int postId)
        {
            var entity = await _context.Posts.FindAsync(postId);

            if (entity == null)
            {
                return NotFound;
            }

            _context.Posts.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }

        private async Task<Supervisor> GetSupervisorAsync(int userId) =>
            await _context.Supervisors.FirstAsync(c => c.Id == userId);

        private async IAsyncEnumerable<Tag> GetTagsAsync(IEnumerable<string> tags)
        {
            var existing = await _context.Tags.Where(t => tags.Contains(t.Name))
                                              .ToDictionaryAsync(t => t.Name);

            foreach (var tag in tags)
            {
                yield return existing.TryGetValue(tag, out var t) ? t : new Tag(tag);
            }
        }

        private async Task<User> GetUserAsync(int userId) =>
            await _context.Users.FirstAsync(u => u.Id == userId);
    }
}
