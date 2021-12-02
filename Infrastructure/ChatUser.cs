namespace ProjectBank.Infrastructure
{
    public class ChatUser
    {
        public int Id { get; set; }

        public User User { get; set; }

        public bool SeenLatestMessage { get; set; }
    }
}
