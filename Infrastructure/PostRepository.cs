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
            var entity = new Post(post.Title, post.Content);

            _context.Posts.Add(entity);

            await _context.SaveChangesAsync();

            return new PostDetailsDto(
                                 entity.Id,
                                 entity.Title,
                                 entity.Content
                             );
        }

        public async Task<Option<PostDetailsDto>> ReadAsync(int postId)
        {
            var posts = from c in _context.Posts
                        where c.Id == postId
                        select new PostDetailsDto(
                            c.Id,
                            c.Title,
                            c.Content
                        );

            return await posts.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<PostDto>> ReadAsync() =>
            (await _context.Posts
                           .Select(c => new PostDto(c.Id, c.Title, c.Content))
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
    }
}
