namespace ProjectBank.Core
{
    public interface ICommentRepository
    {
        Task<(Status, CommentDetailsDto?)> CreateAsync(CommentCreateDto comment);
        Task<Status> DeleteAsync(int postId, int commentId);
        Task<Option<CommentDetailsDto>> ReadAsync(int postId, int commentId);
    }
}
