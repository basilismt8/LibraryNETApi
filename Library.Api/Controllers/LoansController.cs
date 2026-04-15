using AutoMapper;
using Library.Api.CustomActionFilters;
using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILoanRepository loanRepository;
        private readonly IMapper mapper;
        private readonly ILogger<LoansController> logger;

        public LoansController(LibraryDbContext dbContext, ILoanRepository loanRepository, IMapper mapper, ILogger<LoansController> logger)
        {
            this.dbContext = dbContext;
            this.loanRepository = loanRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll()
        {
            var loansDomain = await loanRepository.getAllAsync();

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<LoanDto>>(loansDomain));
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUserLoans()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);

            var loansDomain = await loanRepository.getAllLoansByUserIdAsync(userId);

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<LoanDto>>(loansDomain));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var loanDomain = await loanRepository.getByIdAsync(id);

            if (loanDomain == null)
            {
                return NotFound();
            }

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<LoanDto>(loanDomain));
        }

        [HttpPost]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> Create([FromBody] CreateLoanRequestDto createLoanRequestDto)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);

            var loanDomains = await loanRepository.CreateAsync(userId, createLoanRequestDto);

            if (loanDomains == null)
            {
                return BadRequest("One or more books not found or unavailable.");
            }

            var loanDtos = loanDomains.Select(mapper.Map<LoanDto>).ToList();

            return Ok(loanDtos);

        }


        [HttpPut("{id:Guid}/extend")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ExtendLoanPeriod([FromRoute] Guid id, [FromBody] ExtendLoanRequestDto extendLoanRequestDto)
        {
            var extendLoanPeriodDomain = mapper.Map<Loan>(extendLoanRequestDto);

            var existingLoan = await loanRepository.getByIdAsync(id);  // Assuming you have a getByIdAsync method

            if (existingLoan == null)
            {
                return NotFound("Loan not found.");
            }

            if (existingLoan.status == LoanStatus.overdue)
            {
                return BadRequest("Cannot extend an overdue loan.");
            }

            if (existingLoan.dueDate >= extendLoanPeriodDomain.dueDate)
            {
                return BadRequest("New due date must be after the current due date.");
            }

            extendLoanPeriodDomain = await loanRepository.extendLoanPeriodDomainAsync(id, extendLoanPeriodDomain);

            if (extendLoanPeriodDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<LoanDto>(extendLoanPeriodDomain));
        }
    }
}
