using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBank.Infrastructure;

namespace ProjectBank.Server.Integration.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProjectBankContext>));

                if (dbContext != null)
                {
                    services.Remove(dbContext);
                }

                /* Overriding policies and adding Test Scheme defined in TestAuthHandler */
                services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("Test")
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                var connection = new SqliteConnection("Filename=:memory:");

                services.AddDbContext<ProjectBankContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();
                using var appContext = scope.ServiceProvider.GetRequiredService<ProjectBankContext>();
                appContext.Database.OpenConnection();
                appContext.Database.EnsureCreated();

                Seed(appContext);
            });

            builder.UseEnvironment("Integration");

            return base.CreateHost(builder);
        }

        private void Seed(ProjectBankContext context)
        {
            var user1 = new User { oid = "1", Name = "Paolo" };
            var user2 = new User { oid = "2", Name = "Tue" };

            if (!context.Users.Any())
            {
                context.Users.Add(user1);
                context.Users.Add(user2);
            }
            var comment = new Comment
            {
                Content = "Nice post",
                User = user2,
                DateAdded = DateTime.Now
            };
            /*if (!context.Comments.Any())
            {
                context.Comments.Add(comment);
            }*/

            var tag_biology = new Tag("Biology");
            var post = new Post
            {
                Title = "Biology Project",
                Content = "My Cool Biology Project",
                DateAdded = DateTime.Now,
                User = user1,
                Comments = new List<Comment>(){comment},
                Tags = new HashSet<Tag>() { tag_biology }
            };
            if (!context.Posts.Any())
            {
                context.Posts.Add(post);
            }

            

            var notification = new Notification
            {
                Title = "Important Notification!",
                Content = "Hello remember to follow",
                Timestamp = DateTime.Now,
                User = user1,
                Link = "https://google.com",
                Seen = false
            };
            if (!context.Notifications.Any())
            {
                context.Notifications.Add(notification);
            }



            context.SaveChanges();
        }
    }
}
