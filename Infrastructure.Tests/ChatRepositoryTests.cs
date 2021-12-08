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

        User per;
        User bo;
        User alice;

        Post post;
            
        ChatUser chatPer1;
        ChatUser chatPer2;
        ChatUser chatBo;
        ChatUser chatAlice;

        Chat chatEntity;
        Chat epicChatEntity;

        ChatMessage fromPerToBo;
        ChatMessage fromAliceToPer;
        
        public ChatRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<ProjectBankContext>();
            builder.UseSqlite(connection);
            var context = new ProjectBankContext(builder.Options);
            context.Database.EnsureCreated();


            per = new User { oid = "1", Name = "per" };
            bo = new User { oid = "2", Name = "bo" };
            alice = new User { oid = "3", Name = "alice" };

            post = new Post()
            {
                Comments = new List<Comment>(), 
                Content = "This is a test post", 
                Id = 1, 
                Title = "Test post", 
                User = per,
                DateAdded = new DateTime()
            };
            
            chatPer1 = new ChatUser { Id = 1, User = per };
            chatPer2 = new ChatUser { Id = 2, User = per };
            chatBo = new ChatUser { Id = 3, User = bo };
            chatAlice = new ChatUser { Id = 4, User = alice };

            chatEntity = new Chat { Id = 1, ChatUsers = new HashSet<ChatUser>() { chatPer1, chatBo }, Post = post};
            epicChatEntity = new Chat { Id = 2, ChatUsers = new HashSet<ChatUser>() { chatPer2, chatAlice }, Post = post };

            fromPerToBo = new ChatMessage { Id = 1, Chat = chatEntity, Content = "to Bo", FromUser = per, Timestamp = new DateTime(2021, 12,7)};
            fromAliceToPer = new ChatMessage { Id = 2, Chat = epicChatEntity, Content = "to Per", FromUser = alice , Timestamp = new DateTime(2021, 12,8)};

            context.Posts.Add(post);
            context.Chats.AddRange(chatEntity, epicChatEntity);
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

            var expected1 = new ChatDetailsDto { ChatId = 1, TargetUserId = "2", LatestChatMessage = new ChatMessageDto(){Content = "to Bo", FromUser = new UserDto("1", "Per"), Timestamp = DateTime.Now}, SeenLatestMessage = false };
            var expected2 = new ChatDetailsDto { ChatId = 2, TargetUserId = "3", LatestChatMessage = new ChatMessageDto(){Content = "to Per", FromUser = new UserDto("3", "Alice"), Timestamp = DateTime.Now}, SeenLatestMessage = false };

            var actual1 = actual.ElementAt(0);
            var actual2 = actual.ElementAt(1);

            Assert.Equal(expected1.ChatId, actual1.ChatId);
            Assert.Equal(expected1.TargetUserId, actual1.TargetUserId);
            Assert.Equal(expected1.LatestChatMessage.FromUser.oid, actual1.LatestChatMessage.FromUser.oid);
            Assert.Equal(expected1.LatestChatMessage.Content, actual1.LatestChatMessage.Content);
            Assert.False(actual1.SeenLatestMessage);

            Assert.Equal(expected2.ChatId, actual2.ChatId);
            Assert.Equal(expected2.TargetUserId, actual2.TargetUserId);
            Assert.Equal(expected2.LatestChatMessage.FromUser.oid, actual2.LatestChatMessage.FromUser.oid);
            Assert.Equal(expected2.LatestChatMessage.Content, actual2.LatestChatMessage.Content);
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
            Assert.Equal(chatMessage.FromUserId, response.FromUser.oid);
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
            var actual = await _repository.ReadChatAsync(1, "1");

            var expectedLatestChatMessage = new ChatMessageDto()
            {
                Content = fromPerToBo.Content,
                FromUser = new UserDto(fromPerToBo.FromUser.oid, fromPerToBo.FromUser.Name),
                Timestamp = fromPerToBo.Timestamp
            };
            var expected = new ChatDetailsDto()
            {
                ChatId = 1,
                ProjectId = 1, 
                LatestChatMessage = expectedLatestChatMessage,
                SeenLatestMessage = false,
                TargetUserId = "2"
            };
            Assert.NotNull(actual);
            Assert.Equal(expected.SeenLatestMessage, actual?.SeenLatestMessage);
            Assert.Equal(expected.TargetUserId, actual?.TargetUserId);
            Assert.Equal(expectedLatestChatMessage.Content, actual?.LatestChatMessage.Content);
            Assert.Equal(expectedLatestChatMessage.FromUser.oid, actual?.LatestChatMessage.FromUser.oid);
            Assert.Equal(expectedLatestChatMessage.Timestamp, actual?.LatestChatMessage.Timestamp);
            Assert.Equal(expected.ChatId, actual?.ChatId);
            Assert.Equal(expected.ProjectId, actual?.ProjectId);
            
        }
        
        [Fact]
        public async Task ReadChatAsync_given_unknown_id_returns_null()
        {
            var actual = await _repository.ReadChatAsync(4124, "1");
            ChatDetailsDto? expected = null;
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
