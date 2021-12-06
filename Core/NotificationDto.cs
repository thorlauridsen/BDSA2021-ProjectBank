namespace ProjectBank.Core
{
    public record NotificationCreateDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string UserOid { get; set; }

        public string Link { get; set; }
    }

    public record NotificationDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string UserOid { get; set; }

        public DateTime Timestamp { get; set; }

        public string Link { get; set; }

        public bool Seen { get; set; }
    }
}
