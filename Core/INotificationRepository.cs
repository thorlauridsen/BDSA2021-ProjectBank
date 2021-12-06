namespace ProjectBank.Core
{
    public interface INotificationRepository
    {
        Task<(Status, NotificationDetailsDto?)> CreateAsync(NotificationCreateDto notification);
        Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationsAsync(string UserOid);
        Task<Status> SeenNotificationAsync(int notificationId);
    }
}
