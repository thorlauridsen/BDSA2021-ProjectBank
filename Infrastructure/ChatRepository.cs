using Microsoft.EntityFrameworkCore;
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
            foreach (var userOid in chat.ChatUserOids)
            {
                var entityChatUser = new ChatUser
                {
                    User = await GetUserAsync(userOid),
                    SeenLatestMessage = userOid == chat.FromUserOid ? true : false,
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
                ProjectId = entityChat.Post?.Id,
                ChatUserOids = entityChat.ChatUsers.Select(cu => cu.Id).ToHashSet()
            });
        }

        public async Task<(Status, ChatMessageDetailsDto?)> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage)
        {
            if (chatMessage.Content.Trim().Equals(""))
            {
                return (BadRequest, null);
            }

            var chat = await GetChatAsync(chatMessage.ChatId);
            var entityChatMessage = new ChatMessage
            {
                Chat = chat,
                Content = chatMessage.Content,
                Timestamp = DateTime.Now,
                FromUser = await GetUserAsync(chatMessage.FromUserOid)
            };

            foreach (var chatUser in chat.ChatUsers)
            {
                chatUser.SeenLatestMessage = chatUser.User.Oid == chatMessage.FromUserOid;
                _context.ChatUsers.Update(chatUser);
            }
            entityChatMessage.Chat.ChatUsers
                .Select(cu => cu.SeenLatestMessage = cu.User.Oid == chatMessage.FromUserOid);

            _context.ChatMessages.Add(entityChatMessage);
            await _context.SaveChangesAsync();

            return (Status.Created, new ChatMessageDetailsDto()
            {
                Content = entityChatMessage.Content,
                FromUser = new UserDetailsDto
                {
                    Oid = entityChatMessage.FromUser.Oid,
                    Name = entityChatMessage.FromUser.Name,
                    Email = entityChatMessage.FromUser.Email
                },
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
                FromUser = new UserDetailsDto
                {
                    Oid = chatMessage.FromUser.Oid,
                    Name = chatMessage.FromUser.Name,
                    Email = chatMessage.FromUser.Email
                },
                Timestamp = chatMessage.Timestamp,
            };
        }

        public async Task<Status> SetSeen(int chatId, string userOid)
        {
            var chat = await _context.Chats.Include("ChatUsers.User").FirstOrDefaultAsync(c => c.Id.Equals(chatId)).ConfigureAwait(false);
            var user = chat?.ChatUsers.FirstOrDefault(u => u.User.Oid.Equals(userOid));
            if (user == null) return NotFound;

            user.SeenLatestMessage = true;
            _context.ChatUsers.Update(user);
            await _context.SaveChangesAsync();
            return Success;
        }

        public async Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userOid)
        {
            return (await _context.Chats.Include("ChatUsers")
                .Join(_context.ChatMessages, c => c.Id, cm => cm.Chat.Id, (c, cm) => new { c, cm })
                .Where(t => t.c.ChatUsers.Any(u => u.User.Oid == userOid))
                .OrderByDescending(t => t.cm.Timestamp)
                .Select(t => new ChatDetailsDto
                {
                    ChatId = t.c.Id,
                    TargetUserOid = t.c.ChatUsers.First(ch => ch.User.Oid != userOid).User.Oid,
                    LatestChatMessage = new ChatMessageDto()
                    {
                        Content = t.cm.Content,
                        FromUser = new UserDetailsDto
                        {
                            Oid = t.cm.FromUser.Oid,
                            Name = t.cm.FromUser.Name,
                            Email = t.cm.FromUser.Email
                        },
                        Timestamp = t.cm.Timestamp
                    },
                    SeenLatestMessage = t.c.ChatUsers.FirstOrDefault(chatUser => chatUser.User.Oid == userOid)
                        .SeenLatestMessage,
                    ProjectId = t.c.Post.Id

                }).ToListAsync()
                .WaitAsync(TimeSpan.FromMinutes(10))
                .ConfigureAwait(false))
                .DistinctBy(dto => dto.ChatId)
                .ToList();
        }

        public async Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId) =>
            (await _context.ChatMessages.Where(c => c.Chat.Id == chatId)
                .Select(c => new ChatMessageDto
                {
                    FromUser = new UserDetailsDto
                    {
                        Oid = c.FromUser.Oid,
                        Name = c.FromUser.Name,
                        Email = c.FromUser.Email
                    },
                    Content = c.Content,
                    Timestamp = c.Timestamp
                })
                .ToListAsync())
            .AsReadOnly();

        public async Task<ChatDetailsDto?> ReadChatAsync(int chatId, string userOid)
        {
            var result = await _context.Chats.Include("ChatUsers").Include("ChatUsers.User").Include("Post") //Få alle chats
                .Join(_context.ChatMessages.Include("FromUser"), //Join det med ChatMessages så vi får en tuple
                    chat => chat.Id,
                    chatMessage => chatMessage.Chat.Id,
                    (c, cm) => new { chat = c, chatMessage = cm })
                .FirstOrDefaultAsync(t => t.chat.Id == chatId); //Tag den første tuple

            if (result == null) return null;

            var latestChatMessage = new ChatMessageDto()
            {
                Content = result.chatMessage.Content,
                FromUser = new UserDetailsDto
                {
                    Oid = result.chatMessage.FromUser.Oid,
                    Name = result.chatMessage.FromUser.Name,
                    Email = result.chatMessage.FromUser.Email
                },
                Timestamp = result.chatMessage.Timestamp
            };

            var targetUser = result.chat.ChatUsers.First(ch => ch.User.Oid != userOid);
            var targetUserOid = targetUser.User.Oid;
            var seenLatestMessage = result.chat.ChatUsers.FirstOrDefault(chatUser => chatUser.User.Oid == userOid)?
                .SeenLatestMessage;

            return new ChatDetailsDto()
            {
                ChatId = result.chat.Id,
                LatestChatMessage = latestChatMessage,
                TargetUserOid = targetUserOid,
                SeenLatestMessage = seenLatestMessage ?? true,
                ProjectId = result.chat.Post?.Id
            };

        }

        private async Task<Chat> GetChatAsync(int chatId) =>
            await _context.Chats.Include("ChatUsers")
                                .Include("ChatUsers.User")
                                .Include("Post")
                                .FirstAsync(c => c.Id == chatId);

        private async Task<User> GetUserAsync(string userOid) =>
            await _context.Users.FirstAsync(u => u.Oid == userOid);

        private async Task<Post?> GetPostAsync(int? postId) =>
            await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
    }
}
