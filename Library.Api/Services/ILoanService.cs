using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface ILoanService
    {
        Task<List<LoanDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResult<LoanDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<LoanDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<LoanDto>>> CreateAsync(Guid userId, CreateLoanRequestDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<LoanDto>> ExtendAsync(Guid loanId, ExtendLoanRequestDto dto, CancellationToken cancellationToken = default);
    }
}
