namespace ProjectBank.Infrastructure
{
    public class Comment
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public User User { get; set; }

        public DateTime DateAdded { get; set; }

    }
}
