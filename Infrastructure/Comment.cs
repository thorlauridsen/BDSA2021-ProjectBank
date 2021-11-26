namespace ProjectBank.Infrastructure
{
    public class Comment
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public User Author { get; set; }

        public DateTime DateAdded { get; set; }

        public Post Post { get; set; }

        public Comment(string content, User author, DateTime dateAdded, Post post)
        {
            Content = content;
            Author = author;
            DateAdded = dateAdded;
            Post = post;
        }
    }
}
