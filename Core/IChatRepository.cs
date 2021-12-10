namespace ProjectBank.Core
{

    public interface IChatRepository
    {
        Task<(Status, ChatDto?)> CreateNewChatAsync(ChatCreateDto chat);
        Task<(Status, ChatMessageDetailsDto?)> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage);
        Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userOid);
        Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId);
        Task<ChatDetailsDto?> ReadChatAsync(int chatId, string userOid);
        Task<ChatMessageDto?> ReadSpecificMessageAsync(int chatMessageId);
        Task<Status> SetSeen(int chatId, string userOid);
    }
}
