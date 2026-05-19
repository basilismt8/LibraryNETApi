namespace Library.Api.Models.Dto
{
    public class LoanHistoryEventDto
    {
        public Guid historyId { get; set; }
        public Guid bookCopyId { get; set; }
        public string copyCode { get; set; } = string.Empty;
        public Guid userId { get; set; }
        public string eventType { get; set; } = string.Empty; // "Loaned" or "Returned"
        public DateOnly date { get; set; }
    }
}
