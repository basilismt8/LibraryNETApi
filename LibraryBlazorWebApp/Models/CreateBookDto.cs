namespace LibraryBlazorWebApp.Models
{
    public class CreateBookDto
    {
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }
    }
}
