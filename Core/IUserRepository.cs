namespace ProjectBank.Core
{
    public interface IUserRepository
    {
        Task<UserDetailsDto> CreateAsync(UserCreateDto user);
        Task<Status> DeleteAsync(int userId);
        Task<Option<UserDetailsDto>> ReadAsync(int userId);
        Task<IReadOnlyCollection<UserDto>> ReadAsync();
        Task<Status> UpdateAsync(int userId, UserUpdateDto user);
    }
}
