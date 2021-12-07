using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
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

        public async Task<(Status, PostDetailsDto?)> CreateAsync(PostCreateDto post)
        {
            if (post.Title.Trim().Equals("") ||
                post.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }

            var entity = new Post
            {
                Title = post.Title,
                Content = post.Content,
                DateAdded = DateTime.Now,
                User = await GetUserAsync(post.SupervisorOid),
                Tags = post.Tags.ToArray()
            };
            _context.Posts.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new PostDetailsDto(
                entity.Id,
                entity.Title,
                entity.Content,
                entity.DateAdded,
                entity.User.oid,
                entity.Tags.ToHashSet()
            ));
        }

        public async Task<Option<PostDetailsDto>> ReadAsync(int postId) =>
            await _context.Posts.Where(p => p.Id == postId)
                .Select(p => new PostDetailsDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.DateAdded,
                    p.User.oid,
                    p.Tags.ToHashSet()
                ))
                .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<PostDto>> ReadAsync() =>
            (await _context.Posts
                .Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.DateAdded,
                    p.User.oid,
                    p.Tags.ToHashSet()
                ))
                .ToListAsync())
            .AsReadOnly();


        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncBySupervisor(string userId) =>
            (await _context.Posts
                .Where(p => p.User.oid == userId)
                .Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.DateAdded,
                    p.User.oid,
                    p.Tags.ToHashSet()
                ))
                .ToListAsync())
            .AsReadOnly();

        //FIXME make tags a string in post, instead of it's own type?
        public async Task<IReadOnlyCollection<PostDto>> ReadAsyncByTag(string tag) =>
            (await _context.Posts
                .Where(p => p.Tags.Any(tag => tag.Equals(tag)))
                .Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Content,
                    p.DateAdded,
                    p.User.oid,
                    p.Tags.ToHashSet()
                ))
                .ToListAsync())
            .AsReadOnly();

        public async Task<(Status, IReadOnlyCollection<CommentDto>)> ReadAsyncComments(int postId)
        {
            var post = await _context.Posts.Include("Comments.User").FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
            {
                return (BadRequest, new List<CommentDto>() { });
            }

            var comments = post.Comments;
            var result = new List<CommentDto>();
            foreach (var comment in comments)
            {
                var commentDto = new CommentDto(
                    comment.Id,
                    comment.Content,
                    comment.DateAdded,
                    comment.User.oid);
                result.Add(commentDto);
            }
            return (Success, result);
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

        private async IAsyncEnumerable<Tag> GetTagsAsync(IEnumerable<string> tags)
        {
            var existing = await _context.Tags.Where(t => tags.Contains(t.Name))
                .ToDictionaryAsync(t => t.Name);

            foreach (var tag in tags)
            {
                yield return existing.TryGetValue(tag, out var t) ? t : new Tag(tag);
            }
        }

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);
    }
}