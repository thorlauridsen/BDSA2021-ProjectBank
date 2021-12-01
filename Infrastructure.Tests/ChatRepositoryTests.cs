using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;

namespace Infrastructure.Tests
{
    public class ChatRepositoryTests : IDisposable
    {
        private readonly ProjectBankContext _context;
        private readonly ChatRepository _repository;

        public ChatRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();

            User per = new User { Id = 1, Name = "per" };
            User bo = new User { Id = 2, Name = "bo" };
            User alice = new User { Id = 3, Name = "alice" };

            ChatUser chatPer = new ChatUser { Id = 1, User = per };
            ChatUser chatBo = new ChatUser { Id = 2, User = bo };
            ChatUser chatAlice = new ChatUser { Id = 3, User = alice };

            Chat chad = new Chat { Id = 1, ChatUsers = new HashSet<ChatUser>() { chatPer, chatBo } };
            Chat epicChad = new Chat { Id = 2, ChatUsers = new HashSet<ChatUser>() { chatPer, chatAlice } };

            ChatMessage fromPerToBo = new ChatMessage { Id = 1, Chat = chad, Content = "to Bo", FromUser = per };
            ChatMessage fromAliceToPer = new ChatMessage { Id = 2, Chat = epicChad, Content = "to Per", FromUser = alice };

            context.Chats.AddRange(chad, epicChad);
            context.ChatUsers.AddRange(chatPer, chatBo, chatAlice);
            context.ChatMessages.AddRange(fromPerToBo, fromAliceToPer);
            context.SaveChanges();

            _context = context;
            _repository = new ChatRepository(_context);
        }

        [Fact]
        public async void ReadAllAsync_given_user_id()
        {
            var actual = await _repository.ReadAllChatsAsync(1);

            var expected1 = new ChatDetailsDto { ChatId = 1, TargetUserId = 2, LatestMessageUserId = 1, LatestMessage = "to Bo", SeenLatestMessage = false };
            var expected2 = new ChatDetailsDto { ChatId = 2, TargetUserId = 1, LatestMessageUserId = 1, LatestMessage = "to Per", SeenLatestMessage = false };

            var actual1 = actual.ElementAt(0);
            var actual2 = actual.ElementAt(1);

            Assert.Equal(expected1.ChatId, actual1.ChatId);
            Assert.Equal(expected1.TargetUserId, actual1.TargetUserId);
            Assert.Equal(expected1.LatestMessageUserId, actual1.LatestMessageUserId);
            Assert.Equal(expected1.LatestMessage, actual1.LatestMessage);
            Assert.False(actual1.SeenLatestMessage);

            Assert.Equal(expected2.ChatId, actual2.ChatId);
            Assert.Equal(expected2.TargetUserId, actual2.TargetUserId);
            Assert.Equal(expected2.LatestMessageUserId, actual2.LatestMessageUserId);
            Assert.Equal(expected2.LatestMessage, actual2.LatestMessage);
            Assert.False(actual2.SeenLatestMessage);

            // Assert.Collection(actual,
            //     c =>
            //     {
            //         Assert.Equal(expected1.ChatId, c.ChatId);
            //         Assert.Equal(expected1.TargetUserId, c.TargetUserId);
            //         Assert.Equal(expected1.LatestMessageUserId, c.LatestMessageUserId);
            //         Assert.Equal(expected1.LatestMessage, c.LatestMessage);
            //         Assert.False(c.SeenLatestMessage);
            //     },
            //     c =>
            //     {
            //         Assert.Equal(expected2.ChatId, c.ChatId);
            //         Assert.Equal(expected2.TargetUserId, c.TargetUserId);
            //         Assert.Equal(expected2.LatestMessageUserId, c.LatestMessageUserId);
            //         Assert.Equal(expected2.LatestMessage, c.LatestMessage);
            //         Assert.False(c.SeenLatestMessage);
            //     }
            // );
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
