using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Dto
{
    public class BookDto
    {

        public Guid id { get; set; }
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }
    }
}
