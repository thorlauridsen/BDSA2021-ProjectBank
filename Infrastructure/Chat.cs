namespace ProjectBank.Infrastructure
{
    public class Chat
    {
        public int Id { get; set; }

        public Post? Post { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}
