using ProjectBank.Core;

namespace ProjectBank.Infrastructure
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
            var entity = new Supervisor { Name = supervisor.Name };

            _context.Supervisors.Add(entity);

            await _context.SaveChangesAsync();

            return new SupervisorDetailsDto(
                                 entity.Id,
                                 entity.Name
                             );
        }

        public async Task<Option<SupervisorDetailsDto>> ReadAsync(int supervisorId)
        {
            var supervisors = from s in _context.Supervisors
                              where s.Id == supervisorId
                              select new SupervisorDetailsDto(
                                    s.Id,
                                    s.Name
                              );

            return await supervisors.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<SupervisorDto>> ReadAsync() =>
            (await _context.Supervisors
                           .Select(s => new SupervisorDto(
                                s.Id,
                                s.Name
                            ))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int id, SupervisorUpdateDto supervisor)
        {
            var entity = await _context.Supervisors.FirstOrDefaultAsync(s => s.Id == supervisor.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Name = supervisor.Name;

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
