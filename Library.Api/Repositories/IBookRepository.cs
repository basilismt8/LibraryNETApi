using Library.Api.Models.Domain;

namespace Library.Api.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> getAllAsync();
        Task<Book?> getByIdAsync(Guid id);
        Task<Book> CreateAsync(Book book);
        Task<Book?> UpdateAsync(string id, Book book);
        Task<Book?> DeleteAsync(Guid id);
        Task<List<Book>> RerturnBookAsync(Guid userId, List<Guid> bookIds);
    }
}
