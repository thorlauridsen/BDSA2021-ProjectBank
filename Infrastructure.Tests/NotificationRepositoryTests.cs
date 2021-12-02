using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

namespace Infrastructure.Tests
{
    public class NotificationRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly NotificationRepository _repository;
        private DateTime today = DateTime.Now;

        public NotificationRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            var user = new User { Id = 1, Name = "Claus" };
            context.Users.Add(user);

            var notification = new Notification
            {
                Id = 1,
                Title = "Important Notification!",
                Content = "Hello remember to follow",
                Timestamp = today,
                User = user,
                Link = "https://google.com",
                Seen = false
            };
            context.Notifications.Add(notification);
            context.SaveChanges();

            _context = context;
            _repository = new NotificationRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_correctly_created()
        {
            var notification = new NotificationCreateDto
            {
                Title = "Important",
                Content = "We gotta go now!",
                UserId = 1,
                Link = "https://google.com"
            };
            var created = await _repository.CreateAsync(notification);
            Assert.Equal(2, created.Id);
            Assert.Equal("Important", created.Title);
            Assert.Equal("We gotta go now!", created.Content);
            Assert.Equal(1, created.UserId);
            Assert.Equal("https://google.com", created.Link);
            Assert.Equal(false, created.Seen);
        }

        [Fact]
        public async Task GetNotificationsAsync()
        {
            var notifications = await _repository.GetNotificationsAsync(1);

            var notification = new NotificationDetailsDto
            {
                Id = 1,
                Title = "Important Notification!",
                Content = "Hello remember to follow",
                Timestamp = today,
                Link = "https://google.com"
            };

            Assert.Equal(1, notifications.Count);
            Assert.Equal(notification, notifications.First());
        }

        [Fact]
        public async void SeenNotifactionsAsync_given_existing_returns_Updated()
        {
            var response = await _repository.SeenNotificationAsync(1);
            Assert.Equal(Updated, response);
        }

        [Fact]
        public async void SeenNotifactionsAsync_given_non_existing_returns_NotFound()
        {
            var response = await _repository.SeenNotificationAsync(11);
            Assert.Equal(NotFound, response);
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. 
            // Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
