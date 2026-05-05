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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/books called by {User}", User.Identity?.Name);
            var books = await _bookService.GetAllAsync(cancellationToken);
            return Ok(books);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _bookService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create([FromBody] CreateBookRequestDto createBookRequestDto, CancellationToken cancellationToken)
        {
            var result = await _bookService.CreateAsync(createBookRequestDto, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.id }, result.Data);
        }

        [HttpPut("{id}")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateBookRequestDto updateBookRequestDto, CancellationToken cancellationToken)
        {
            var result = await _bookService.UpdateAsync(id, updateBookRequestDto, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _bookService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPut("returnBook")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBooksRequesDto returnBooksRequest, CancellationToken cancellationToken)
        {
            var result = await _bookService.ReturnBookAsync(returnBooksRequest.UserId, returnBooksRequest.BookCopyId, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
