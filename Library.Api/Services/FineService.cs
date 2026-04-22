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

        public async Task<List<FineDto>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all fines");
            var fines = await _fineRepository.getAllAsync();
            _logger.LogInformation("Retrieved {Count} fines", fines.Count);
            return _mapper.Map<List<FineDto>>(fines);
        }

        public async Task<ServiceResult<FineDto>> GetByIdAsync(Guid id)
        {
            var fine = await _fineRepository.getByIdAsync(id);
            if (fine == null)
            {
                _logger.LogWarning("Fine {Id} not found", id);
                return ServiceResult<FineDto>.NotFound($"Fine with id '{id}' was not found.");
            }
            return ServiceResult<FineDto>.Ok(_mapper.Map<FineDto>(fine));
        }

        public async Task<ServiceResult<FineDto>> AddFineAsync(AddFineRequestDto dto)
        {
            _logger.LogInformation("Adding fine for loan {LoanId}", dto.loanId);
            var fine = await _fineRepository.addFineAsync(dto);
            if (fine == null)
            {
                _logger.LogWarning("Loan {LoanId} not found when adding fine", dto.loanId);
                return ServiceResult<FineDto>.NotFound($"Loan with id '{dto.loanId}' was not found.");
            }
            _logger.LogInformation("Fine added for loan {LoanId}, amount {Amount}", dto.loanId, dto.amount);
            return ServiceResult<FineDto>.Ok(_mapper.Map<FineDto>(fine));
        }

        public async Task<ServiceResult<List<FineDto>>> ProcessOverdueLoansAsync(Guid userId)
        {
            _logger.LogInformation("Processing overdue loans for user {UserId}", userId);
            var fines = await _fineRepository.processOverdueLoansAsync(userId);
            if (fines == null)
            {
                _logger.LogWarning("Failed to process overdue loans for user {UserId}", userId);
                return ServiceResult<List<FineDto>>.BadRequest("Something went wrong while processing overdue loans.");
            }
            _logger.LogInformation("Processed {Count} fine(s) for user {UserId}", fines.Count, userId);
            return ServiceResult<List<FineDto>>.Ok(_mapper.Map<List<FineDto>>(fines));
        }
    }
}
