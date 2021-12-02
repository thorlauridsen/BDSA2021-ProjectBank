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

        public async Task<int> CreateNewChatAsync(ChatCreateDto chat)
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

            return entityChat.Id;
        }


        public async Task<Status> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage)
        {
            var entityChatMessage = new ChatMessage
            {
                Chat = await GetChatAsync(chatMessage.ChatId),
                Content = chatMessage.Content,
                Timestamp = DateTime.Now,
                FromUser = await GetUserAsync(chatMessage.FromUserId)
            };

            //entityChatMessage.Chat.ChatUsers.Where(c => c.Id != chatMessage.FromUserId);

            foreach (var chatUser in entityChatMessage.Chat.ChatUsers)
            {
                if (chatUser.Id != chatMessage.FromUserId)
                {
                    chatUser.SeenLatestMessage = false;
                }
            }

            _context.ChatMessages.Add(entityChatMessage);
            await _context.SaveChangesAsync();

            return Status.Created;
        }

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync2(int userId) =>
            (await _context.Chats.Where(c => c.ChatUsers
                    .Any(u => u.User.Id == userId))
                .Select(c =>
                    new ChatDetailsDto
                    {
                        ChatId = c.Id,
                        TargetUserId = c.ChatUsers.First(c => c.User.Id != userId).User.Id,
                        LatestMessageUserId = GetLatestChatMessage(c.Id).Id,
                        LatestMessageTime = GetLatestChatMessage(c.Id).Timestamp,
                        LatestMessage = GetLatestChatMessage(c.Id).Content,
                        SeenLatestMessage = c.ChatUsers.First().SeenLatestMessage
                    }
                ).ToListAsync());

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(int userId)
        {
            return await (from c in _context.Chats
                join cm in _context.ChatMessages
                    on c.Id equals cm.Chat.Id
                where c.ChatUsers.Any(u => u.User.Id == userId)
                orderby cm.Timestamp descending
                select new ChatDetailsDto
                {
                    ChatId = c.Id,
                    TargetUserId = c.ChatUsers.First(cu => cu.User.Id != userId).User.Id,
                    LatestMessageUserId = cm.FromUser.Id,
                    LatestMessageTime = cm.Timestamp,
                    LatestMessage = cm.Content,
                    SeenLatestMessage = c.ChatUsers.First().SeenLatestMessage
                }).ToListAsync();
        }


        /*
        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync1(int userId)
        {
            List<ChatDetailsDto> list = new List<ChatDetailsDto>();
            await foreach (var chat in _context.Chats)
            {
                if (chat.ChatUsers.Any(u => u.User.Id == userId))
                {
                    var latest = await GetLatestChatMessage(chat.Id);
                    var dto = new ChatDetailsDto
                    {
                        ChatId = chat.Id,
                        TargetUserId = chat.ChatUsers.First(c => c.User.Id != userId).User.Id,
                        LatestMessageUserId = latest.Id,
                        LatestMessageTime = latest.Timestamp,
                        LatestMessage = latest.Content,
                        SeenLatestMessage = chat.ChatUsers.First(c => c.User.Id == userId).SeenLatestMessage
                    };
                    list.Add(dto);
                }
            }

            return list;
        }
        */

        public async Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId) =>
            (await _context.ChatMessages.Where(c => c.Chat.Id == chatId)
                .Select(c => new ChatMessageDto
                {
                    FromUserId = c.FromUser.Id,
                    Content = c.Content,
                    Timestamp = c.Timestamp
                })
                .ToListAsync())
            .AsReadOnly();

        private ChatMessage GetLatestChatMessage(int chatId) =>
            _context.ChatMessages.Where(c => c.Chat.Id == chatId)
                .OrderByDescending(c => c.Timestamp)
                .First();


        private async Task<Chat> GetChatAsync(int chatId) =>
            await _context.Chats.FirstAsync(c => c.Id == chatId);

        private async Task<User> GetUserAsync(int userId) =>
            await _context.Users.FirstAsync(u => u.Id == userId);

        private async Task<Post> GetPostAsync(int postId) =>
            await _context.Posts.FirstAsync(p => p.Id == postId);
    }
}