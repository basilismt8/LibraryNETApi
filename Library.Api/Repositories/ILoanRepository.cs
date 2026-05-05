using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> getAllAsync(CancellationToken cancellationToken = default);
        Task<List<Loan>> getAllLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Loan?> getByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Loan>?> CreateAsync(Guid userId, CreateLoanRequestDto createLoanRequestDto, CancellationToken cancellationToken = default);
        Task<Loan>? extendLoanPeriodDomainAsync(Guid id, Loan loan, CancellationToken cancellationToken = default);
        Task<List<Loan>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
