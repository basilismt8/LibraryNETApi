using Library.Api.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class LoanDto
    {
        public Guid id { get; set; }
        public Guid bookCopyId { get; set; }
        public Guid userId { get; set; }
        public DateOnly loanDate { get; set; }
        public DateOnly dueDate { get; set; }
        public LoanStatus status { get; set; }
        public string bookTitle { get; set; } = string.Empty;
        public BookCopyDto? BookCopy { get; set; }
    }
}
