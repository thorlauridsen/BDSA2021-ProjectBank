namespace ProjectBank.Infrastructure
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public Chat Chat { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public User FromUser { get; set; }
    }
}
