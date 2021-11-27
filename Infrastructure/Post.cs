namespace ProjectBank.Infrastructure
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public int SupervisorId { get; set; }

        public ICollection<Tag> Tags { get; set; } = null!;
    }
}
