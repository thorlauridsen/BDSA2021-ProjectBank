using Microsoft.EntityFrameworkCore;
using ProjectBank.Infrastructure;

namespace ProjectBank.Server.Model
{
    public static class SeedExtensions
    {
        public static IHost Seed(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProjectBankContext>();

                SeedCharacters(context);
            }
            return host;
        }

        private static void SeedCharacters(ProjectBankContext context)
        {
            context.Database.Migrate();

            var user1 = new User { Name = "Paolo" };
            var user2 = new User { Name = "Tue" };

            if (!context.Users.Any())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
            }

            var post = new Post
            {
                Title = "Biology Project",
                Content = "My Cool Biology Project",
                DateAdded = DateTime.Now,
                User = user1,
                Tags = new HashSet<Tag>() { new Tag("Biology") }
            };

            if (!context.Posts.Any())
            {
                context.Posts.Add(post);
            }

            var comment = new Comment
            {
                Content = "Nice post",
                UserId = user2.Id,
                DateAdded = DateTime.Now,
                PostId = post.Id
            };
            if (!context.Comments.Any())
            {
                context.Comments.Add(comment);
            }
            context.SaveChanges();
        }
    }
}
