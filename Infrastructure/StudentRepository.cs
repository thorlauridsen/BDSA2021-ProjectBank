using ProjectBank.Core;

namespace ProjectBank.Infrastructure
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IProjectBankContext _context;

        public StudentRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<StudentDetailsDto> CreateAsync(StudentCreateDto student)
        {
            var entity = new Student { Name = student.Name };

            _context.Students.Add(entity);

            await _context.SaveChangesAsync();

            return new StudentDetailsDto(
                entity.Id,
                entity.Name
            );
        }

        public async Task<Option<StudentDetailsDto>> ReadAsync(int studentId)
        {
            var students = from c in _context.Students
                           where c.Id == studentId
                           select new StudentDetailsDto(
                               c.Id,
                               c.Name
                           );

            return await students.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<StudentDto>> ReadAsync() =>
            (await _context.Students
                           .Select(c => new StudentDto(c.Id, c.Name))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int id, StudentUpdateDto student)
        {
            var entity = await _context.Students.FirstOrDefaultAsync(c => c.Id == student.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Name = student.Name;

            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(int studentId)
        {
            var entity = await _context.Students.FindAsync(studentId);

            if (entity == null)
            {
                return NotFound;
            }

            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }
    }
}
