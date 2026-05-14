namespace Library.Api.Models.Domain
{
    public class LoanHistory
    {
        public Guid id { get; set; }
        public Guid bookCopyId { get; set; }
        public Guid userId { get; set; }
        public DateOnly loanDate { get; set; }
        public DateOnly? returnDate { get; set; }
    }
}
