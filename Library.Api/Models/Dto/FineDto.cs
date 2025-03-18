using Library.Api.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class FineDto
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid loanId { get; set; }
        public decimal amount { get; set; }
        public bool paid { get; set; }
        public DateOnly fineDate { get; set; }
        //Navigation properties
        public required LoanDto Loan { get; set; }
    }
}
