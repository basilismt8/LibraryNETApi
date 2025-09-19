using Library.Web.Models.DTO;

namespace Library.Web.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(BookDto book, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, BookDto book, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
