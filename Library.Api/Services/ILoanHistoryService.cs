using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface ILoanHistoryService
    {
        Task<LoanHistoryDto> RecordLoanAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default);
        Task<LoanHistoryDto?> RecordReturnAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default);
        Task<List<LoanHistoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<LoanHistoryDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<LoanHistoryDto>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default);
    }
}
