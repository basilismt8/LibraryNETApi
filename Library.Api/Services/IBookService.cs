using Library.Api.Models.Dto;

namespace Library.Api.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResult<BookDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<BookDto>> CreateAsync(CreateBookRequestDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<BookDto>> UpdateAsync(string id, UpdateBookRequestDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResult<BookDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResult<BookDto>> ReturnBookAsync(Guid userId, Guid bookCopyId, CancellationToken cancellationToken = default);
    }
}
