namespace LibraryBlazor.Shared.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CopiesAvailable { get; set; }
        public int TotalCopies { get; set; }
    }
}
