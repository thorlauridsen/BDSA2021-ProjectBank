namespace ProjectBank.Core
{
    public interface IUserRepository
    {
        Task<(Status, UserDetailsDto?)> CreateAsync(UserCreateDto user);
        Task<Status> DeleteAsync(string userId);
        Task<Option<UserDetailsDto>> ReadAsync(string userId);
        Task<IReadOnlyCollection<UserDto>> ReadAsync();
        Task<Status> UpdateAsync(string userId, UserUpdateDto user);
    }
}
