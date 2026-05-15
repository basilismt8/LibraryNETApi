using AutoMapper;
using Library.Api.Models.Dto;
using Library.Api.Repositories;

namespace Library.Api.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fineRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FineService> _logger;

        public FineService(IFineRepository fineRepository, IMapper mapper, ILogger<FineService> logger)
        {
            _fineRepository = fineRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<FineDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all fines");
            var fines = await _fineRepository.getAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} fines", fines.Count);
            return ServiceResult<List<FineDto>>.Ok(_mapper.Map<List<FineDto>>(fines));
        }

        public async Task<ServiceResult<FineDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var fine = await _fineRepository.getByIdAsync(id, cancellationToken);
            if (fine == null)
            {
                _logger.LogWarning("Fine {Id} not found", id);
                return ServiceResult<FineDto>.NotFound($"Fine with id '{id}' was not found.");
            }
            return ServiceResult<FineDto>.Ok(_mapper.Map<FineDto>(fine));
        }

        public async Task<ServiceResult<FineDto>> AddFineAsync(AddFineRequestDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding fine for loan {LoanId}", dto.loanId);
            var fine = await _fineRepository.addFineAsync(dto, cancellationToken);
            if (fine == null)
            {
                _logger.LogWarning("Loan {LoanId} not found when adding fine", dto.loanId);
                return ServiceResult<FineDto>.NotFound($"Loan with id '{dto.loanId}' was not found.");
            }
            _logger.LogInformation("Fine added for loan {LoanId}, amount {Amount}", dto.loanId, dto.amount);
            return ServiceResult<FineDto>.Ok(_mapper.Map<FineDto>(fine));
        }

        public async Task<ServiceResult<List<FineDto>>> ProcessOverdueLoansAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing overdue loans for user {UserId}", userId);
            var fines = await _fineRepository.processOverdueLoansAsync(userId, cancellationToken);
            if (fines == null)
            {
                _logger.LogWarning("Failed to process overdue loans for user {UserId}", userId);
                return ServiceResult<List<FineDto>>.BadRequest("Something went wrong while processing overdue loans.");
            }
            _logger.LogInformation("Processed {Count} fine(s) for user {UserId}", fines.Count, userId);
            return ServiceResult<List<FineDto>>.Ok(_mapper.Map<List<FineDto>>(fines));
        }

        public async Task<ServiceResult<List<FineDto>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving fines for user {UserId}", userId);
            var fines = await _fineRepository.getByUserIdAsync(userId, cancellationToken);
            _logger.LogInformation("Retrieved {Count} fines for user {UserId}", fines.Count, userId);
            return ServiceResult<List<FineDto>>.Ok(_mapper.Map<List<FineDto>>(fines));
        }

        public async Task<ServiceResult<FineDto>> PayFineAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Paying fine {FineId}", id);
            var fine = await _fineRepository.payFineAsync(id, cancellationToken);
            if (fine == null)
            {
                _logger.LogWarning("Fine {FineId} not found or already paid", id);
                return ServiceResult<FineDto>.NotFound($"Fine with id '{id}' was not found or is already paid.");
            }
            _logger.LogInformation("Fine {FineId} marked as paid", id);
            return ServiceResult<FineDto>.Ok(_mapper.Map<FineDto>(fine));
        }
    }
}
