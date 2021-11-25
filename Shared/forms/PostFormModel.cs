using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Shared.forms
{
    public class PostFormModel
    {   
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Title length needs to be between 5-50")]
        public string Title { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Content length needs to be between 5-50")]
        public string Content { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Content length needs to be between 5-50")]
        public string Tags { get; set; }

    }
}