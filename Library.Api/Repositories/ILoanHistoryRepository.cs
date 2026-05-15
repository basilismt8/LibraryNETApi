using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Repositories
{
    public interface ILoanHistoryRepository
    {
        Task<LoanHistory> AddAsync(LoanHistory loanHistory, CancellationToken cancellationToken = default);
        Task<LoanHistory?> SetReturnDateAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default);
        Task<List<LoanHistory>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<LoanHistory>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<LoanHistory>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default);
        Task<List<LoanHistoryEventDto>> GetAllPairedAsync(CancellationToken cancellationToken = default);
        Task<List<LoanHistoryEventDto>> GetByBookCopyIdPairedAsync(Guid bookCopyId, CancellationToken cancellationToken = default);
    }
}
