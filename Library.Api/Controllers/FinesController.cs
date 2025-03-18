using Library.Api.Data;
using Library.Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;

        public FinesController(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("getAll")]
        public IActionResult getAll()
        {
            var finesDomain = dbContext.Fines.ToList();
            var finesDto = new List<FineDto>();

            foreach (var fineDomain in finesDomain)
            {
                finesDto.Add(new FineDto()
                {
                    id = fineDomain.id,
                    userId = fineDomain.userId,
                    loanId = fineDomain.loanId,
                    amount = fineDomain.amount,
                    paid = fineDomain.paid,
                    fineDate = fineDomain.fineDate
                });
            }

            return Ok(finesDto);
        }

        [HttpGet("getById/{id}")]
        public IActionResult getById(Guid id)
        {
            var fineDomain = dbContext.Fines.Find(id);
            //LINQ query. Is the same with the above line!!!
            //var book = dbContext.Books.FirstOrDefault(x => x.id == id);

            if (fineDomain == null)
            {
                return NotFound();
            }

            var fineDto = new FineDto
            {
                id = fineDomain.id,
                userId = fineDomain.userId,
                loanId = fineDomain.loanId,
                amount = fineDomain.amount,
                paid = fineDomain.paid,
                fineDate = fineDomain.fineDate
            };

            return Ok(fineDto);
        }
    }
}
