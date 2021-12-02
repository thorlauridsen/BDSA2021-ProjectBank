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
                                 entity.Name,
                                 entity.IsSupervisor
                             );
        }

        public async Task<Option<UserDetailsDto>> ReadAsync(int userId) =>
            await _context.Users.Where(u => u.Id == userId)
                                .Select(u => new UserDetailsDto(
                                    u.Id,
                                    u.Name,
                                    u.IsSupervisor
                                ))
                                .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<UserDto>> ReadAsync() =>
            (await _context.Users
                           .Select(u => new UserDto(
                                u.Id,
                                u.Name,
                                u.IsSupervisor
                            ))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(int userId, UserUpdateDto user)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (entity == null)
            {
                return NotFound;
            }
            entity.Name = user.Name;
            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(int userId)
        {
            var entity = await _context.Users.FindAsync(userId);

            if (entity == null)
            {
                return NotFound;
            }
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();

            return Deleted;
        }
    }
}
