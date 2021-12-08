namespace ProjectBank.Core
{
    public record ChatCreateDto
    {
        public int ProjectId { get; init; }
        public string FromUserId { get; init; }
        public HashSet<string> ChatUserIds { get; init; }
    }

    public record ChatMessageCreateDto
    {
        public string FromUserId { get; init; }
        public int ChatId { get; init; }
        public string Content { get; init; }
    }

    public record ChatMessageDto
    {
        public UserDto FromUser { get; init; }

        public string Content { get; init; }

        public DateTime Timestamp { get; init; }
    }

    public record ChatMessageDetailsDto : ChatMessageDto
    {
        public int chatId { get; init; }

        public int chatMessageId { get; init; }
    }

    public record ChatDto
    {
        public int ChatId { get; init; }
        public int ProjectId { get; init; }
        public HashSet<int> ChatUserIds { get; init; }
    }

    public record ChatDetailsDto
    {
        public int ChatId { get; init; }

        public int ProjectId { get; set; }
        public string TargetUserId { get; init; }

        public ChatMessageDto LatestChatMessage { get; init; }

        public bool SeenLatestMessage { get; init; }
    }
}
