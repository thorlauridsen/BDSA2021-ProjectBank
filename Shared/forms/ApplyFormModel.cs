using System.ComponentModel.DataAnnotations;

namespace ProjectBank.Shared.forms
{
    public class ApplyFormModel
    {
        [Required]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Cover letter length needs to be at least 5 letters")]
        public string Content { get; set; }
    }
}
