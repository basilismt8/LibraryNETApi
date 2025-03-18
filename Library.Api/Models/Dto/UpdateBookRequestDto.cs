namespace Library.Api.Models.Dto
{
    public class UpdateBookRequestDto
    {
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }
    }
}
