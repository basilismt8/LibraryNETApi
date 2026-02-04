namespace LibraryBlazor.Features.Books.Models;

public sealed record UpdateBookRequestDto(
    string Title,
    int CopiesAvailable,
    int TotalCopies
);
