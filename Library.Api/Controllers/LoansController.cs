using AutoMapper;
using Library.Api.Data;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILoanRepository loanRepository;
        private readonly IMapper mapper;

        public LoansController(LibraryDbContext dbContext, ILoanRepository loanRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.loanRepository = loanRepository;
            this.mapper = mapper;
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
    }
}
