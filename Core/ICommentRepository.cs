namespace ProjectBank.Core
{
    public interface ICommentRepository
    {
        Task<CommentDetailsDto> CreateAsync(CommentCreateDto comment);
        Task<Status> DeleteAsync(int commentId);
        Task<Option<CommentDetailsDto>> ReadAsync(int commentId);
        Task<IReadOnlyCollection<CommentDto>> ReadAsync();
        Task<Status> UpdateAsync(int commentId, CommentUpdateDto comment);
    }
}