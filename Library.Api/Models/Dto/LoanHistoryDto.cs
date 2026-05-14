namespace Library.Api.Models.Dto
{
    public class LoanHistoryDto
    {
        public Guid id { get; set; }
        public Guid bookCopyId { get; set; }
        public Guid userId { get; set; }
        public DateOnly loanDate { get; set; }
        public DateOnly? returnDate { get; set; }
    }
}
