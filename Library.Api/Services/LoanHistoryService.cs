using AutoMapper;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;

namespace Library.Api.Services
{
    public class LoanHistoryService : ILoanHistoryService
    {
        private readonly ILoanHistoryRepository loanHistoryRepository;
        private readonly IMapper mapper;

        public LoanHistoryService(ILoanHistoryRepository loanHistoryRepository, IMapper mapper)
        {
            this.loanHistoryRepository = loanHistoryRepository;
            this.mapper = mapper;
        }

        public async Task<LoanHistoryDto> RecordLoanAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default)
        {
            var record = new LoanHistory
            {
                id = Guid.NewGuid(),
                bookCopyId = bookCopyId,
                userId = userId,
                loanDate = DateOnly.FromDateTime(DateTime.Today),
                returnDate = null
            };

            var saved = await loanHistoryRepository.AddAsync(record, cancellationToken);
            return mapper.Map<LoanHistoryDto>(saved);
        }

        public async Task<LoanHistoryDto?> RecordReturnAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default)
        {
            var updated = await loanHistoryRepository.SetReturnDateAsync(bookCopyId, userId, cancellationToken);
            return updated == null ? null : mapper.Map<LoanHistoryDto>(updated);
        }

        public async Task<List<LoanHistoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var records = await loanHistoryRepository.GetAllAsync(cancellationToken);
            return mapper.Map<List<LoanHistoryDto>>(records);
        }

        public async Task<List<LoanHistoryDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var records = await loanHistoryRepository.GetByUserIdAsync(userId, cancellationToken);
            return mapper.Map<List<LoanHistoryDto>>(records);
        }

        public async Task<List<LoanHistoryDto>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            var records = await loanHistoryRepository.GetByBookCopyIdAsync(bookCopyId, cancellationToken);
            return mapper.Map<List<LoanHistoryDto>>(records);
        }
    }
}
