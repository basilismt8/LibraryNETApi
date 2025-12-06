using Library.Web.Models.DTO;

namespace Library.Web.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<string> CreateAsync(CreateBookDto book, CancellationToken cancellationToken = default);
        Task<string> UpdateAsync(string id, UpdateBookDto book, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
