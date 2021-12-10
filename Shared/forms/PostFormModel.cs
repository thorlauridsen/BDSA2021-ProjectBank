using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Shared.forms
{
    public class PostFormModel
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9 -/]+", ErrorMessage = "Title should not contain special characters")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Title length needs to be between 5-255")]
        public string Title { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Content length needs to be at least 5")]
        public string Content { get; set; }

        public string Tags { get; set; }
    }
}
