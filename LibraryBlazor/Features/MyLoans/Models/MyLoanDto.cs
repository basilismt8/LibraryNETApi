namespace LibraryBlazor.Features.MyLoans.Models
{
    public sealed record MyLoanDto(
         Guid id,
         Guid bookCopyId,
         Guid userId,
         DateOnly loanDate,
         DateOnly dueDate,
         MyLoanStatus status,
         string bookTitle
     );
}
