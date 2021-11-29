namespace ProjectBank.Infrastructure
{
    public class Chat
    {
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }
    }
}
