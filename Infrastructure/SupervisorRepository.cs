using ProjectBank.Infrastructure;

namespace ProjectBank.Core
{
    public class SupervisorRepository : ISupervisorRepository
    {
        private readonly IProjectBankContext _context;

        public SupervisorRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<SupervisorDetailsDto> CreateAsync(SupervisorCreateDto supervisor)
        {
            var entity = new Supervisor(supervisor.Name, supervisor.Salary);

            _context.Supervisors.Add(entity);

            await _context.SaveChangesAsync();

            return new SupervisorDetailsDto(
                                 entity.Id,
                                 entity.Name,
                                 entity.Salary
                             );
        }

        public async Task<Option<SupervisorDetailsDto>> ReadAsync(int supervisorId)
        {
            var supervisors = from c in _context.Supervisors
                              where c.Id == supervisorId
                              select new SupervisorDetailsDto(
                                  c.Id,
                                  c.Name,
                                  c.Salary
                              );

            return await supervisors.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<SupervisorDto>> ReadAsync() =>
            (await _context.Supervisors
                           .Select(c => new SupervisorDto(c.Id, c.Name, c.Salary))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int id, SupervisorUpdateDto supervisor)
        {
            var entity = await _context.Supervisors.FirstOrDefaultAsync(c => c.Id == supervisor.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Name = supervisor.Name;
            entity.Salary = supervisor.Salary;

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