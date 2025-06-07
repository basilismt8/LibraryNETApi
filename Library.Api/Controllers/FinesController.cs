using AutoMapper;
using Library.Api.CustomActionFilters;
using Library.Api.Data;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;
        private readonly IFineRepository fineRepository;
        private readonly IMapper mapper;

        public FinesController(LibraryDbContext dbContext, IFineRepository fineRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.fineRepository = fineRepository;
            this.mapper = mapper;
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> getAll()
        {
            var finesDomain = await fineRepository.getAllAsync();

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<FineDto>>(finesDomain));
        }

        [HttpGet("getById/{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> getById(Guid id)
        {
            var fineDomain = await fineRepository.getByIdAsync(id);

            if (fineDomain == null)
            {
                return NotFound();
            }

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<FineDto>(fineDomain));
        }

        [HttpPost("addFine")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> addFine([FromBody] AddFineRequestDto addFineRequestDto)
        {

            var fineDomains = await fineRepository.addFineAsync(addFineRequestDto);

            if (fineDomains == null)
            {
                return BadRequest($"Loan with ID '{addFineRequestDto.loanId}' was not found.");
            }


            return Ok(mapper.Map<FineDto>(fineDomains));

        }

        [HttpPost("processOverdueLoans/{id:Guid}")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> processOverdueLoans(Guid id)
        {

            var fineDomains = await fineRepository.processOverdueLoansAsync(id);

            if (fineDomains == null)
            {
                return BadRequest("Something went wrong...");
            }

            return Ok(mapper.Map<List<FineDto>>(fineDomains));
        }
    }
}
