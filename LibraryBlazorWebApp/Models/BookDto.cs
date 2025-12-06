namespace LibraryBlazorWebApp.Models
{
    public class BookDto
    {
        public Guid id { get; set; }
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }
    }
}
