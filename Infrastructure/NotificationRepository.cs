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
            if (notification.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }
            var entity = new Notification
            {
                Title = notification.Title,
                Content = notification.Content,
                Timestamp = DateTime.Now,
                User = await GetUserAsync(notification.UserOid),
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
                UserOid = entity.User.oid,
                Timestamp = entity.Timestamp,
                Link = entity.Link,
                Seen = entity.Seen
            });
        }

        public async Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationsAsync(string userId) =>
            (await _context.Notifications.Where(n => n.User.oid == userId)
                                         .Select(n => new NotificationDetailsDto
                                         {
                                             Id = n.Id,
                                             Title = n.Title,
                                             Content = n.Content,
                                             Timestamp = n.Timestamp,
                                             Link = n.Link,
                                             Seen = n.Seen

                                         }).ToListAsync()).AsReadOnly();

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

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);
    }
}
