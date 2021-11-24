namespace ProjectBank.Infrastructure
{
    public class Post
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public Post(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
