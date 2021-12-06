namespace ProjectBank.Infrastructure
{
    public class Comment
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public string UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public int PostId { get; set; }
    }
}
