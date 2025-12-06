using LibraryBlazorWebApp.Models;
using System.Net.Http.Json;

namespace LibraryBlazorWebApp.Services
{
    public class BooksService : IBooksService
    {
        private readonly HttpClient _http;
        private const string BooksApi = "api/Books";

        public BooksService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<BookDto>> GetBooksAsync()
        {
            var response = await _http.GetAsync($"{BooksApi}/getAll");
            if (!response.IsSuccessStatusCode)
                return new List<BookDto>();

            return await response.Content.ReadFromJsonAsync<List<BookDto>>() ?? new List<BookDto>();
        }

        public async Task<BookDto?> GetBookAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<BookDto>($"{BooksApi}/{id}");
        }

        public async Task<bool> CreateBookAsync(CreateBookDto dto)
        {
            var response = await _http.PostAsJsonAsync($"{BooksApi}/create", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBookAsync(Guid id, UpdateBookDto dto)
        {
            var response = await _http.PutAsJsonAsync($"{BooksApi}/update/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{BooksApi}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
