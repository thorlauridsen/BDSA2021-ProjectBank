using ProjectBank.Core;

namespace ProjectBank.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly IProjectBankContext _context;

        public UserRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<UserDetailsDto> CreateAsync(UserCreateDto user)
        {
            var entity = new User { Name = user.Name };

            _context.Users.Add(entity);

            await _context.SaveChangesAsync();

            return new UserDetailsDto(
                                 entity.Id,
                                 entity.Name
                             );
        }

        public async Task<Option<UserDetailsDto>> ReadAsync(int userId)
        {
            var users = from s in _context.Users
                        where s.Id == userId
                        select new UserDetailsDto(
                              s.Id,
                              s.Name
                        );

            return await users.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<UserDto>> ReadAsync() =>
            (await _context.Users
                           .Select(s => new UserDto(
                                s.Id,
                                s.Name
                            ))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int id, UserUpdateDto user)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(s => s.Id == user.Id);

            if (entity == null)
            {
                return NotFound;
            }

            entity.Name = user.Name;

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
