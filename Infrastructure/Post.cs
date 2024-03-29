using System.ComponentModel.DataAnnotations;
using ProjectBank.Core;

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

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public string[]? Tags { get; set; } = null!;

        public PostState PostState { get; set; }

        public int ViewCount { get; set; }
    }
}
