namespace LibraryBlazor.Features.Books.Models;

public sealed record CreateBookRequestDto(
    string Title,
    int CopiesAvailable,
    int TotalCopies
);
