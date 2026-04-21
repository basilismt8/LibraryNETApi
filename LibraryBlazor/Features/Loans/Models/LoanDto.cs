namespace LibraryBlazor.Features.Loans.Models
{
    public sealed record LoanDto(
         Guid id,
         Guid bookCopyId,
         Guid userId,
         DateOnly loanDate,
         DateOnly dueDate,
         LoanStatus status,
         string bookTitle
     );
}
