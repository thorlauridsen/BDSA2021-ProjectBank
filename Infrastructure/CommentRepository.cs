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
                User = await GetUserAsync(comment.UserId),
                DateAdded = DateTime.Now,
                Post = await GetPostAsync(comment.PostId)
            };
            _context.Comments.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new CommentDetailsDto(
                                 entity.Id,
                                 entity.Content,
                                 entity.DateAdded,
                                 entity.User.oid,
                                 entity.Post.Id
                             ));
        }

        public async Task<Option<CommentDetailsDto>> ReadAsync(int commentId) =>
            await _context.Comments.Where(c => c.Id == commentId)
                        .Select(c => new CommentDetailsDto(
                            c.Id,
                            c.Content,
                            c.DateAdded,
                            c.User.oid,
                            c.Post.Id
                        ))
                        .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<CommentDto>> ReadAsync() =>
            (await _context.Comments
                           .Select(c => new CommentDto(
                               c.Id,
                               c.Content,
                               c.DateAdded,
                               c.User.oid,
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

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);
    }
}
