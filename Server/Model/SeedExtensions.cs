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
            var supervisor = new Supervisor { Name = "Paolo" };
            var post = new Post
            (
                "Biology Project",
                "My Cool Biology Project",
                supervisor,
                new HashSet<Tag>() { new Tag("Biology") }
            );

            if (!context.Students.Any())
            {
                context.Students.Add(student);
            }
            if (!context.Supervisors.Any())
            {
                context.Supervisors.Add(supervisor);
            }
            if (!context.Posts.Any())
            {
                context.Posts.Add(post);
            }
            context.SaveChanges();
        }
    }
}
