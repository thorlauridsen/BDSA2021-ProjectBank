namespace ProjectBank.Core
{

    public interface IChatRepository
    {
        Task<int> CreateNewChatAsync(ChatCreateDto chat);
        Task<Status> CreateNewChatMessageAsync(ChatMessageCreateDto chatMessage);
        Task<IReadOnlyCollection<ChatDetailsDto>> ReadAllChatsAsync(int userId);
        Task<IReadOnlyCollection<ChatMessageDto>> ReadChatAsync(int chatId);
    }
}
