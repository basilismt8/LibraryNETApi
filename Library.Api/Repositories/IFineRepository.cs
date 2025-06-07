using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Repositories
{
    public interface IFineRepository
    {
        Task<List<Fine>> getAllAsync();
        Task<Fine?> getByIdAsync(Guid id);
        Task<Fine> addFineAsync(AddFineRequestDto addFineRequestDto);
        Task<Fine?> processOverdueLoansAsync(Guid id);
    }
}
