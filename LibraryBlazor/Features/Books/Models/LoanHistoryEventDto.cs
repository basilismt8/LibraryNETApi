namespace LibraryBlazor.Features.Books.Models;

public sealed class LoanHistoryEventDto
{
    public Guid HistoryId { get; set; }
    public Guid BookCopyId { get; set; }
    public string CopyCode { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}
