namespace ProjectBank.Core
{
    public record NotificationCreateDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public int UserId { get; set; }

        public string Link { get; set; }
    }

    public record NotificationReadDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public string Link { get; set; }

        public bool Seen { get; set; }
    }
}
