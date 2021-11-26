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
            var entity = new Post(
                post.Title,
                post.Content,
                DateTime.Now,
                await GetSupervisorAsync(post.SupervisorId),
                await GetTagsAsync(post.Tags).ToListAsync(),
                await GetCommentsAsync(post.Comments).ToListAsync()
            );

            _context.Posts.Add(entity);

            await _context.SaveChangesAsync();

            return new PostDetailsDto(
                entity.Id,
                entity.Title,
                entity.Content,
                entity.DateAdded,
                entity.Author.Id,
                entity.Tags.Select(t => t.Name).ToHashSet(),
                entity.Comments.Select(c => c.Id).ToHashSet()
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
                            p.Author.Id,
                            p.Tags.Select(t => t.Name).ToHashSet(),
                            p.Comments.Select(c => c.Id).ToHashSet()
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
                                p.Author.Id,
                                p.Tags.Select(t => t.Name).ToHashSet(),
                                p.Comments.Select(c => c.Id).ToHashSet()
                            ))
                           .ToListAsync())
                           .AsReadOnly();
        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncBySupervisor(int supervisorId) =>
            (await _context.Posts
                           .Select(p => new PostDto(
                                p.Id,
                                p.Title,
                                p.Content,
                                p.DateAdded,
                                p.Author.Id,
                                p.Tags.Select(t => t.Name).ToHashSet(),
                                p.Comments.Select(c => c.Id).ToHashSet()
                            ))
                           .Where(p => p.SupervisorId == supervisorId)
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncByTag(string tag) =>
            (await _context.Posts
                           .Select(p => new PostDto(
                                p.Id,
                                p.Title,
                                p.Content,
                                p.DateAdded,
                                p.Author.Id,
                                p.Tags.Select(t => t.Name).ToHashSet(),
                                p.Comments.Select(c => c.Id).ToHashSet()
                            ))
                           .Where(p => p.Tags.Contains(tag))
                           .ToListAsync())
                           .AsReadOnly();
        public async Task<Status> UpdateAsync(int id, PostUpdateDto post)
        {
            var entity = await _context.Posts.FirstOrDefaultAsync(c => c.Id == post.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Title = post.Title;
            entity.Content = post.Content;
            entity.Author = await GetSupervisorAsync(post.SupervisorId);
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

        private async Task<Supervisor> GetSupervisorAsync(int supervisorId) =>
            await _context.Supervisors.FirstAsync(c => c.Id == supervisorId);

        private async IAsyncEnumerable<Tag> GetTagsAsync(IEnumerable<string> tags)
        {
            var existing = await _context.Tags.Where(t => tags.Contains(t.Name)).ToDictionaryAsync(t => t.Name);

            foreach (var tag in tags)
            {
                yield return existing.TryGetValue(tag, out var t) ? t : new Tag(tag);
            }
        }

        private async IAsyncEnumerable<Comment> GetCommentsAsync(IEnumerable<int> comments)
        {
            var existing = await _context.Comments.Where(c => comments.Contains(c.Id)).ToDictionaryAsync(c => c.Id);

            foreach (var comment in comments)
            {
                yield return existing.TryGetValue(comment, out var c) ? c : null;
            }
        }
    }
}
