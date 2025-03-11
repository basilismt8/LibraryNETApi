using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models.Domain
{
    public class Book
    {
        public Guid id { get; set; }

        [Required]
        public string title { get; set; } = string.Empty;
        public int copiesAvailable { get; set; }
        public int totalCopies { get; set; }

        // Navigation property: One book can have many loans
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
