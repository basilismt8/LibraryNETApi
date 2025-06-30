namespace Library.Api.Models.Dto
{
    public class ReturnBooksRequesDto
    {
        public List<Guid> BookIds { get; set; }
        public Guid UserId { get; set; }
    }
}
