using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class CreateBookRequestDto : IValidatableObject
    {
        [Required]
        [MinLength(10, ErrorMessage = "Title must have a minimum 10 characters")]
        [MaxLength(255, ErrorMessage = "Title must have a maximum 255 characters")]
        public string title { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Copies Available must be at least 1.")]
        public int copiesAvailable { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Total Copies must be at least 1.")]
        public int totalCopies { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (copiesAvailable > totalCopies)
            {
                yield return new ValidationResult(
                    "Copies Available cannot be greater than Total Copies.",
                    new[] { nameof(copiesAvailable) });
            }
        }
    }
}
