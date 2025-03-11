using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;

        public BooksController(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("getAll")]
        public IActionResult getAll() {
            var booksDomain = dbContext.Books.ToList();
            var booksDto = new List<BookDto>();

            foreach (var bookDomain in booksDomain){
                booksDto.Add(new BookDto() {
                    id = bookDomain.id,
                    title = bookDomain.title,
                    copiesAvailable = bookDomain.copiesAvailable,
                    totalCopies = bookDomain.totalCopies
                });
            }

            return Ok(booksDto);
        }

        [HttpGet("getById/{id}")]
        public IActionResult getById(Guid id)
        {
            var bookDomain = dbContext.Books.Find(id);
            //LINQ query. Is the same with the above line!!!
            //var book = dbContext.Books.FirstOrDefault(x => x.id == id);

            if (bookDomain == null) {
                return NotFound();
            }

            var bookDto = new BookDto {
                id = bookDomain.id,
                title = bookDomain.title,
                copiesAvailable = bookDomain.copiesAvailable,
                totalCopies = bookDomain.totalCopies
            };

            return Ok(bookDto);
        }
    }
}
