using Library.Api.Data;
using Library.Api.Models.Domain;
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
