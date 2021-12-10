namespace ProjectBank.Core
{
    public interface IUserRepository
    {
        Task<(Status, UserDetailsDto?)> CreateAsync(UserCreateDto user);
        Task<Status> DeleteAsync(string userOid);
        Task<Option<UserDetailsDto>> ReadAsync(string userOid);
        Task<IReadOnlyCollection<UserDetailsDto>> ReadAsync();
    }
}
