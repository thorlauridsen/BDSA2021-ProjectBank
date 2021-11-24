using Microsoft.EntityFrameworkCore;
using ProjectBank.Infrastructure;

namespace ProjectBank.Server.Model;

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

        if (!context.Students.Any())
        {
            context.Students.Add(new Student("Tue", "Dropout"));
            context.SaveChanges();
        }
        if (!context.Supervisors.Any())
        {
            context.Supervisors.Add(new Supervisor("Paolo", 30));
            context.SaveChanges();
        }
        if (!context.Posts.Any())
        {
            context.Posts.Add(new Post("Biology Project", "My Cool Biology Project"));
            context.SaveChanges();
        }
    }
}
