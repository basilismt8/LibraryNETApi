using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Domain
{
    public class Fine
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid loanId { get; set; }
        [Required] // Ensures NOT NULL
        [Column(TypeName = "decimal(6,2)")] 
        public decimal amount { get; set; }
        public bool paid { get; set; }
        public DateOnly fineDate { get; set; }

        //Navigation properties
        public required Loan Loan { get; set; }
    }
}
