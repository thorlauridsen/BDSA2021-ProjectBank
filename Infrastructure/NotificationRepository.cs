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
            var entity = new Notification
            {
                Title = notification.Title,
                Content = notification.Content,
                Timestamp = notification.Timestamp,
                UserId = notification.UserId,
                Link = notification.Link,
                Seen = false
            };
            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return Created;
        }

        public async Task<IReadOnlyCollection<NotificationReadDto>> GetNotificationsAsync(int userId) =>
            (await _context.Notifications.Where(n => n.UserId == userId)
                                         .Select(n => new NotificationReadDto
                                         {
                                             Title = n.Title,
                                             Content = n.Content,
                                             Timestamp = n.Timestamp,
                                             Link = n.Link,
                                             Seen = n.Seen

                                         }).ToListAsync()).AsReadOnly();

        public async Task<Status> ReadNotificationAsync(int notificationId)
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
    }
}
