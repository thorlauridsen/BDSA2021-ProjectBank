namespace ProjectBank.Core
{
    public interface IStudentRepository
    {
        Task<StudentDetailsDto> CreateAsync(StudentCreateDto student);
        Task<Option<StudentDetailsDto>> ReadAsync(int userId);
        Task<IReadOnlyCollection<StudentDto>> ReadAsync();
        Task<Status> UpdateAsync(int userId, StudentUpdateDto student);
        Task<Status> DeleteAsync(int userId);
    }
}