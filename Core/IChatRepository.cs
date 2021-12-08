namespace ProjectBank.Core
{

    public interface IChatRepository
    {
        Task<(Status, ChatDto?)> CreateNewChatAsync(ChatCreateDto chat);
        Task<(Status, ChatMessageDetailsDto?)> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage);
        Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(string userId);
        Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId);
        Task<ChatDetailsDto?> ReadChatAsync(int chatId, string userId);
        Task<ChatMessageDto?> ReadSpecificMessageAsync(int chatMessageId);
    }
}
