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

        public async Task<(Status, CommentDetailsDto?)> CreateAsync(CommentCreateDto comment)
        {
            if (comment.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }
            var entity = new Comment
            {
                Content = comment.Content,
                UserId = comment.UserId,
                DateAdded = DateTime.Now,
                PostId = comment.PostId
            };
            _context.Comments.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new CommentDetailsDto(
                                 entity.Id,
                                 entity.Content,
                                 entity.DateAdded,
                                 entity.UserId,
                                 entity.PostId
                             ));
        }

        public async Task<Option<CommentDetailsDto>> ReadAsync(int commentId) =>
            await _context.Comments.Where(c => c.Id == commentId)
                        .Select(c => new CommentDetailsDto(
                            c.Id,
                            c.Content,
                            c.DateAdded,
                            c.UserId,
                            c.PostId
                        ))
                        .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<CommentDto>> ReadAsync() =>
            (await _context.Comments
                           .Select(c => new CommentDto(
                               c.Id,
                               c.Content,
                               c.DateAdded,
                               c.UserId,
                               c.PostId
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
