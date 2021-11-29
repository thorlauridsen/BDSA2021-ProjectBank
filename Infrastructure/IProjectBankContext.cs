namespace ProjectBank.Infrastructure
{
    public interface IProjectBankContext : IDisposable
    {
        DbSet<User> Users { get; }
        DbSet<Supervisor> Supervisors { get; }
        DbSet<Student> Students { get; }
        DbSet<Post> Posts { get; }
        DbSet<Tag> Tags { get; }
        DbSet<Comment> Comments { get; }
        DbSet<Chat> Chats { get; }
        DbSet<ChatUser> ChatUsers { get; }
        DbSet<ChatMessage> ChatMessages { get; }
        DbSet<Notification> Notifications { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
