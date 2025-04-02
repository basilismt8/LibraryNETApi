using AutoMapper;
using Library.Api.CustomActionFilters;
using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;

        public BooksController(LibraryDbContext dbContext,
            IBookRepository bookRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.bookRepository = bookRepository;
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> getAll() {
            var booksDomain = await bookRepository.getAllAsync();

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<BookDto>>(booksDomain));
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> getById(Guid id)
        {
            var bookDomain = await bookRepository.getByIdAsync(id);

            if (bookDomain == null) {
                return NotFound();
            }

            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<BookDto>(bookDomain));
        }

        [HttpPost("create")]
        [validateModel]
        public async Task<IActionResult> createBook([FromBody] CreateBookRequestDto createBookRequestDto)
        {

            var bookDomain = mapper.Map<Book>(createBookRequestDto);

            bookDomain = await bookRepository.CreateAsync(bookDomain);

            return CreatedAtAction(nameof(getById), new { bookDomain.id }, mapper.Map<BookDto>(bookDomain));
        }

        [HttpPut("update/{id:Guid}")]
        [validateModel]
        public async Task<IActionResult> updateBook([FromRoute] Guid id, [FromBody] UpdateBookRequestDto updateBookRequestDto)
        {
            var bookDomain = mapper.Map<Book>(updateBookRequestDto);

            bookDomain = await bookRepository.UpdateAsync(id, bookDomain);

            if (bookDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<BookDto>(bookDomain));
        }

        [HttpDelete("delete/{id:Guid}")]
        public async Task<IActionResult> deleteBook([FromRoute] Guid id)
        {
            var bookDomain = await bookRepository.DeleteAsync(id);
            if (bookDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<BookDto>(bookDomain));
        }
    }
}
