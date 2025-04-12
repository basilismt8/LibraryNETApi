using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class ExtendLoanRequestDto
    {
        [Required]
        public DateOnly dueDate { get; set; }
    }
}
