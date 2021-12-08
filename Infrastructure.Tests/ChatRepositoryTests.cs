using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectBank.Core;
using ProjectBank.Infrastructure;
using Xunit;
using static ProjectBank.Core.Status;

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

            
            
            var per = new User { oid = "1", Name = "per" };
            var bo = new User { oid = "2", Name = "bo" };
            var alice = new User { oid = "3", Name = "alice" };

            var post = new Post()
            {
                Comments = new List<Comment>(), 
                Content = "This is a test post", 
                Id = 1, 
                Title = "Test post", 
                User = per,
                DateAdded = new DateTime()
            };
            
            var chatPer1 = new ChatUser { Id = 1, User = per };
            var chatPer2 = new ChatUser { Id = 2, User = per };
            var chatBo = new ChatUser { Id = 3, User = bo };
            var chatAlice = new ChatUser { Id = 4, User = alice };

            var chad = new Chat { Id = 1, ChatUsers = new HashSet<ChatUser>() { chatPer1, chatBo }, Post = post};
            var epicChad = new Chat { Id = 2, ChatUsers = new HashSet<ChatUser>() { chatPer2, chatAlice } };

            var fromPerToBo = new ChatMessage { Id = 1, Chat = chad, Content = "to Bo", FromUser = per };
            var fromAliceToPer = new ChatMessage { Id = 2, Chat = epicChad, Content = "to Per", FromUser = alice };

            context.Posts.Add(post);
            context.Chats.AddRange(chad, epicChad);
            context.ChatUsers.AddRange(chatPer1, chatPer2, chatBo, chatAlice);
            context.ChatMessages.AddRange(fromPerToBo, fromAliceToPer);
            context.SaveChanges();

            _context = context;
            _repository = new ChatRepository(_context);
        }

        [Fact]
        public async Task ReadAllAsync_given_user_id()
        {
            var actual = await _repository.ReadAllChatsAsync("1");

            var expected1 = new ChatDetailsDto { ChatId = 1, TargetUserId = "2", LatestMessageUserId = "1", LatestMessage = "to Bo", SeenLatestMessage = false };
            var expected2 = new ChatDetailsDto { ChatId = 2, TargetUserId = "3", LatestMessageUserId = "3", LatestMessage = "to Per", SeenLatestMessage = false };

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
        }

        [Fact]
        public async Task CreateNewChatMessage_given_Content_returns_Created()
        {
            var chatMessage = new ChatMessageCreateDto
            {
                FromUserId = "1",
                ChatId = 1,
                Content = "Hello"
            };
            var (status, response) = await _repository.CreateNewChatMessageAsync(chatMessage);
            Assert.Equal(Created, status);
            Assert.Equal(chatMessage.FromUserId, response.FromUserId);
            Assert.Equal(chatMessage.Content, response.Content);
            Assert.Equal(chatMessage.ChatId, response.chatId);
        }

        [Fact]
        public async Task CreateNewChatMessage_given_no_Content_returns_BadRequest()
        {
            var chatMessage = new ChatMessageCreateDto
            {
                FromUserId = "1",
                ChatId = 1,
                Content = ""
            };
            var (status, response) = await _repository.CreateNewChatMessageAsync(chatMessage);
            Assert.Equal(BadRequest, status);
            Assert.Null(response);
        }

        [Fact]
        public async Task ReadChatAsync_given_id_returns_ChatDto()
        {
            var actual = await _repository.ReadChatAsync(1);
            var expected = new ChatDto() { ChatId = 1, ChatUserIds = new HashSet<int>(){1,3}, ProjectId = 1};
            Assert.NotNull(actual);
            Assert.Equal(expected.ChatUserIds, actual?.ChatUserIds);
            Assert.Equal(expected.ChatId, actual?.ChatId);
            Assert.Equal(expected.ProjectId, actual?.ProjectId);
            
        }
        
        [Fact]
        public async Task ReadChatAsync_given_unknown_id_returns_null()
        {
            var actual = await _repository.ReadChatAsync(4124);
            ChatDto? expected = null;
            Assert.Equal(expected,actual);
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
