namespace LibraryBlazor.Features.Books.Models;

public sealed class BookRowVm
{
    public BookRowVm(BookDto dto)
    {
        Id = dto.id;
        Title = dto.title;
        CopiesAvailable = dto.copiesAvailable;
        TotalCopies = dto.totalCopies;
    }

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CopiesAvailable { get; set; }
    public int TotalCopies { get; set; }
    public bool IsSelected { get; set; }
}