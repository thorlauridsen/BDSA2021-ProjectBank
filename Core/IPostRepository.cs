namespace ProjectBank.Core
{
    public interface IPostRepository
    {
        Task<PostDetailsDto> CreateAsync(PostCreateDto post);
        Task<Option<PostDetailsDto>> ReadAsync(int postId);
        Task<IReadOnlyCollection<PostDto>> ReadAsync();
        Task<Status> UpdateAsync(int id, PostUpdateDto post);
        Task<Status> DeleteAsync(int postId);
    }
}