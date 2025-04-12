using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> getAllAsync();
        Task<Loan?> getByIdAsync(Guid id);
        Task<List<Loan>?> CreateAsync(Guid userId, CreateLoanRequestDto createLoanRequestDto);
        Task<Loan>? extendLoanPeriodDomainAsync(Guid id, Loan loan);

    }
}
