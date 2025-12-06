using LibraryBlazorWebApp.Models;

namespace LibraryBlazorWebApp.Services
{
    public interface IBooksService
    {
        Task<List<BookDto>> GetBooksAsync();
        Task<BookDto?> GetBookAsync(Guid id);
        Task<bool> CreateBookAsync(CreateBookDto dto);
        Task<bool> UpdateBookAsync(Guid id, UpdateBookDto dto);
        Task<bool> DeleteBookAsync(Guid id);
    }
}
