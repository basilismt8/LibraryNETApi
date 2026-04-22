using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface ILoanService
    {
        Task<List<LoanDto>> GetAllAsync();
        Task<ServiceResult<LoanDto>> GetByIdAsync(Guid id);
        Task<List<LoanDto>> GetByUserIdAsync(Guid userId);
        Task<ServiceResult<List<LoanDto>>> CreateAsync(Guid userId, CreateLoanRequestDto dto);
        Task<ServiceResult<LoanDto>> ExtendAsync(Guid loanId, ExtendLoanRequestDto dto);
    }
}
