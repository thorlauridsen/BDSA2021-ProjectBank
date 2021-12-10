using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using static ProjectBank.Core.Status;

namespace ProjectBank.Infrastructure
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IProjectBankContext _context;

        public NotificationRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<(Status, NotificationDetailsDto?)> CreateAsync(NotificationCreateDto notification)
        {
            var user = await GetUserAsync(notification.UserOid);

            if (user == null ||
                notification.Title.Trim().Equals("") ||
                notification.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }

            var entity = new Notification
            {
                Title = notification.Title,
                Content = notification.Content,
                Timestamp = DateTime.Now,
                User = user,
                Link = notification.Link,
                Seen = false
            };
            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return (Created, new NotificationDetailsDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                UserOid = entity.User.Oid,
                Timestamp = entity.Timestamp,
                Link = entity.Link,
                Seen = entity.Seen
            });
        }

        public async Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationsAsync(string userOid)
        {
            var query =  _context.Notifications.Where(n => n.User.Oid == userOid);
            var response = (await query
                .OrderByDescending(n => n.Timestamp)
                .Select(n => new NotificationDetailsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Timestamp = n.Timestamp,
                    Link = n.Link,
                    Seen = n.Seen
                }).ToListAsync().ConfigureAwait(false)).AsReadOnly();
            (await query.Where(notification => !notification.Seen).ToListAsync())
                .ForEach(notification =>
                {
                    notification.Seen = true;
                    _context.Notifications.Update(notification);
                });
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<Status> SeenNotificationAsync(int notificationId)
        {
            var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (entity == null)
            {
                return NotFound;
            }
            entity.Seen = true;
            await _context.SaveChangesAsync();

            return Updated;
        }

        private async Task<User?> GetUserAsync(string userOid) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Oid == userOid);
    }
}
