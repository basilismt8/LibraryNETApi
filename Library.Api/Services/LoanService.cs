using AutoMapper;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;

namespace Library.Api.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, IMapper mapper, ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<LoanDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all loans");
            var loans = await _loanRepository.getAllAsync();
            _logger.LogInformation("Retrieved {Count} loans", loans.Count);
            return _mapper.Map<List<LoanDto>>(loans);
        }

        public async Task<ServiceResult<LoanDto>> GetByIdAsync(Guid id)
        {
            var loan = await _loanRepository.getByIdAsync(id);
            if (loan == null)
            {
                _logger.LogWarning("Loan {Id} not found", id);
                return ServiceResult<LoanDto>.NotFound($"Loan with id '{id}' was not found.");
            }
            return ServiceResult<LoanDto>.Ok(_mapper.Map<LoanDto>(loan));
        }

        public async Task<List<LoanDto>> GetByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving loans for user {UserId}", userId);
            var loans = await _loanRepository.getAllLoansByUserIdAsync(userId);
            return _mapper.Map<List<LoanDto>>(loans);
        }

        public async Task<ServiceResult<List<LoanDto>>> CreateAsync(Guid userId, CreateLoanRequestDto dto)
        {
            _logger.LogInformation("Creating loans for user {UserId}, books: {BookIds}", userId, string.Join(", ", dto.bookIds));
            var loans = await _loanRepository.CreateAsync(userId, dto);
            if (loans == null)
            {
                _logger.LogWarning("Loan creation failed for user {UserId} — one or more books unavailable", userId);
                return ServiceResult<List<LoanDto>>.BadRequest("One or more books were not found or have no available copies.");
            }
            _logger.LogInformation("Created {Count} loan(s) for user {UserId}", loans.Count, userId);
            return ServiceResult<List<LoanDto>>.Ok(_mapper.Map<List<LoanDto>>(loans));
        }

        public async Task<ServiceResult<LoanDto>> ExtendAsync(Guid loanId, ExtendLoanRequestDto dto)
        {
            _logger.LogInformation("Extending loan {LoanId} to {NewDueDate}", loanId, dto.dueDate);

            var loan = await _loanRepository.getByIdAsync(loanId);
            if (loan == null)
            {
                _logger.LogWarning("Loan {LoanId} not found for extension", loanId);
                return ServiceResult<LoanDto>.NotFound("Loan not found.");
            }

            if (loan.status == LoanStatus.overdue)
                return ServiceResult<LoanDto>.BadRequest("Cannot extend an overdue loan.");

            if (loan.status == LoanStatus.returned)
                return ServiceResult<LoanDto>.BadRequest("Cannot extend a returned loan.");

            if (loan.dueDate >= dto.dueDate)
                return ServiceResult<LoanDto>.BadRequest("New due date must be after the current due date.");

            var loanDomain = _mapper.Map<Loan>(dto);
            var updated = await _loanRepository.extendLoanPeriodDomainAsync(loanId, loanDomain);
            if (updated == null)
                return ServiceResult<LoanDto>.NotFound("Loan not found.");

            _logger.LogInformation("Loan {LoanId} extended to {NewDueDate}", loanId, dto.dueDate);
            return ServiceResult<LoanDto>.Ok(_mapper.Map<LoanDto>(updated));
        }
    }
}
