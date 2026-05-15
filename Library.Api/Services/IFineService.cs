using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface IFineService
    {
        Task<ServiceResult<List<FineDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResult<FineDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<FineDto>> AddFineAsync(AddFineRequestDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<FineDto>>> ProcessOverdueLoansAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<List<FineDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ServiceResult<FineDto>> PayFineAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
