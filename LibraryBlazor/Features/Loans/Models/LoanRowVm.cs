using LibraryBlazor.Features.Books.Models;
using static System.Net.WebRequestMethods;

namespace LibraryBlazor.Features.Loans.Models
{
    public sealed class LoanRowVm
    {
        public LoanRowVm(LoanDto dto)
        {
            Id = dto.id;
            BookCopyId = dto.bookCopyId;
            UserId = dto.userId;
            LoanDate = dto.loanDate;
            DueDate = dto.dueDate;
            Status = dto.status;
            BookTitle = dto.bookTitle;
        }



        public Guid Id { get; set; }
        public Guid BookCopyId { get; set; }
        public Guid UserId { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly DueDate { get; set; }
        public LoanStatus Status { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

    }
}
