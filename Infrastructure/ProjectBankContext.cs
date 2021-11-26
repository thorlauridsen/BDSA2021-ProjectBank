namespace ProjectBank.Infrastructure
{
    public class ProjectBankContext : DbContext, IProjectBankContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Supervisor> Supervisors => Set<Supervisor>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Comment> Comments => Set<Comment>();

        public ProjectBankContext(DbContextOptions<ProjectBankContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
