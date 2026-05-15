using AutoMapper;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;

namespace Library.Api.Services
{
    public class LoanHistoryService : ILoanHistoryService
    {
        private readonly ILoanHistoryRepository _loanHistoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanHistoryService> _logger;

        public LoanHistoryService(ILoanHistoryRepository loanHistoryRepository, IMapper mapper, ILogger<LoanHistoryService> logger)
        {
            _loanHistoryRepository = loanHistoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<LoanHistoryDto>> RecordLoanAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Recording loan for BookCopyId {BookCopyId}, UserId {UserId}", bookCopyId, userId);
            var record = new LoanHistory
            {
                id = Guid.NewGuid(),
                bookCopyId = bookCopyId,
                userId = userId,
                loanDate = DateOnly.FromDateTime(DateTime.Today),
                returnDate = null
            };

            var saved = await _loanHistoryRepository.AddAsync(record, cancellationToken);
            _logger.LogInformation("Loan history record created with Id {Id}", saved.id);
            return ServiceResult<LoanHistoryDto>.Ok(_mapper.Map<LoanHistoryDto>(saved));
        }

        public async Task<ServiceResult<LoanHistoryDto>> RecordReturnAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Recording return for BookCopyId {BookCopyId}, UserId {UserId}", bookCopyId, userId);
            var updated = await _loanHistoryRepository.SetReturnDateAsync(bookCopyId, userId, cancellationToken);
            if (updated == null)
            {
                _logger.LogWarning("No open loan history found for BookCopyId {BookCopyId}, UserId {UserId}", bookCopyId, userId);
                return ServiceResult<LoanHistoryDto>.NotFound("No open loan history found for this book copy and user.");
            }
            _logger.LogInformation("Return recorded for history Id {Id}", updated.id);
            return ServiceResult<LoanHistoryDto>.Ok(_mapper.Map<LoanHistoryDto>(updated));
        }

        public async Task<ServiceResult<List<LoanHistoryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all loan history records");
            var records = await _loanHistoryRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} loan history records", records.Count);
            return ServiceResult<List<LoanHistoryDto>>.Ok(_mapper.Map<List<LoanHistoryDto>>(records));
        }

        public async Task<ServiceResult<List<LoanHistoryDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving loan history for UserId {UserId}", userId);
            var records = await _loanHistoryRepository.GetByUserIdAsync(userId, cancellationToken);
            _logger.LogInformation("Retrieved {Count} loan history records for UserId {UserId}", records.Count, userId);
            return ServiceResult<List<LoanHistoryDto>>.Ok(_mapper.Map<List<LoanHistoryDto>>(records));
        }

        public async Task<ServiceResult<List<LoanHistoryEventDto>>> GetAllPairedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all paired loan history events");
            var events = await _loanHistoryRepository.GetAllPairedAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} paired loan history events", events.Count);
            return ServiceResult<List<LoanHistoryEventDto>>.Ok(events);
        }

        public async Task<ServiceResult<List<LoanHistoryEventDto>>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving paired loan history events for BookCopyId {BookCopyId}", bookCopyId);
            var events = await _loanHistoryRepository.GetByBookCopyIdPairedAsync(bookCopyId, cancellationToken);
            _logger.LogInformation("Retrieved {Count} paired loan history events for BookCopyId {BookCopyId}", events.Count, bookCopyId);
            return ServiceResult<List<LoanHistoryEventDto>>.Ok(events);
        }
    }
}
