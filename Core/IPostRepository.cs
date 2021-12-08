namespace ProjectBank.Core
{
    public interface IPostRepository
    {
        Task<(Status, PostDetailsDto?)> CreateAsync(PostCreateDto post);
        Task<Status> DeleteAsync(int postId);
        Task<Option<PostDetailsDto>> ReadAsync(int postId);
        Task<IReadOnlyCollection<PostDto>> ReadAsync();
        Task<(Status, IReadOnlyCollection<PostDto>)> ReadAsyncBySupervisor(string userId);
        Task<IReadOnlyCollection<PostDto>> ReadAsyncByTag(string tag);
        Task<IReadOnlyCollection<CommentDto>> ReadAsyncComments(int postId);
        Task<Status> UpdateAsync(int postId, PostUpdateDto post);
    }
}
