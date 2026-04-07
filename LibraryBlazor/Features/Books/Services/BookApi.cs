using LibraryBlazor.Features.Books.Models;
using LibraryBlazor.Http;
using System.Text.Json;

namespace LibraryBlazor.Features.Books.Services;

public sealed class BookApi
{
    private readonly ApiClient _api;

    public BookApi(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResult<IReadOnlyList<BookDto>>> GetBooksAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<BookDto>>("api/books/getAll", cancellationToken);

    public Task<ApiResult<BookDto>> GetBookByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _api.GetResultAsync<BookDto>($"api/books/getById/{id}", cancellationToken);

    public Task<ApiResult> CreateBookAsync(CreateBookRequestDto body, CancellationToken cancellationToken = default)
    => _api.PostAsync("api/books/create", new
    {
        title = body.Title,
        copiesAvailable = body.CopiesAvailable,
        totalCopies = body.TotalCopies
    }, cancellationToken);

    public Task<ApiResult> CreateLoanAsync(CreateLoanRequestDto body, CancellationToken cancellationToken = default)
    => _api.PostAsync("api/loans/create", new
    {
        bookIds = body.BookIds,
        dueDate = DateTime.Now.ToString("yyyy-MM-dd")
    }, cancellationToken);

    public Task<ApiResult> UpdateBookAsync( Guid id, UpdateBookRequestDto body, CancellationToken cancellationToken = default)
    => _api.PutAsync($"api/books/update/{id}", new
    {
        title = body.Title,
        copiesAvailable = body.CopiesAvailable,
        totalCopies = body.TotalCopies
    }, cancellationToken);

    public Task<ApiResult> DeleteBookAsync(Guid id, CancellationToken cancellationToken = default)
        => _api.DeleteResultAsync($"api/books/delete/{id}", cancellationToken);

}
