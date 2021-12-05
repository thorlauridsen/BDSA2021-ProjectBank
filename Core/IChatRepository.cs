namespace ProjectBank.Core
{

    public interface IChatRepository
    {
        Task<(Status, ChatDto?)> CreateNewChatAsync(ChatCreateDto chat);
        Task<Status> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage);
        Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(int userId);
        Task<IReadOnlyCollection<ChatMessageDto>> ReadSpecificChatAsync(int chatId);
    }
}
