namespace LibraryBlazor.Features.MyLoans.Models
{
    public sealed record MyFineDto(
        Guid id,
        Guid userId,
        Guid loanId,
        decimal amount,
        bool paid,
        DateOnly fineDate,
        string bookTitle
    );
}
