namespace ProjectBank.Core
{
    public interface INotificationRepository
    {
        Task<Status> CreateAsync(NotificationCreateDto notification);
        Task<IReadOnlyCollection<NotificationDetailsDto>> GetNotificationsAsync(int userId);
        Task<Status> SeenNotificationAsync(int notificationId);
    }
}