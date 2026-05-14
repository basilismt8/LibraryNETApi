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

        public async Task<List<Loan>?> CreateAsync(Guid userId, CreateLoanRequestDto createLoanRequestDto, CancellationToken cancellationToken = default)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var createdLoans = new List<Loan>();

                foreach (var bookId in createLoanRequestDto.bookIds)
                {
                    var book = await dbContext.Books.FirstOrDefaultAsync(b => b.id == bookId, cancellationToken);

                    if (book == null || book.copiesAvailable <= 0)
                    {
                        // Cancel everything if even one book is invalid
                        await transaction.RollbackAsync(cancellationToken);
                        return null;
                    }

                    // Find the first available BookCopy for this book
                    var availableCopy = await dbContext.BookCopies
                        .FirstOrDefaultAsync(bc => bc.bookId == bookId && bc.status == CopyStatus.Available, cancellationToken);

                    if (availableCopy == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return null;
                    }

                    // Set BookCopy status to OnLoan
                    availableCopy.status = CopyStatus.OnLoan;
                    dbContext.BookCopies.Update(availableCopy);

                    var loan = new Loan
                    {
                        id = Guid.NewGuid(),
                        bookCopyId = availableCopy.id,
                        userId = userId,
                        loanDate = DateOnly.FromDateTime(DateTime.Now),
                        dueDate = createLoanRequestDto.dueDate,
                        status = LoanStatus.borrowed
                    };

                    await dbContext.Loans.AddAsync(loan, cancellationToken);

                    book.copiesAvailable -= 1;
                    dbContext.Books.Update(book);

                    createdLoans.Add(loan);

                    var loanDate = loan.loanDate;

                    // Update or create the LoanHistory entry for this book copy
                    var historyEntry = await dbContext.LoanHistories
                        .FirstOrDefaultAsync(lh => lh.bookCopyId == availableCopy.id, cancellationToken);

                    if (historyEntry != null)
                    {
                        // Copy was previously returned — reset for the new loan
                        historyEntry.loanDate = loanDate;
                        historyEntry.returnDate = null;
                        historyEntry.userId = userId;
                        dbContext.LoanHistories.Update(historyEntry);
                    }
                    else
                    {
                        // First time this copy is being loaned out
                        var newHistory = new LoanHistory
                        {
                            id = Guid.NewGuid(),
                            bookCopyId = availableCopy.id,
                            userId = userId,
                            loanDate = loanDate,
                            returnDate = null
                        };
                        await dbContext.LoanHistories.AddAsync(newHistory, cancellationToken);
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return createdLoans;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<Loan>? extendLoanPeriodDomainAsync(Guid id, Loan loan, CancellationToken cancellationToken = default)
        {
            var existingLoan = await dbContext.Loans.FirstOrDefaultAsync(x => x.id == id, cancellationToken);
            if (existingLoan == null)
            {
                return null;
            }

            existingLoan.dueDate = loan.dueDate;

            dbContext.Loans.Update(existingLoan);

            await dbContext.SaveChangesAsync(cancellationToken);
            return existingLoan;
        }

        public async Task<List<Loan>> getAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Loan>> getAllLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .Where(x => x.userId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Loan?> getByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .FirstOrDefaultAsync(x => x.id == id, cancellationToken);
        }

        public async Task<List<Loan>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .Where(l => l.userId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
