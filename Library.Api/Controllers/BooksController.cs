using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;

        public IBookRepository bookRepository { get; }

        public BooksController(LibraryDbContext dbContext, IBookRepository bookRepository)
        {
            this.dbContext = dbContext;
            this.bookRepository = bookRepository;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> getAll() {
            var booksDomain = await bookRepository.getAllAsync();
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
        public async Task<IActionResult> getById(Guid id)
        {
            var bookDomain = await bookRepository.getByIdAsync(id);

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

        [HttpPost("create")]
        public async Task<IActionResult> createBook([FromBody] CreateBookRequestDto createBookRequestDto) {
            var bookDomain = new Book {
                title = createBookRequestDto.title,
                copiesAvailable = createBookRequestDto.copiesAvailable,
                totalCopies = createBookRequestDto.totalCopies
            };

            bookDomain = await bookRepository.CreateAsync(bookDomain);

            var bookDto = new BookDto {
                id = bookDomain.id,
                title = bookDomain.title,
                copiesAvailable = bookDomain.copiesAvailable,
                totalCopies = bookDomain.totalCopies
            };

            return CreatedAtAction(nameof(getById), new { bookDomain.id }, bookDomain);
        }

        [HttpPut("update")]
        [Route("{id:Guid}")]
        public async Task<IActionResult> updateBook([FromRoute] Guid id, [FromBody] UpdateBookRequestDto updateBookRequestDto) {
            var bookDomain = new Book
            {
                title = updateBookRequestDto.title,
                copiesAvailable = updateBookRequestDto.copiesAvailable,
                totalCopies = updateBookRequestDto.totalCopies
            };

            bookDomain = await bookRepository.UpdateAsync(id, bookDomain);

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

        [HttpDelete("delete")]
        [Route("{id:Guid}")]
        public async Task<IActionResult> deleteBook([FromRoute] Guid id)
        {
            var bookDomain = await bookRepository.DeleteAsync(id);
            if (bookDomain == null)
            {
                return NotFound();
            }

            //optional: return deleted Book back
            var bookDto = new BookDto
            {
                id = bookDomain.id,
                title = bookDomain.title,
                copiesAvailable = bookDomain.copiesAvailable,
                totalCopies = bookDomain.totalCopies
            };

            return Ok(bookDto);
        }
    }
}
