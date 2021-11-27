namespace ProjectBank.Core
{
    public interface IPostRepository
    {
        Task<PostDetailsDto> CreateAsync(PostCreateDto post);
        Task<Option<PostDetailsDto>> ReadAsync(int postId);
        Task<IReadOnlyCollection<PostDto>> ReadAsync();
        Task<IReadOnlyCollection<PostDto>> ReadAsyncByTag(string tag);
        Task<IReadOnlyCollection<PostDto>> ReadAsyncBySupervisor(int supervisorId);
        Task<IReadOnlyCollection<CommentDto>> ReadAsyncComments(int postId);
        Task<Status> UpdateAsync(int id, PostUpdateDto post);
        Task<Status> DeleteAsync(int postId);
    }
}