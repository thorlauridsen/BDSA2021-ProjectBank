namespace ProjectBank.Core
{
    public record ChatCreateDto
    {
        public int? ProjectId { get; init; }
        public string FromUserOid { get; init; }
        public HashSet<string> ChatUserOids { get; init; }
    }

    public record ChatMessageCreateDto
    {
        public string FromUserOid { get; init; }
        public int ChatId { get; init; }
        public string Content { get; init; }
    }

    public record ChatMessageDto
    {
        public UserDetailsDto FromUser { get; init; }

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
        public int? ProjectId { get; init; }
        public HashSet<int> ChatUserOids { get; init; }
    }

    public record ChatDetailsDto
    {
        public int ChatId { get; init; }

        public int? ProjectId { get; set; }
        public string TargetUserOid { get; init; }

        public ChatMessageDto LatestChatMessage { get; init; }

        public bool SeenLatestMessage { get; init; }
    }
}
