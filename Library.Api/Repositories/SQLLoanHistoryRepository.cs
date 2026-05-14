using Library.Api.Data;
using Library.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLLoanHistoryRepository : ILoanHistoryRepository
    {
        private readonly LibraryDbContext dbContext;

        public SQLLoanHistoryRepository(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<LoanHistory> AddAsync(LoanHistory loanHistory, CancellationToken cancellationToken = default)
        {
            await dbContext.LoanHistories.AddAsync(loanHistory, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return loanHistory;
        }

        public async Task<LoanHistory?> SetReturnDateAsync(Guid bookCopyId, Guid userId, CancellationToken cancellationToken = default)
        {
            var record = await dbContext.LoanHistories
                .Where(lh => lh.bookCopyId == bookCopyId && lh.userId == userId && lh.returnDate == null)
                .OrderByDescending(lh => lh.loanDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (record == null)
                return null;

            record.returnDate = DateOnly.FromDateTime(DateTime.Today);
            dbContext.LoanHistories.Update(record);
            await dbContext.SaveChangesAsync(cancellationToken);
            return record;
        }

        public async Task<List<LoanHistory>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.LoanHistories
                .OrderByDescending(lh => lh.loanDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LoanHistory>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbContext.LoanHistories
                .Where(lh => lh.userId == userId)
                .OrderByDescending(lh => lh.loanDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LoanHistory>> GetByBookCopyIdAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            return await dbContext.LoanHistories
                .Where(lh => lh.bookCopyId == bookCopyId)
                .OrderByDescending(lh => lh.loanDate)
                .ToListAsync(cancellationToken);
        }
    }
}
