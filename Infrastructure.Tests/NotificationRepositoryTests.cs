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

            var user = new User { Oid = "1", Name = "Claus", Email = "claus@outlook.com" };
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
        public async Task CreateAsync_creates_notification_successfully()
        {
            var notification = new NotificationCreateDto
            {
                Title = "Important",
                Content = "We gotta go now!",
                UserOid = "1",
                Link = "https://google.com"
            };
            var (status, content) = await _repository.CreateAsync(notification);

            Assert.Equal(Created, status);
            Assert.NotNull(content);
            Assert.Equal(2, content?.Id);
            Assert.Equal("Important", content?.Title);
            Assert.Equal("We gotta go now!", content?.Content);
            Assert.Equal("1", content?.UserOid);
            Assert.Equal("https://google.com", content?.Link);
            Assert.Equal(false, content?.Seen);
        }

        [Fact]
        public async Task CreateAsync_without_content_returns_BadRequest()
        {
            var notification = new NotificationCreateDto
            {
                Title = "",
                Content = "",
                UserOid = "1",
                Link = "https://google.com"
            };
            var (status, content) = await _repository.CreateAsync(notification);

            Assert.Equal(BadRequest, status);
            Assert.Null(content);
        }

        [Fact]
        public async Task CreateAsync_non_existing_user_returns_BadRequest()
        {
            var notification = new NotificationCreateDto
            {
                Title = "Important",
                Content = "We gotta go now!",
                UserOid = "1111",
                Link = "https://google.com"
            };
            var (status, content) = await _repository.CreateAsync(notification);

            Assert.Equal(BadRequest, status);
            Assert.Null(content);
        }

        [Fact]
        public async Task GetNotificationsAsync_with_existing_oid_returns_notifications()
        {
            var notifications = await _repository.GetNotificationsAsync("1");

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
        public async Task GetNotificationsAsync_without_existing_oid_returns_empty_list()
        {
            var notifications = await _repository.GetNotificationsAsync("111");
            Assert.Empty(notifications);
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
