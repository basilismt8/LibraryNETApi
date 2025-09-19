using Library.Web.Models.DTO;

namespace Library.Web.Models.ViewModels
{
    public class BooksPageViewModel
    {
        public IEnumerable<BookDto> Books { get; set; } = Enumerable.Empty<BookDto>();
    }
}
