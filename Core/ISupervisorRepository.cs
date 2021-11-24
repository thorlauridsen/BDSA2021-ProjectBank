namespace ProjectBank.Core
{
    public interface ISupervisorRepository
    {
        Task<SupervisorDetailsDto> CreateAsync(SupervisorCreateDto supervisor);
        Task<Option<SupervisorDetailsDto>> ReadAsync(int supervisorId);
        Task<IReadOnlyCollection<SupervisorDto>> ReadAsync();
        Task<Status> UpdateAsync(int id, SupervisorUpdateDto supervisor);
        Task<Status> DeleteAsync(int supervisorId);
    }
}