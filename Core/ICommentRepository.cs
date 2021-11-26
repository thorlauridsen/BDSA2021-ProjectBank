namespace ProjectBank.Core
{
    public interface ICommentRepository
    {
        Task<CommentDetailsDto> CreateAsync(CommentCreateDto comment);
        Task<Option<CommentDetailsDto>> ReadAsync(int commentId);
        Task<IReadOnlyCollection<CommentDto>> ReadAsync();
        Task<Status> UpdateAsync(int id, CommentUpdateDto comment);
        Task<Status> DeleteAsync(int commentId);

    }
}