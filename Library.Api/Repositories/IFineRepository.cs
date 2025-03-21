using Library.Api.Models.Domain;

namespace Library.Api.Repositories
{
    public interface IFineRepository
    {
        Task<List<Fine>> getAllAsync();
        Task<Fine?> getByIdAsync(Guid id);
    }
}
