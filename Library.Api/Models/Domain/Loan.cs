using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Domain
{
    public class Loan
    {
        public Guid id { get; set; }
        public Guid bookId { get; set; }
        public Guid userId { get; set; }
        public DateOnly loanDate { get; set; }
        [Required]
        public DateOnly dueDate { get; set; }
        public LoanStatus status { get; set; }

        // Navigation properties
        public Book? Book { get; set; }
        public Fine? Fine { get; set; }

    }
}

public enum LoanStatus
{
    borrowed,
    returned,
    overdue
}
