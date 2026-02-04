namespace LibraryBlazor.Features.Books.Models;

public sealed record BookDto(
    Guid id,
    string title,
    int copiesAvailable,
    int totalCopies
);
