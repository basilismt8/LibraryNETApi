using LibraryBlazor.Features.Books.Models;
using LibraryBlazor.Http;

namespace LibraryBlazor.Features.Books.Services;

public sealed class BookApi
{
    private readonly ApiClient _api;

    public BookApi(ApiClient api)
    {
        _api = api;
    }

    public Task<IReadOnlyList<BookDto>?> GetBooksAsync(CancellationToken cancellationToken = default)
        => _api.GetAsync<IReadOnlyList<BookDto>>("api/books/getAll", cancellationToken);

    public Task<BookDto?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _api.GetAsync<BookDto>($"api/books/getById/{id}", cancellationToken);

    public Task CreateBookAsync(CreateBookRequestDto body, CancellationToken cancellationToken = default)
        => _api.PostAsync("api/books/create", new
        {
            title = body.Title,
            copiesAvailable = body.CopiesAvailable,
            totalCopies = body.TotalCopies
        }, cancellationToken);

    public Task UpdateBookAsync(Guid id, UpdateBookRequestDto body, CancellationToken cancellationToken = default)
        => _api.PutAsync($"api/books/update/{id}", new
        {
            title = body.Title,
            copiesAvailable = body.CopiesAvailable,
            totalCopies = body.TotalCopies
        }, cancellationToken);

    public Task DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
        => _api.DeleteAsync($"api/books/delete/{id}", cancellationToken);

}
