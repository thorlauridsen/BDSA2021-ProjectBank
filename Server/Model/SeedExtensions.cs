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

            var student = new Student("Tue", "Chemistry");
            var supervisor = new User { Name = "Paolo" };

            if (!context.Students.Any())
            {
                context.Students.Add(student);
            }
            if (!context.Users.Any())
            {
                context.Users.Add(supervisor);
            }

            var post = new Post
            {
                Title = "Biology Project",
                Content = "My Cool Biology Project",
                DateAdded = DateTime.Now,
                SupervisorId = 1,
                Tags = new HashSet<Tag>() { new Tag("Biology") }
            };

            if (!context.Posts.Any())
            {
                context.Posts.Add(post);
            }
            var comment = new Comment(
                "Nice post",
                1,
                DateTime.Now,
                1
            );
            if (!context.Comments.Any())
            {
                context.Comments.Add(comment);
            }

            context.SaveChanges();
        }
    }
}
