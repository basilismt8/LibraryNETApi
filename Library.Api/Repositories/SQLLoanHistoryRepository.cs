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
                .Join(dbContext.BookCopies,
                    lh => lh.bookCopyId,
                    bc => bc.id,
                    (lh, bc) => new { lh, bc.copyCode })
                .OrderBy(x => x.lh.loanDate)
                .ToListAsync(cancellationToken);

            var groups = records.GroupBy(r => (r.lh.bookCopyId, r.lh.loanDate));

            var result = new List<LoanHistoryEventDto>();

            foreach (var group in groups)
            {
                var openEntry = group.FirstOrDefault(r => r.lh.returnDate == null);
                var closedEntry = group.FirstOrDefault(r => r.lh.returnDate != null);

                var source = openEntry ?? closedEntry!;
                result.Add(new LoanHistoryEventDto
                {
                    historyId = source.lh.id,
                    bookCopyId = source.lh.bookCopyId,
                    copyCode = source.copyCode,
                    userId = source.lh.userId,
                    eventType = "Loaned",
                    date = source.lh.loanDate
                });

                if (closedEntry != null)
                {
                    result.Add(new LoanHistoryEventDto
                    {
                        historyId = closedEntry.lh.id,
                        bookCopyId = closedEntry.lh.bookCopyId,
                        copyCode = closedEntry.copyCode,
                        userId = closedEntry.lh.userId,
                        eventType = "Returned",
                        date = closedEntry.lh.returnDate!.Value
                    });
                }
            }

            return result;
        }

        public async Task<List<LoanHistoryEventDto>> GetByBookCopyIdPairedAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
        {
            var records = await dbContext.LoanHistories
                .Where(lh => lh.bookCopyId == bookCopyId)
                .Join(dbContext.BookCopies,
                    lh => lh.bookCopyId,
                    bc => bc.id,
                    (lh, bc) => new { lh, bc.copyCode })
                .OrderBy(x => x.lh.loanDate)
                .ToListAsync(cancellationToken);

            var result = new List<LoanHistoryEventDto>();

            foreach (var record in records)
            {
                result.Add(new LoanHistoryEventDto
                {
                    historyId = record.lh.id,
                    bookCopyId = record.lh.bookCopyId,
                    copyCode = record.copyCode,
                    userId = record.lh.userId,
                    eventType = "Loaned",
                    date = record.lh.loanDate
                });

                if (record.lh.returnDate.HasValue)
                {
                    result.Add(new LoanHistoryEventDto
                    {
                        historyId = record.lh.id,
                        bookCopyId = record.lh.bookCopyId,
                        copyCode = record.copyCode,
                        userId = record.lh.userId,
                        eventType = "Returned",
                        date = record.lh.returnDate.Value
                    });
                }
            }

            return result;
        }

        public async Task<Dictionary<Guid, List<LoanHistoryEventDto>>> GetByBookIdPairedAsync(Guid bookId, CancellationToken cancellationToken = default)
        {
            var copies = await dbContext.BookCopies
                .Where(bc => bc.bookId == bookId)
                .Select(bc => new { bc.id, bc.copyCode })
                .ToListAsync(cancellationToken);

            var copyIds = copies.Select(c => c.id).ToList();
            var copyCodeMap = copies.ToDictionary(c => c.id, c => c.copyCode);

            var records = await dbContext.LoanHistories
                .Where(lh => copyIds.Contains(lh.bookCopyId))
                .OrderBy(lh => lh.loanDate)
                .ToListAsync(cancellationToken);

            var result = new Dictionary<Guid, List<LoanHistoryEventDto>>();

            foreach (var record in records)
            {
                if (!result.ContainsKey(record.bookCopyId))
                    result[record.bookCopyId] = new List<LoanHistoryEventDto>();

                var code = copyCodeMap.GetValueOrDefault(record.bookCopyId, string.Empty);

                result[record.bookCopyId].Add(new LoanHistoryEventDto
                {
                    historyId = record.id,
                    bookCopyId = record.bookCopyId,
                    copyCode = code,
                    userId = record.userId,
                    eventType = "Loaned",
                    date = record.loanDate
                });

                if (record.returnDate.HasValue)
                {
                    result[record.bookCopyId].Add(new LoanHistoryEventDto
                    {
                        historyId = record.id,
                        bookCopyId = record.bookCopyId,
                        copyCode = code,
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
