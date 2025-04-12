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

        [HttpGet("getAll")]
        public async Task<IActionResult> getAll()
        {
            var loansDomain = await loanRepository.getAllAsync();

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<LoanDto>>(loansDomain));
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> getById(Guid id)
        {
            var loanDomain = await loanRepository.getByIdAsync(id);

            if (loanDomain == null)
            {
                return NotFound();
            }

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<LoanDto>(loanDomain));
        }

        [HttpPost("create")]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequestDto createLoanRequestDto)
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
    }
}
