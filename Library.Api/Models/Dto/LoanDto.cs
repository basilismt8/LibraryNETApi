using Library.Api.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class LoanDto
    {
        public Guid id { get; set; }
        public Guid bookId { get; set; }
        public Guid userId { get; set; }
        public DateOnly loanDate { get; set; }
        public DateOnly dueDate { get; set; }
        public LoanStatus status { get; set; }
        // Navigation properties
        public BookDto? Book { get; set; }
        public FineDto? Fine { get; set; }
    }
}
