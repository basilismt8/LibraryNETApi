using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLLoanRepository : ILoanRepository
    {
        private readonly LibraryDbContext dbContext;

        public SQLLoanRepository(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Loan>?> CreateAsync(Guid userId, CreateLoanRequestDto createLoanRequestDto)
        {
            var createdLoans = new List<Loan>();

            foreach (var bookId in createLoanRequestDto.bookIds)
            {
                var book = await dbContext.Books.FirstOrDefaultAsync(b => b.id == bookId);

                if (book == null || book.copiesAvailable <= 0)
                {
                    // Cancel everything if even one book is invalid
                    return null;
                }

                var loan = new Loan
                {
                    id = Guid.NewGuid(),
                    bookId = book.id,
                    userId = userId,
                    dueDate = createLoanRequestDto.dueDate,
                };

                await dbContext.Loans.AddAsync(loan);

                book.copiesAvailable -= 1;
                dbContext.Books.Update(book);

                createdLoans.Add(loan);
            }

            await dbContext.SaveChangesAsync();
            return createdLoans;
        }

        public async Task<List<Loan>> getAllAsync()
        {
            return await dbContext.Loans.ToListAsync();
        }

        public async Task<Loan?> getByIdAsync(Guid id)
        {
            return await dbContext.Loans.FirstOrDefaultAsync(x => x.id == id);

        }
    }
}
