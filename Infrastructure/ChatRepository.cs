using System.Linq;
using ProjectBank.Core;

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
                FromUser = new UserDto(entityChatMessage.FromUser.oid,entityChatMessage.FromUser.Name),
                Timestamp = entityChatMessage.Timestamp,
                chatId = entityChatMessage.Chat.Id,
                chatMessageId = entityChatMessage.Id
            });
        }

        public Task<ChatDetailsDto?> ReadChatAsync(int chatId)
        {
            throw new NotImplementedException();
        }

        public async Task<ChatMessageDto?> ReadSpecificMessageAsync(int chatMessageId)
        {
            var chatMessage = await _context.ChatMessages.Include("Chat").FirstOrDefaultAsync(cm => cm.Id == chatMessageId);
            if (chatMessage == null) return null;
            return new ChatMessageDto()
            {
                Content = chatMessage.Content,
                FromUser = new UserDto(chatMessage.FromUser.oid,chatMessage.FromUser.Name),
                Timestamp = chatMessage.Timestamp,
            };
        }

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userId)
        {
            return await (from c in _context.Chats
                          join cm in _context.ChatMessages
                          on c.Id equals cm.Chat.Id
                          where c.ChatUsers.Any(u => u.User.oid == userId)
                          orderby cm.Timestamp ascending 
                          select new ChatDetailsDto
                          {
                              ChatId = c.Id,
                              TargetUserId = c.ChatUsers.First(ch => ch.User.oid != userId).User.oid,
                              LatestChatMessage = new ChatMessageDto()
                              {
                                  Content = cm.Content,
                                  FromUser = new UserDto(cm.FromUser.oid, cm.FromUser.Name),
                                  Timestamp = cm.Timestamp
                              },
                              SeenLatestMessage = c.ChatUsers.First().SeenLatestMessage,
                              ProjectId = c.Post.Id
                          }).ToListAsync();
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

            var c = await _context.Chats
                .Join(_context.ChatMessages, c => c.Id, cm => cm.Chat.Id, (c, cm) => new {c, cm})
                .FirstOrDefaultAsync(@t => @t.c.Id == chatId);
            if (c?.c.Post != null)
                return new ChatDetailsDto()
                {
                    ChatId = c.c.Id,
                    LatestChatMessage = new ChatMessageDto()
                    {
                        Content = c.cm.Content,
                        FromUser = new UserDto(c.cm.FromUser.oid, c.cm.FromUser.Name),
                        Timestamp = c.cm.Timestamp
                    },
                    TargetUserId = c.c.ChatUsers.First(ch => ch.User.oid != userId).User.oid,
                    SeenLatestMessage = c.c.ChatUsers.FirstOrDefault(chatUser => chatUser.User.oid != userId)
                        .SeenLatestMessage,
                    ProjectId = c.c.Post.Id
                };
            return null;
        }

        private async Task<Chat> GetChatAsync(int chatId) =>
            await _context.Chats.Include("ChatUsers").Include("Post").FirstAsync(c => c.Id == chatId);

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);

        private async Task<Post> GetPostAsync(int postId) =>
            await _context.Posts.FirstAsync(p => p.Id == postId);
    }
}
