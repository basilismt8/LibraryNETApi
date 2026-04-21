namespace LibraryBlazor.Features.MyLoans.Models
{
    public sealed class MyLoanRowVm
    {
        public MyLoanRowVm(MyLoanDto dto)
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
        public MyLoanStatus Status { get; set; }
        public string BookTitle { get; set; }
        public bool IsSelected { get; set; }
    }
}
