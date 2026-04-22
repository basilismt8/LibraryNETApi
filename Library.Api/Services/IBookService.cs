using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<ServiceResult<BookDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<BookDto>> CreateAsync(CreateBookRequestDto dto);
        Task<ServiceResult<BookDto>> UpdateAsync(string id, UpdateBookRequestDto dto);
        Task<ServiceResult<BookDto>> DeleteAsync(Guid id);
        Task<ServiceResult<BookDto>> ReturnBookAsync(Guid userId, Guid bookCopyId);
    }
}
