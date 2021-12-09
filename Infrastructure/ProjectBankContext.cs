using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectBank.Core;

namespace ProjectBank.Infrastructure
{
    public class ProjectBankContext : DbContext, IProjectBankContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<ChatUser> ChatUsers => Set<ChatUser>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Notification> Notifications => Set<Notification>();

        public ProjectBankContext(DbContextOptions<ProjectBankContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                        .Property(p => p.Tags)
                        .HasConversion(
                            v => string.Join(',', v),
                            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),

                            new ValueComparer<string[]>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())))
                            );

            modelBuilder
                .Entity<Post>()
                .Property(p => p.PostState)
                .HasMaxLength(50)
                .HasConversion(new EnumToStringConverter<PostState>());
        }
    }
}
