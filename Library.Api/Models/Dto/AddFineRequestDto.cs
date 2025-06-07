using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class AddFineRequestDto
    {
        [Required]
        public Guid userId { get; set; }

        [Required]
        public Guid loanId { get; set; }

        [Required]
        [Range(0.01, 9999.99)]
        public decimal amount { get; set; }

        public bool paid { get; set; } = false;
    }
}
