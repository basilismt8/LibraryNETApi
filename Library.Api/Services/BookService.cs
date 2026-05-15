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

        public async Task<ServiceResult<List<BookDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all books");
            var books = await _bookRepository.getAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} books", books.Count);
            return ServiceResult<List<BookDto>>.Ok(_mapper.Map<List<BookDto>>(books));
        }

        public async Task<ServiceResult<BookDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var book = await _bookRepository.getByIdAsync(id, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning("Book {Id} not found", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found.");
            }
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(book));
        }

        public async Task<ServiceResult<BookDto>> CreateAsync(CreateBookRequestDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating book '{Title}'", dto.title);
            var bookDomain = _mapper.Map<Book>(dto);
            var created = await _bookRepository.CreateAsync(bookDomain, cancellationToken);
            _logger.LogInformation("Book '{Title}' created with id {Id}", created.title, created.id);
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(created));
        }

        public async Task<ServiceResult<BookDto>> UpdateAsync(string id, UpdateBookRequestDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating book {Id}", id);
            var bookDomain = _mapper.Map<Book>(dto);
            var updated = await _bookRepository.UpdateAsync(id, bookDomain, cancellationToken);
            if (updated == null)
            {
                _logger.LogWarning("Book {Id} not found for update", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found.");
            }
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(updated));
        }

        public async Task<ServiceResult<BookDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting book {Id}", id);
            var deleted = await _bookRepository.DeleteAsync(id, cancellationToken);
            if (deleted == null)
            {
                _logger.LogWarning("Book {Id} not found or has active loans — delete aborted", id);
                return ServiceResult<BookDto>.NotFound($"Book with id '{id}' was not found or has active loans.");
            }
            _logger.LogInformation("Book {Id} deleted", id);
            return ServiceResult<BookDto>.Ok(_mapper.Map<BookDto>(deleted));
        }

        public async Task<ServiceResult<BookDto>> ReturnBookAsync(Guid userId, Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("User {UserId} returning book copy {BookCopyId}", userId, bookCopyId);
            var returned = await _bookRepository.ReturnBookAsync(userId, bookCopyId, cancellationToken);
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
