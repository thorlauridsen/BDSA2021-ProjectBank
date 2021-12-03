namespace ProjectBank.Infrastructure
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        public DateTime DateAdded { get; set; }

        public User User { get; set; } = null!;

        public ICollection<Tag> Tags { get; set; } = null!;
    }
}
