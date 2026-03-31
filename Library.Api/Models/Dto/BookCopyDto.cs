using Library.Api.Models.Domain;

namespace Library.Api.Models.Dto
{
    public class BookCopyDto
    {
        public Guid id { get; set; }
        public Guid bookId { get; set; }
        public string copyCode { get; set; } = string.Empty;
        public CopyStatus status { get; set; }
    }
}
