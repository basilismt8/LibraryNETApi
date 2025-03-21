using Library.Api.Models.Domain;

namespace Library.Api.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> getAllAsync();
        Task<Loan?> getByIdAsync(Guid id);
    }
}
