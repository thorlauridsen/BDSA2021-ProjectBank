using Microsoft.EntityFrameworkCore;

namespace ProjectBank.Infrastructure
{
    public interface IProjectBankContext : IDisposable
    {
        DbSet<User> Users { get; }
        DbSet<Post> Posts { get; }
        DbSet<Chat> Chats { get; }
        DbSet<ChatUser> ChatUsers { get; }
        DbSet<ChatMessage> ChatMessages { get; }
        DbSet<Notification> Notifications { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
