using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using static ProjectBank.Core.Status;

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.oid == comment.UserId);
            var postEntity = await _context.Posts.FirstOrDefaultAsync(c => c.Id == comment.postid);

            if (user == null ||
                postEntity == null ||
                comment.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }
            var entity = new Comment
            {
                Content = comment.Content,
                User = user,
                DateAdded = DateTime.Now
            };
            postEntity.Comments.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new CommentDetailsDto(
                                 entity.Id,
                                 entity.Content,
                                 entity.DateAdded,
                                 entity.User.oid
                             ));
        }

        public async Task<Option<CommentDetailsDto>> ReadAsync(int postId, int commentId)
        {
            var c = (await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId))?.Comments.FirstOrDefault(c => c.Id == commentId);
            if (c == null) return null;
            return new CommentDetailsDto(
                c.Id,
                c.Content,
                c.DateAdded,
                c.User.oid
            );
        }

        public async Task<Status> DeleteAsync(int postId, int commentId)
        {
            var postEntity = await _context.Posts.Include("Comments").FirstOrDefaultAsync(p => p.Id == postId).ConfigureAwait(false);

            if (postEntity == null)
            {
                return NotFound;
            }
            if (postEntity.Comments.All(c => c.Id != commentId))
            {
                return NotFound;
            }

            var entity = postEntity.Comments.First(c => c.Id == commentId);
            postEntity.Comments.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }
    }
}
