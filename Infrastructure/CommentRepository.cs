using ProjectBank.Core;

namespace ProjectBank.Infrastructure
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IProjectBankContext _context;

        public CommentRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<CommentDetailsDto> CreateAsync(CommentCreateDto comment)
        {
            var entity = new Comment(
                comment.Content,
                await GetUserAsync(comment.UserId),
                comment.DateAdded,
                await GetPostAsync(comment.PostId)
            );

            _context.Comments.Add(entity);

            await _context.SaveChangesAsync();

            return new CommentDetailsDto(
                                 entity.Id,
                                 entity.Content,
                                 entity.DateAdded,
                                 entity.Author.Id,
                                 entity.Post.Id
                             );
        }

        public async Task<Option<CommentDetailsDto>> ReadAsync(int commentId)
        {
            var comments = from c in _context.Comments
                           where c.Id == commentId
                           select new CommentDetailsDto(
                               c.Id,
                               c.Content,
                               c.DateAdded,
                               c.Author.Id,
                               c.Post.Id
                           );

            return await comments.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<CommentDto>> ReadAsync() =>
            (await _context.Comments
                           .Select(c => new CommentDto(
                               c.Id,
                               c.Content,
                               c.DateAdded,
                               c.Author.Id,
                               c.Post.Id
                            ))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int id, CommentUpdateDto comment)
        {
            var entity = await _context.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Content = comment.Content;
            entity.DateAdded = comment.DateAdded;

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(int commentId)
        {
            var entity = await _context.Comments.FindAsync(commentId);

            if (entity == null)
            {
                return NotFound;
            }

            _context.Comments.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }

        private async Task<Post> GetPostAsync(int postId) =>
            await _context.Posts.FirstAsync(p => p.Id == postId);

        private async Task<User> GetUserAsync(int userId) =>
            await _context.Users.FirstAsync(u => u.Id == userId);
    }
}
