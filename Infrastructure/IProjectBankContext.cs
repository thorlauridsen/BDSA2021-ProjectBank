namespace ProjectBank.Infrastructure
{
    public interface IProjectBankContext : IDisposable
    {
        DbSet<Supervisor> Supervisors { get; }
        DbSet<Student> Students { get; }
        DbSet<Post> Posts { get; }
        DbSet<Tag> Tags { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
