using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectBank.Core;
using static ProjectBank.Core.Status;

namespace ProjectBank.Infrastructure
{
    public class ChatRepository : IChatRepository
    {
        private readonly IProjectBankContext _context;

        public ChatRepository(IProjectBankContext context)
        {
            _context = context;
        }

        public async Task<(Status, ChatDto?)> CreateNewChatAsync(ChatCreateDto chat)
        {
            HashSet<ChatUser> chatUsers = new HashSet<ChatUser>();
            foreach (var userId in chat.ChatUserIds)
            {
                var entityChatUser = new ChatUser
                {
                    User = await GetUserAsync(userId),
                    SeenLatestMessage = userId == chat.FromUserId ? true : false,
                };
                chatUsers.Add(entityChatUser);
                _context.ChatUsers.Add(entityChatUser);
            }

            var entityChat = new Chat
            {
                Post = await GetPostAsync(chat.ProjectId),
                ChatUsers = chatUsers,
            };
            _context.Chats.Add(entityChat);
            await _context.SaveChangesAsync();

            return (Created, new ChatDto
            {
                ChatId = entityChat.Id,
                ProjectId = entityChat.Post.Id,
                ChatUserIds = entityChat.ChatUsers.Select(cu => cu.Id).ToHashSet()
            });
        }

        public async Task<(Status, ChatMessageDetailsDto?)> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage)
        {
            if (chatMessage.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }

            var entityChatMessage = new ChatMessage
            {
                Chat = await GetChatAsync(chatMessage.ChatId),
                Content = chatMessage.Content,
                Timestamp = DateTime.Now,
                FromUser = await GetUserAsync(chatMessage.FromUserId)
            };

            entityChatMessage.Chat.ChatUsers.Where(cu => cu.User.oid != chatMessage.FromUserId)
                .Select(cu => cu.SeenLatestMessage = false);

            // foreach (var chatUser in entityChatMessage.Chat.ChatUsers)
            // {
            //     if (chatUser.Id != chatMessage.FromUserId)
            //     {
            //         chatUser.SeenLatestMessage = false;
            //     }
            // }

            _context.ChatMessages.Add(entityChatMessage);
            await _context.SaveChangesAsync();

            return (Status.Created, new ChatMessageDetailsDto()
            {
                Content = entityChatMessage.Content,
                FromUser = new UserDto(entityChatMessage.FromUser.oid, entityChatMessage.FromUser.Name),
                Timestamp = entityChatMessage.Timestamp,
                chatId = entityChatMessage.Chat.Id,
                chatMessageId = entityChatMessage.Id
            });
        }

        public async Task<ChatMessageDto?> ReadSpecificMessageAsync(int chatMessageId)
        {
            var chatMessage = await _context.ChatMessages.Include("Chat")
                .FirstOrDefaultAsync(cm => cm.Id == chatMessageId);
            if (chatMessage == null) return null;
            return new ChatMessageDto()
            {
                Content = chatMessage.Content,
                FromUser = new UserDto(chatMessage.FromUser.oid, chatMessage.FromUser.Name),
                Timestamp = chatMessage.Timestamp,
            };
        }

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userId)
        {
            return (await _context.Chats
                .Join(_context.ChatMessages, c => c.Id, cm => cm.Chat.Id, (c, cm) => new {c, cm})
                .Where(@t => @t.c.ChatUsers.Any(u => u.User.oid == userId))
                .OrderByDescending(@t => @t.cm.Timestamp)
                .Select(@t => new ChatDetailsDto
                {
                    ChatId = @t.c.Id,
                    TargetUserId = @t.c.ChatUsers.First(ch => ch.User.oid != userId).User.oid,
                    LatestChatMessage = new ChatMessageDto()
                    {
                        Content = @t.cm.Content,
                        FromUser = new UserDto(@t.cm.FromUser.oid, @t.cm.FromUser.Name),
                        Timestamp = @t.cm.Timestamp
                    },
                    SeenLatestMessage = @t.c.ChatUsers.First().SeenLatestMessage,
                    ProjectId = @t.c.Post.Id
                }).ToListAsync().WaitAsync(TimeSpan.FromMinutes(10))).DistinctBy(dto => dto.ChatId).ToList();
        }

        public async Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId) =>
            (await _context.ChatMessages.Where(c => c.Chat.Id == chatId)
                .Select(c => new ChatMessageDto
                {
                    FromUser = new UserDto(c.FromUser.oid, c.FromUser.Name),
                    Content = c.Content,
                    Timestamp = c.Timestamp
                })
                .ToListAsync())
            .AsReadOnly();

        public async Task<ChatDetailsDto?> ReadChatAsync(int chatId, string userId)
        {
            var result = await _context.Chats.Include("ChatUsers").Include("ChatUsers.User").Include("Post") //Få alle chats
                .Join(_context.ChatMessages.Include("FromUser"), //Join det med ChatMessages så vi får en tuple
                    chat => chat.Id,
                    chatMessage => chatMessage.Chat.Id,
                    (c, cm) => new {chat = c, chatMessage = cm})
                .FirstOrDefaultAsync(t => t.chat.Id == chatId);//Tag den første tuple
            if (result?.chat.Post == null) return null;
            var latestChatMessage = new ChatMessageDto()
            {
                Content = result.chatMessage.Content,
                FromUser = new UserDto(result.chatMessage.FromUser.oid, result.chatMessage.FromUser.Name),
                Timestamp = result.chatMessage.Timestamp
            };
            var targetUser = result.chat.ChatUsers.First(ch => ch.User.oid != userId);
            var targetUserId = targetUser.User.oid;
            var seenLatestMessage = result.chat.ChatUsers.FirstOrDefault(chatUser => chatUser.User.oid != userId)
                .SeenLatestMessage;
            return new ChatDetailsDto()
            {
                ChatId = result.chat.Id,
                LatestChatMessage = latestChatMessage,
                TargetUserId = targetUserId,
                SeenLatestMessage = seenLatestMessage,
                ProjectId = result.chat.Post.Id
            };

        }

        private async Task<Chat> GetChatAsync(int chatId) =>
            await _context.Chats.Include("ChatUsers").Include("Post").FirstAsync(c => c.Id == chatId);

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);

        private async Task<Post> GetPostAsync(int postId) =>
            await _context.Posts.FirstAsync(p => p.Id == postId);
    }
}