namespace LibraryBlazor.Features.MyLoans.Models
{
    public sealed class MyFineRowVm
    {
        public MyFineRowVm(MyFineDto dto)
        {
            Id = dto.id;
            UserId = dto.userId;
            LoanId = dto.loanId;
            Amount = dto.amount;
            Paid = dto.paid;
            FineDate = dto.fineDate;
            BookTitle = dto.bookTitle;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LoanId { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public DateOnly FineDate { get; set; }
        public string BookTitle { get; set; }
    }
}
