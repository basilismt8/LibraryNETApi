namespace LibraryBlazorWebApp.Models
{
    public class UpdateBookDto
    {
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }
    }
}
