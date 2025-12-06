using Library.Web.Models.DTO;
using Library.Web.Models.ViewModels;
using Library.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var vm = new BooksPageViewModel
            {
                Books = await _bookService.GetAllAsync(cancellationToken)
            };
            return View(vm); // pass the view model
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookDto book, CancellationToken cancellationToken)
        {
            var id = await _bookService.CreateAsync(book, cancellationToken);
            return Json(new { success = true, id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateBookDto book, CancellationToken cancellationToken)
        {
            var idUpdated = await _bookService.UpdateAsync(id, book, cancellationToken);
            return Json(new { success = true, id });
        }
    }
}
