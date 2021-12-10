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
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAAACXBIWXMAAAsSAAALEgHS3X78AAACMVBMVEVHcEyYmJiYmJiYmJiZmZmYmJiYmJiYmJiZmZmYmJiZmZmZmZmZmZmYmJiZmZmZmZmYmJiYmJiampqYmJiZmZmYmJiampqYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiUlJSYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiSkpKYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiPj4+Ojo6YmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiXl5eYmJiYmJiYmJiXl5eYmJiYmJiYmJiXl5eYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiVlZWYmJiYmJiYmJiYmJiYmJiYmJiYmJiWlpaYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiYmJiZmZmioqKjo6Ofn5+ampqhoaGbm5ucnJykpKSdnZ2enp6goKCYmJhtUMYQAAAArnRSTlMAA/v8+gIB/v79+wL9BPwBQ6ECfgP5AaOn52F8ZHf69+yBBU8/xO1TtZqDAlaO0QSM9ciNIITwB53pmBHTwqTLlgyoVQ96JUI31coByb4Ox3O7XqaHKgoVoB8pr/IutNifHkx1AgGtsjscaaxK333zhiRncZUOGUcoLTIXIonlFsaSMAa6v2wzdnvN8R1vEl8EzzYabSvaFgVo7uNI4UsNQO8bOFFGgqqQgI9/6GszSDKLAAAD7klEQVRYw81W1ZbcRhCtaUF3azSz5PV6nUUzxcyMQYeZmZmZYwza4TjMqBYO+OtSrYUMSC0pT6kXndNn7p26xQD/YzNLlbny218x/gvcKMefBWviT7koh9EDcN3CFQ8tvnDnrqGB557BF7MIvgSwqG9J3Q28esMTbvTq0y/Gj3nFA9yyLBKhrRONc43oLBJnh26Acl73Ae4igU04pQ51GHMo5ZYjNl8ZU+fB3zPPdQhCqTNtFEn0MFibi8GEu291dd4Cn6bgVXdrPglXuXrseZshH7XcTdlxKMHlwpLauwwfbW9pVi4MuHHM50l4yaBFw69lhKEHbgpIMl4y6O65MEftwP0ho536Z+PAqP/YRqULZbheWGkOoAvUEo+qomDCjuGQMyfdtMbqlcoU3Fm3KU3HYzK1FxQaKjBQ0xV4JKh68xUa+mFIEEdlVK9djf+Tbpd4GlMyEHE7/k96FA83qkoCSoJ5MFfhwWWemoBlEVzRzJLgLlc0VD/0ZQdxQBHECozKVlSl0a6vUqTRgEXYCspC8sdfVjbDtY9EilJmjuYtVnbzHHjF1dObiWEzHVWOFAOeH3TS29nh0fD76oliwHaR5gJ2ki6OZ8w0E17aFmmJDOgX8VZ/mD1Ul3o2d7pVyE72+WnI3LJlmBTVzrUwhWfN+fkW5FqXae0M0iHiNz/OtR4xyAP1us7p2Rkdcj1y3eMj+dariXP7m6/d0NIond6tVLMisfPHfBsef9P7HXy74Uvh+bjfCRnTbb8Z/HUI4OdPsilMAz7aHZJNn8Nno3sGGzVXCLfWGN/19z44uDdaN7kmgwFztGpC2I749IstABsP/L711/3b/zzwA8COQ9sC5runjinjgPg3mpGO+6MenNn71b6Z94M/9U2IyJKR1EYUR4IBby1zq5oMnFaNRPT9+t1/TP7y2/49mxsitOLqIn7weCqDAVuOxKcBlRcJt1joiRqa8ELb4jIh8sywxYb0/L8t8XTmGqDyvkLDSwsp6XQ94qGyIjkOPbAcZ0FbH0kWrKf2J24nV3QJngyST5POI4H72pvdPWXAE+tCno2XDMQ7uTJBwH3BWB781FwZ7RRRgoVeNXWWdYkI33m3K5frPS2XA/Fs1EXHrVSChxs2ZU5OAsbDJc+2udAD92IK8hLEt9JTrVEw4YGb/fx4eSt5r7crGGlWKc2Px0yQa1o0lDO3cveKCh5s3dILbqvzIgrkqdP3b0eYcMe4X0QBuqA1Lrq4pYx7fVYIj90eTlzaEsMPsAydYgz+4HmzUazAe+rDJLmeL5htyQqcUB+oSQTMP7+F4BwkKGjM720nKOYA9kM7gTtY0AEcslME/wBy9oMTGoJCiwAAAABJRU5ErkJggg==";

            if (image != null) //Use fallback image if we were not able to retrieve one
            {
                base64Image = "data:image/png;base64," + Convert.ToBase64String(image);
            }
            if (user.Image != null) //In case you want to force an image with the DTO
            {
                base64Image = user.Image;
            }
            var entity = new User
            {
                Oid = user.Oid,
                Name = user.Name,
                Email = user.Email,
                Image = base64Image
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new UserDetailsDto
            {
                Oid = entity.Oid,
                Name = entity.Name,
                Email = entity.Email,
                Image = entity.Image
            });
        }

        public async Task<Option<UserDetailsDto>> ReadAsync(string userOid) =>
            await _context.Users.Where(u => u.Oid == userOid)
                                .Select(u => new UserDetailsDto
                                {
                                    Oid = u.Oid,
                                    Name = u.Name,
                                    Email = u.Email,
                                    Image = u.Image
                                })
                                .FirstOrDefaultAsync();

        public async Task<IReadOnlyCollection<UserDetailsDto>> ReadAsync() =>
            (await _context.Users
                           .Select(u => new UserDetailsDto
                           {
                               Oid = u.Oid,
                               Name = u.Name,
                               Email = u.Email,
                               Image = u.Image
                           })
                           .ToListAsync())
                           .AsReadOnly();

        public async Task<Status> DeleteAsync(string userOid)
        {
            var entity = await _context.Users.FindAsync(userOid);

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
