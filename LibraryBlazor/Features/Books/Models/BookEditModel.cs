namespace LibraryBlazor.Features.Books.Models;

public sealed class BookEditModel
{
    public string Title { get; set; } = string.Empty;
    public int CopiesAvailable { get; set; }
    public int TotalCopies { get; set; }
}