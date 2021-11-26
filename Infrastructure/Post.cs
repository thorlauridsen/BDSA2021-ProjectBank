namespace ProjectBank.Infrastructure
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public Supervisor Author { get; set; }

        public ICollection<Tag> Tags { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = null!;

        public Post(string title, string content, DateTime dateAdded, Supervisor author, ICollection<Tag> tags, ICollection<Comment> comments)
        {
            Title = title;
            Content = content;
            Author = author;
            Tags = tags;
            Comments = comments;
            DateAdded = dateAdded;
        }
    }
}
