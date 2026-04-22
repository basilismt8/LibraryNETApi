using Library.Api.CustomActionFilters;
using Library.Api.Models.Dto;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/books called by {User}", User.Identity?.Name);
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookService.GetByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create([FromBody] CreateBookRequestDto createBookRequestDto)
        {
            var result = await _bookService.CreateAsync(createBookRequestDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.id }, result.Data);
        }

        [HttpPut("{id}")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateBookRequestDto updateBookRequestDto)
        {
            var result = await _bookService.UpdateAsync(id, updateBookRequestDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _bookService.DeleteAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPut("returnBook")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBooksRequesDto returnBooksRequest)
        {
            var result = await _bookService.ReturnBookAsync(returnBooksRequest.UserId, returnBooksRequest.BookCopyId);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
