using Library.Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            List<BookDto> responseBody = new List<BookDto>();
            try
            {
                var client = httpClientFactory.CreateClient();

                var booksResponse = await client.GetAsync("https://localhost:7256/api/Books/getAll");
                booksResponse.EnsureSuccessStatusCode();
                responseBody.AddRange(await booksResponse.Content.ReadFromJsonAsync<IEnumerable<BookDto>>());
            }
            catch (Exception e)
            {

                throw;
            }

            return View(responseBody);
        }
    }
}
