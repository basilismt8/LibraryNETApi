using AutoMapper;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;

namespace Library.Api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all books");
            var books = await _bookRepository.getAllAsync();
            _logger.LogInformation("Retrieved {Count} books", books.Count);
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<ServiceResult<BookDto>> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.getByIdAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Book {Id} not found", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found.");
            }
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(book));
        }

        public async Task<ServiceResult<BookDto>> CreateAsync(CreateBookRequestDto dto)
        {
            _logger.LogInformation("Creating book '{Title}'", dto.title);
            var bookDomain = _mapper.Map<Book>(dto);
            var created = await _bookRepository.CreateAsync(bookDomain);
            _logger.LogInformation("Book '{Title}' created with id {Id}", created.title, created.id);
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(created));
        }

        public async Task<ServiceResult<BookDto>> UpdateAsync(string id, UpdateBookRequestDto dto)
        {
            _logger.LogInformation("Updating book {Id}", id);
            var bookDomain = _mapper.Map<Book>(dto);
            var updated = await _bookRepository.UpdateAsync(id, bookDomain);
            if (updated == null)
            {
                _logger.LogWarning("Book {Id} not found for update", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found.");
            }
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(updated));
        }

        public async Task<ServiceResult<BookDto>> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting book {Id}", id);
            var deleted = await _bookRepository.DeleteAsync(id);
            if (deleted == null)
            {
                _logger.LogWarning("Book {Id} not found or has active loans — delete aborted", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found or has active loans.");
            }
            _logger.LogInformation("Book {Id} deleted", id);
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(deleted));
        }

        public async Task<ServiceResult<BookDto>> ReturnBookAsync(Guid userId, Guid bookCopyId)
        {
            _logger.LogInformation("User {UserId} returning book copy {BookCopyId}", userId, bookCopyId);
            var returned = await _bookRepository.ReturnBookAsync(userId, bookCopyId);
            if (returned == null)
            {
                _logger.LogWarning("Return failed for copy {BookCopyId} by user {UserId}", bookCopyId, userId);
                return ServiceResult<BookDto>.NotFound("Book copy not found or not currently on loan by this user.");
            }
            _logger.LogInformation("Book copy {BookCopyId} returned by user {UserId}", bookCopyId, userId);
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(returned));
        }
    }
}
