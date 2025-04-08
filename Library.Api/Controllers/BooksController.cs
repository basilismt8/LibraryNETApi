using AutoMapper;
using Azure.Core.Serialization;
using Library.Api.CustomActionFilters;
using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext dbContext;
        private readonly IBookRepository bookRepository;
        private readonly IMapper mapper;
        private readonly ILogger<BooksController> logger;

        public BooksController(LibraryDbContext dbContext,
            IBookRepository bookRepository, IMapper mapper, ILogger<BooksController> logger)
        {
            this.dbContext = dbContext;
            this.bookRepository = bookRepository;
            this.bookRepository = bookRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet("getAll")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> getAll() {
            logger.LogInformation("Get all books called");
            var booksDomain = await bookRepository.getAllAsync();
            logger.LogInformation($"all books are {JsonSerializer.Serialize(booksDomain)}");
            //Map Domain Model to DTO and return it
            return Ok(mapper.Map<List<BookDto>>(booksDomain));
        }

        [HttpGet("getById/{id}")]
        [Authorize(Roles = "Librarian,Member")]
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
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> createBook([FromBody] CreateBookRequestDto createBookRequestDto)
        {

            var bookDomain = mapper.Map<Book>(createBookRequestDto);

            bookDomain = await bookRepository.CreateAsync(bookDomain);

            return CreatedAtAction(nameof(getById), new { bookDomain.id }, mapper.Map<BookDto>(bookDomain));
        }

        [HttpPut("update/{id:Guid}")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
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
        [Authorize(Roles = "Librarian")]
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
