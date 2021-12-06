namespace ProjectBank.Infrastructure
{
    public class ProjectBankContext : DbContext, IProjectBankContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<ChatUser> ChatUsers => Set<ChatUser>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Notification> Notifications => Set<Notification>();

        public ProjectBankContext(DbContextOptions<ProjectBankContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}