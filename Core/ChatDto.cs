namespace ProjectBank.Core
{
    public record ChatCreateDto
    {
        public ChatCreateDto(
            int projectId,
            int fromUserId,
            string content,
            HashSet<int> chatUserIds)
        {
            ProjectId = projectId;
            FromUserId = fromUserId;
            ChatUserIds = chatUserIds;
        }

        public int ProjectId { get; init; }
        public int FromUserId { get; init; }
        public HashSet<int> ChatUserIds { get; init; }
    }

    public record ChatMessageCreateDto
    {
        public int FromUserId { get; init; }
        public int ChatId { get; init; }
        public string Content { get; init; }
    }

    public record ChatMessageDto
    {
        public int FromUserId { get; init; }

        public string Content { get; init; }

        public DateTime Timestamp { get; init; }
    }

    public record ChatDetailsDto
    {
        public int ChatId { get; init; }

        public int TargetUserId { get; init; }

        public int LatestMessageUserId { get; init; }

        public DateTime LatestMessageTime { get; init; }

        public string LatestMessage { get; init; }

        public bool SeenLatestMessage { get; init; }
    }
}
