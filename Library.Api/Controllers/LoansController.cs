using Library.Api.Data;
using Library.Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;

        public LoansController(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("getAll")]
        public IActionResult getAll()
        {
            var loansDomain = dbContext.Loans.ToList();
            var loansDto = new List<LoanDto>();

            foreach (var loanDomain in loansDomain)
            {
                loansDto.Add(new LoanDto()
                {
                    id = loanDomain.id,
                    bookId = loanDomain.bookId,
                    userId = loanDomain.userId,
                    loanDate = loanDomain.loanDate,
                    dueDate = loanDomain.dueDate,
                    status = loanDomain.status
                });
            }

            return Ok(loansDto);
        }

        [HttpGet("getById/{id}")]
        public IActionResult getById(Guid id)
        {
            var loanDomain = dbContext.Loans.Find(id);
            //LINQ query. Is the same with the above line!!!
            //var book = dbContext.Books.FirstOrDefault(x => x.id == id);

            if (loanDomain == null)
            {
                return NotFound();
            }

            var loanDto = new LoanDto
            {
                id = loanDomain.id,
                bookId = loanDomain.bookId,
                userId = loanDomain.userId,
                loanDate = loanDomain.loanDate,
                dueDate = loanDomain.dueDate,
                status = loanDomain.status
            };

            return Ok(loanDto);
        }
    }
}
