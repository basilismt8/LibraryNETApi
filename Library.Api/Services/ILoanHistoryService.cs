using Library.Api.Models.Dto;
using Library.Api.Models;

namespace Library.Api.Services
{
    public interface ILoanHistoryService
    {
        Task<ServiceResult<LoanHistoryDto>> RecordLoanAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<LoanHistoryDto>> RecordReturnAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<LoanHistoryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResult<List<LoanHistoryDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<LoanHistoryEventDto>>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<LoanHistoryEventDto>>> GetAllPairedAsync(CancellationToken cancellationToken = default);
        Task<ServiceResult<Dictionary<Guid, List<LoanHistoryEventDto>>>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    }
}
