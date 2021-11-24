namespace ProjectBank.Core
{
    public interface IStudentRepository
    {
        Task<StudentDetailsDto> CreateAsync(StudentCreateDto student);
        Task<Option<StudentDetailsDto>> ReadAsync(int studentId);
        Task<IReadOnlyCollection<StudentDto>> ReadAsync();
        Task<Status> UpdateAsync(int id, StudentUpdateDto student);
        Task<Status> DeleteAsync(int studentId);
    }
}