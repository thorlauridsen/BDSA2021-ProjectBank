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
        User karl;

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


            per = new User { Oid = "1", Name = "per", Email = "per@outlook.com" };
            bo = new User { Oid = "2", Name = "bo", Email = "bo@outlook.com" };
            alice = new User { Oid = "3", Name = "alice", Email = "alice@outlook.com" };
            karl = new User { Oid = "4", Name = "karl", Email = "karl@outlook.com" };

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
            chatAlice = new ChatUser
            {
                Id = 4,
                User = alice,
                SeenLatestMessage = false
            };

            chatEntity = new Chat
            {
                Id = 1,
                ChatUsers = new HashSet<ChatUser>() { chatPer1, chatBo },
                Post = post
            };
            epicChatEntity = new Chat
            {
                Id = 2,
                ChatUsers = new HashSet<ChatUser>() { chatPer2, chatAlice },
                Post = post
            };

            fromPerToBo = new ChatMessage
            {
                Id = 1,
                Chat = chatEntity,
                Content = "to Bo",
                FromUser = per,
                Timestamp = new DateTime(2021, 12, 7)
            };
            fromAliceToPer = new ChatMessage
            {
                Id = 2,
                Chat = epicChatEntity,
                Content = "to Per",
                FromUser = alice,
                Timestamp = new DateTime(2021, 12, 8)
            };

            context.Posts.Add(post);
            context.Users.Add(karl);
            context.Chats.AddRange(chatEntity, epicChatEntity);
            context.ChatUsers.AddRange(chatPer1, chatPer2, chatBo, chatAlice);
            context.ChatMessages.AddRange(fromPerToBo, fromAliceToPer);
            context.SaveChanges();

            _context = context;
            _repository = new ChatRepository(_context);
        }

        [Fact]
        public async Task SetSeen_sets_returns_success()
        {
            var actual = await _repository.SetSeen(epicChatEntity.Id, chatAlice.User.Oid);
            Assert.Equal(Status.Success, actual);
        }

        [Theory]
        [InlineData(1, "8943894")]
        [InlineData(1000, "3")]
        public async Task SetSeen_sets_returns_not_found(int chatId, string oid)
        {
            var actual = await _repository.SetSeen(chatId, oid);
            Assert.Equal(Status.NotFound, actual);
        }

        [Fact]
        public async Task CreateNewChatAsync_creates_new_chat()
        {
            var chat = new ChatCreateDto()
            {
                ProjectId = 1,
                FromUserOid = "4",
                ChatUserOids = new HashSet<string>() { karl.Oid, bo.Oid }
            };
            var (status, response) = await _repository.CreateNewChatAsync(chat);

            var expected = new ChatDto()
            {
                ProjectId = 1,
                ChatId = 3,
                ChatUserOids = new HashSet<int>() { 5, 6 }
            };

            Assert.Equal(Created, status);
            Assert.NotNull(response);
            Assert.Equal(expected.ProjectId, response?.ProjectId);
            Assert.Equal(expected.ChatId, response?.ChatId);
            Assert.Equal(expected.ChatUserOids, response?.ChatUserOids);
        }

        [Fact]
        public async Task ReadAllAsync_given_user_id()
        {
            var actual = await _repository.ReadAllChatsAsync("1");

            var expected1 = new ChatDetailsDto
            {
                ChatId = 2,
                TargetUserOid = "3",
                LatestChatMessage = new ChatMessageDto()
                {
                    Content = "to Per",
                    FromUser = new UserDetailsDto
                    {
                        Oid = "3",
                        Name = "Alice",
                        Email = "alice@outlook.com"
                    },
                    Timestamp = DateTime.Now
                },
                SeenLatestMessage = false
            };
            var expected2 = new ChatDetailsDto
            {
                ChatId = 1,
                TargetUserOid = "2",
                LatestChatMessage = new ChatMessageDto()
                {
                    Content = "to Bo",
                    FromUser = new UserDetailsDto
                    {
                        Oid = "1",
                        Name = "Per",
                        Email = "per@outlook.com"
                    },
                    Timestamp = DateTime.Now
                },
                SeenLatestMessage = false
            };

            var actual1 = actual.ElementAt(0);
            var actual2 = actual.ElementAt(1);

            Assert.Equal(expected1.ChatId, actual1.ChatId);
            Assert.Equal(expected1.TargetUserOid, actual1.TargetUserOid);
            Assert.Equal(expected1.LatestChatMessage.FromUser.Oid, actual1.LatestChatMessage.FromUser.Oid);
            Assert.Equal(expected1.LatestChatMessage.Content, actual1.LatestChatMessage.Content);
            Assert.False(actual1.SeenLatestMessage);

            Assert.Equal(expected2.ChatId, actual2.ChatId);
            Assert.Equal(expected2.TargetUserOid, actual2.TargetUserOid);
            Assert.Equal(expected2.LatestChatMessage.FromUser.Oid, actual2.LatestChatMessage.FromUser.Oid);
            Assert.Equal(expected2.LatestChatMessage.Content, actual2.LatestChatMessage.Content);
            Assert.False(actual2.SeenLatestMessage);
        }

        [Fact]
        public async Task CreateNewChatMessage_given_Content_returns_Created()
        {
            var chatMessage = new ChatMessageCreateDto
            {
                FromUserOid = "1",
                ChatId = 1,
                Content = "Hello"
            };
            var (status, response) = await _repository.CreateNewChatMessageAsync(chatMessage);

            Assert.Equal(Created, status);
            Assert.NotNull(response);
            Assert.Equal(chatMessage.FromUserOid, response?.FromUser.Oid);
            Assert.Equal(chatMessage.Content, response?.Content);
            Assert.Equal(chatMessage.ChatId, response?.chatId);
        }

        [Fact]
        public async Task CreateNewChatMessage_given_no_Content_returns_BadRequest()
        {
            var chatMessage = new ChatMessageCreateDto
            {
                FromUserOid = "1",
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
                FromUser = new UserDetailsDto
                {
                    Oid = fromPerToBo.FromUser.Oid,
                    Name = fromPerToBo.FromUser.Name,
                    Email = fromPerToBo.FromUser.Email
                },
                Timestamp = fromPerToBo.Timestamp
            };
            var expected = new ChatDetailsDto()
            {
                ChatId = 1,
                ProjectId = 1,
                LatestChatMessage = expectedLatestChatMessage,
                SeenLatestMessage = false,
                TargetUserOid = "2"
            };
            Assert.NotNull(actual);
            Assert.Equal(expected.SeenLatestMessage, actual?.SeenLatestMessage);
            Assert.Equal(expected.TargetUserOid, actual?.TargetUserOid);
            Assert.Equal(expectedLatestChatMessage.Content, actual?.LatestChatMessage.Content);
            Assert.Equal(expectedLatestChatMessage.FromUser.Oid, actual?.LatestChatMessage.FromUser.Oid);
            Assert.Equal(expectedLatestChatMessage.Timestamp, actual?.LatestChatMessage.Timestamp);
            Assert.Equal(expected.ChatId, actual?.ChatId);
            Assert.Equal(expected.ProjectId, actual?.ProjectId);

        }

        [Fact]
        public async Task ReadChatAsync_given_unknown_id_returns_null()
        {
            var actual = await _repository.ReadChatAsync(4124, "1");
            ChatDetailsDto? expected = null;
            Assert.Equal(expected, actual);
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
