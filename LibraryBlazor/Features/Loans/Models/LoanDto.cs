namespace LibraryBlazor.Features.Loans.Models
{
    public sealed record LoanDto(
         Guid id,
         Guid bookId,
         Guid userId,
         DateOnly loanDate,
         DateOnly dueDate,
         LoanStatus status
     );
}
