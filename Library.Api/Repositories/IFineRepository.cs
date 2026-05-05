using Library.Api.Models.Domain;
using Library.Api.Models.Dto;

namespace Library.Api.Repositories
{
    public interface IFineRepository
    {
        Task<List<Fine>> getAllAsync(CancellationToken cancellationToken = default);
        Task<Fine?> getByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Fine> addFineAsync(AddFineRequestDto addFineRequestDto, CancellationToken cancellationToken = default);
        Task<List<Fine>> processOverdueLoansAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Fine>> getByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
