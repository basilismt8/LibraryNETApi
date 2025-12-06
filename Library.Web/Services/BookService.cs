using Library.Web.Models.DTO;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Library.Web.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _http;
        private readonly ILogger<BookService> _logger;
        private const string BasePath = "https://localhost:7256/api/Books";

        public BookService(HttpClient httpClient, ILogger<BookService> logger)
        {
            _http = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _http.GetAsync($"{BasePath}/getAll", cancellationToken);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<IEnumerable<BookDto>>(cancellationToken: cancellationToken);
                return data ?? Enumerable.Empty<BookDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch books");
                return Enumerable.Empty<BookDto>();
            }
        }

        public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _http.GetAsync($"{BasePath}/{id}", cancellationToken);
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<BookDto>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch this book {Id}", id);
                return null;
            }
        }

        public async Task<string> CreateAsync(CreateBookDto book, CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsJsonAsync($"{BasePath}/create", book, cancellationToken);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<BookDto>(cancellationToken: cancellationToken);
            return created?.title ?? string.Empty;
        }

        public async Task<string> UpdateAsync(string id, UpdateBookDto book, CancellationToken cancellationToken = default)
        {
            var response = await _http.PutAsJsonAsync($"{BasePath}/update/{id}", book, cancellationToken);
            response.EnsureSuccessStatusCode();
            var updated = await response.Content.ReadFromJsonAsync<BookDto>(cancellationToken: cancellationToken);
            return updated?.title ?? string.Empty;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"{BasePath}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
