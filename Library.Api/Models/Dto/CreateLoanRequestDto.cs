using Library.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class CreateLoanRequestDto
    {
        [Required]
        public List<Guid> bookIds { get; set; } = new();
        [Required]
        [DueDateCustomValidation(ErrorMessage = "Due date must be at least one day after today.")]
        public DateOnly dueDate { get; set; }
    }
}
