using ProjectBank.Core;

namespace ProjectBank.Infrastructure
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IProjectBankContext _context;

        public NotificationRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<Status> CreateAsync(NotificationCreateDto notification)
        {
            if (notification.Title.Trim().Equals("")
             || notification.Content.Trim().Equals(""))
            {
                return BadRequest;
            }

            var entity = new Notification
            {
                Title = notification.Title,
                Content = notification.Content,
                Timestamp = DateTime.Now,
                User = await GetUserAsync(notification.UserId),
                Link = notification.Link,
                Seen = false
            };
            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return Created;
        }

        public async Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationsAsync(int userId) =>
            (await _context.Notifications.Where(n => n.User.Id == userId)
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

        private async Task<User> GetUserAsync(int userId) =>
            await _context.Users.FirstAsync(u => u.Id == userId);
    }
}
