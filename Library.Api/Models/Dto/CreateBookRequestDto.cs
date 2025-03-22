using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class CreateBookRequestDto
    {
        [Required]
        [MinLength(30, ErrorMessage = "Title must have a minimum 30 characters")]
        [MaxLength(255, ErrorMessage = "Title must have a maximum 255 characters")]
        public string title { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "copiesAvailable must be at least 1.")]
        public int copiesAvailable { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "totalCopies must be at least 1.")]
        public int totalCopies { get; set; }
    }
}
