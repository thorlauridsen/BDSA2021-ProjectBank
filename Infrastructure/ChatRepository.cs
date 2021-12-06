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

        public async Task<Status> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage)
        {
            if (chatMessage.Content.Trim().Equals(""))
            {
                return BadRequest;
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

            return Status.Created;
        }

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userId)
        {
            return await (from c in _context.Chats
                          join cm in _context.ChatMessages
                          on c.Id equals cm.Chat.Id
                          where c.ChatUsers.Any(u => u.User.oid == userId)
                          orderby cm.Timestamp descending
                          select new ChatDetailsDto
                          {
                              ChatId = c.Id,
                              TargetUserId = c.ChatUsers.First(ch => ch.User.oid != userId).User.oid,
                              LatestMessageUserId = cm.FromUser.oid,
                              LatestMessageTime = cm.Timestamp,
                              LatestMessage = cm.Content,
                              SeenLatestMessage = c.ChatUsers.First().SeenLatestMessage
                          }).ToListAsync();
        }

        public async Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId) =>
            (await _context.ChatMessages.Where(c => c.Chat.Id == chatId)
                                        .Select(c => new ChatMessageDto
                                        {
                                            FromUserId = c.FromUser.oid,
                                            Content = c.Content,
                                            Timestamp = c.Timestamp

                                        })
                                        .ToListAsync())
                                        .AsReadOnly();
        private async Task<Chat> GetChatAsync(int chatId) =>
            await _context.Chats.FirstAsync(c => c.Id == chatId);

        private async Task<User> GetUserAsync(string userId) =>
            await _context.Users.FirstAsync(u => u.oid == userId);

        private async Task<Post> GetPostAsync(int postId) =>
            await _context.Posts.FirstAsync(p => p.Id == postId);
    }
}
