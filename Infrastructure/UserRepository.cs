using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using static ProjectBank.Core.Status;

namespace ProjectBank.Infrastructure
{

    public class UserRepository : IUserRepository
    {
        private readonly IProjectBankContext _context;
        private HttpClient _http;

        public UserRepository(IProjectBankContext context)
        {
            _context = context;
            _http = new HttpClient();
        }

        public async Task<(Status, UserDetailsDto?)> CreateAsync(UserCreateDto user)
        {
            var name = user.Name.Replace(" ", "+");
            var image = await _http.GetByteArrayAsync($"https://eu.ui-avatars.com/api/?name={name}&background=random");
            string base64Image = "data:image/png;base64," + Convert.ToBase64String(image);
            var entity = new User
            {
                oid = user.oid,
                Image = base64Image,
                Name = user.Name
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new UserDetailsDto(
                                 entity.oid,
                                 entity.Name,
                                 entity.Image
                             ));
        }

        public async Task<Option<UserDetailsDto>> ReadAsync(string userId) =>
            await _context.Users.Where(u => u.oid == userId)
                                .Select(u => new UserDetailsDto(
                                    u.oid,
                                    u.Name,
                                    u.Image
                                ))
                                .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<UserDto>> ReadAsync() =>
            (await _context.Users
                           .Select(u => new UserDto(
                                u.oid,
                                u.Name
                            ))
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> UpdateAsync(string userId, UserUpdateDto user)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.oid == user.oid);

            if (entity == null)
            {
                return NotFound;
            }
            entity.Name = user.Name;
            await _context.SaveChangesAsync();

            return Updated;
        }

        public async Task<Status> DeleteAsync(string userId)
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
