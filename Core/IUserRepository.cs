namespace ProjectBank.Core
{
    public interface IUserRepository
    {
        Task<UserDetailsDto> CreateAsync(UserCreateDto user);
        Task<Option<UserDetailsDto>> ReadAsync(int userId);
        Task<IReadOnlyCollection<UserDto>> ReadAsync();
        Task<Status> UpdateAsync(int id, UserUpdateDto user);
        Task<Status> DeleteAsync(int userId);
    }
}