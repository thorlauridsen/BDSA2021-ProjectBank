namespace ProjectBank.Infrastructure
{
    public interface IProjectBankContext : IDisposable
    {
        DbSet<Supervisor> Supervisors { get; }
        DbSet<Student> Students { get; }
        DbSet<Post> Posts { get; }
        DbSet<Tag> Tags { get; }
        DbSet<Comment> Comments { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
