using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
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

        public async Task<List<LoanHistoryEventDto>> GetAllPairedAsync(CancellationToken cancellationToken = default)
        {
            var records = await dbContext.LoanHistories
                .OrderBy(lh => lh.loanDate)
                .ToListAsync(cancellationToken);

            var groups = records.GroupBy(r => (r.bookCopyId, r.loanDate));

            var result = new List<LoanHistoryEventDto>();

            foreach (var group in groups)
            {
                var openEntry = group.FirstOrDefault(r => r.returnDate == null);
                var closedEntry = group.FirstOrDefault(r => r.returnDate != null);

                var source = openEntry ?? closedEntry!;
                result.Add(new LoanHistoryEventDto
                {
                    historyId = source.id,
                    bookCopyId = source.bookCopyId,
                    userId = source.userId,
                    eventType = "Loaned",
                    date = source.loanDate
                });

                if (closedEntry != null)
                {
                    result.Add(new LoanHistoryEventDto
                    {
                        historyId = closedEntry.id,
                        bookCopyId = closedEntry.bookCopyId,
                        userId = closedEntry.userId,
                        eventType = "Returned",
                        date = closedEntry.returnDate!.Value
                    });
                }
            }

            return result;
        }

        public async Task<List<LoanHistoryEventDto>> GetByBookCopyIdPairedAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            var records = await dbContext.LoanHistories
                .Where(lh => lh.bookCopyId == bookCopyId)
                .OrderBy(lh => lh.loanDate)
                .ToListAsync(cancellationToken);

            var result = new List<LoanHistoryEventDto>();

            foreach (var record in records)
            {
                result.Add(new LoanHistoryEventDto
                {
                    historyId = record.id,
                    bookCopyId = record.bookCopyId,
                    userId = record.userId,
                    eventType = "Loaned",
                    date = record.loanDate
                });

                if (record.returnDate.HasValue)
                {
                    result.Add(new LoanHistoryEventDto
                    {
                        historyId = record.id,
                        bookCopyId = record.bookCopyId,
                        userId = record.userId,
                        eventType = "Returned",
                        date = record.returnDate.Value
                    });
                }
            }

            return result;
        }
    }
}
