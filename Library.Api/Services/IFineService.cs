using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface IFineService
    {
        Task<List<FineDto>> GetAllAsync();
        Task<ServiceResult<FineDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<FineDto>> AddFineAsync(AddFineRequestDto dto);
        Task<ServiceResult<List<FineDto>>> ProcessOverdueLoansAsync(Guid userId);
    }
}
