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
        public async Task<IActionResult> getAll()
        {
            var finesDomain = await fineRepository.getAllAsync();

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<FineDto>>(finesDomain));
        }

        [HttpGet("getById/{id}")]
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
    }
}
