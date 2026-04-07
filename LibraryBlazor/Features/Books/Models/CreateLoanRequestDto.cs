namespace LibraryBlazor.Features.Books.Models;

public sealed record CreateLoanRequestDto(
    List<Guid> BookIds
);
