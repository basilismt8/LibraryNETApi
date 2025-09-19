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

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var vm = new BooksPageViewModel
            {
                Books = await _bookService.GetAllAsync(cancellationToken)
            };
            return View(vm.Books); // keep existing view expecting IEnumerable<BookDto>
        }
    }
}
