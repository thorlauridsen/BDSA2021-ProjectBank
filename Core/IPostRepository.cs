namespace ProjectBank.Core
{
    public interface IPostRepository
    {
        Task<(Status, PostDetailsDto?)> CreateAsync(PostCreateDto post);
        Task<Status> DeleteAsync(int postId);
        Task<(Status, int)> IncrementViewCountAsync(int postId);
        Task<Option<PostDetailsDto>> ReadAsync(int postId);
        Task<IReadOnlyCollection<PostDetailsDto>> ReadAsync();
        Task<(Status, IReadOnlyCollection<PostDetailsDto>)> ReadAsyncBySupervisor(string userOid);
        Task<IReadOnlyCollection<PostDetailsDto>> ReadAsyncByTag(string tag);
        Task<IReadOnlyCollection<CommentDetailsDto>> ReadAsyncComments(int postId);
        Task<Status> UpdateAsync(int postId, PostUpdateDto post);
    }
}
