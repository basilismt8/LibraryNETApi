namespace Library.Api.Models.Domain
{
    public class BookCopy
    {
        public Guid id { get; set; }
        public Guid bookId { get; set; }
        public string copyCode { get; set; } = string.Empty;
        public CopyStatus status { get; set; }

        // Navigation properties
        public Book? Book { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
