namespace ProjectBank.Infrastructure
{
    public class Comment
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public Supervisor Author { get; set; }

        public DateTime DateAdded { get; set; }

        public ICollection<Post> Post { get; set; } = null!;


        public Comment(string content, Supervisor author, DateTime dateAdded)
        {
            Content = content;
            Author = author;
            DateAdded = dateAdded;
        }
    }
}
