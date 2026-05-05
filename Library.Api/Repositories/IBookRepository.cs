using Library.Api.Models.Domain;

namespace Library.Api.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> getAllAsync(CancellationToken cancellationToken = default);
        Task<Book?> getByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Book> CreateAsync(Book book, CancellationToken cancellationToken = default);
        Task<Book?> UpdateAsync(string id, Book book, CancellationToken cancellationToken = default);
        Task<Book?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Loan?> BorrowBookAsync(Guid userId, Guid bookId, DateOnly dueDate, CancellationToken cancellationToken = default);
        Task<Book?> ReturnBookAsync(Guid userId, Guid bookCopyId, CancellationToken cancellationToken = default);
    }
}
