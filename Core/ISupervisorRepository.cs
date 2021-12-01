namespace ProjectBank.Core
{
    public interface ISupervisorRepository
    {
        Task<SupervisorDetailsDto> CreateAsync(SupervisorCreateDto supervisor);
        Task<Option<SupervisorDetailsDto>> ReadAsync(int userId);
        Task<IReadOnlyCollection<SupervisorDto>> ReadAsync();
        Task<Status> UpdateAsync(int userId, SupervisorUpdateDto supervisor);
        Task<Status> DeleteAsync(int userId);
    }
}
