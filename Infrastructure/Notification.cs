using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Infrastructure
{
    public class Notification
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public User User { get; set; }

        public string Link { get; set; }

        public bool Seen { get; set; }
    }
}
