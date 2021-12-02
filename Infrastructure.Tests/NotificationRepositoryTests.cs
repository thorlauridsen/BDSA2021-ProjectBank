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
                Content = "We gotta go! now!",
                UserId = 1,
                Link = "https://google.com"
            };
            var res = await _repository.CreateAsync(notification);
            Assert.Equal(Created, res);
        }

        [Fact]
        public async Task CreateAsync_given_empty_content_returns_BadRequest()
        {
            var notification = new NotificationCreateDto
            {
                Title = "Prank",
                Content = "",
                UserId = 1,
                Link = "https://youtube.com"
            };
            var res = await _repository.CreateAsync(notification);
            Assert.Equal(BadRequest, res);
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
