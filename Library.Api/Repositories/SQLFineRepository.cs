using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLFineRepository : IFineRepository
    {
        private readonly LibraryDbContext dbContext;

        public SQLFineRepository(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Fine?> addFineAsync(AddFineRequestDto addFineRequestDto, CancellationToken cancellationToken = default)
        {
            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.id == addFineRequestDto.loanId, cancellationToken);

            if (loan == null)
            {
                // Cancel everything
                return null;
            }

            var fine = new Fine
            {
                id = Guid.NewGuid(),
                userId = addFineRequestDto.userId,
                loanId = addFineRequestDto.loanId,
                amount = addFineRequestDto.amount,
                Loan = loan,
            };

            await dbContext.Fines.AddAsync(fine, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return fine;
        }

        public async Task<List<Fine>> getAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Fines.ToListAsync(cancellationToken);
        }

        public async Task<Fine?> getByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Fines.FirstOrDefaultAsync(x => x.id == id, cancellationToken);
        }

        public async Task<List<Fine?>> processOverdueLoansAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var loansToCheck = await dbContext.Loans
                .Where(l => l.userId == id).ToListAsync(cancellationToken);

            var processedFines = new List<Fine>();

            foreach (var loan in loansToCheck)
            {
                var isActuallyOverdue = loan.dueDate < DateOnly.FromDateTime(DateTime.Today)
                    && (loan.status == LoanStatus.borrowed || loan.status == LoanStatus.overdue);

                if (!isActuallyOverdue)
                    continue;

                // Mark the loan as overdue if it isn't already
                if (loan.status != LoanStatus.overdue)
                {
                    loan.status = LoanStatus.overdue;
                    dbContext.Loans.Update(loan);
                }

                var searchForFine = await dbContext.Fines.FirstOrDefaultAsync(f => f.loanId == loan.id, cancellationToken);

                if (searchForFine == null)
                {
                    // No fine yet — create one with the base amount
                    var fine = new Fine
                    {
                        id = Guid.NewGuid(),
                        userId = loan.userId,
                        loanId = loan.id,
                        amount = 5.0M,
                        Loan = loan,
                    };
                    processedFines.Add(fine);
                    await dbContext.Fines.AddAsync(fine, cancellationToken);
                }
                else
                {
                    // Fine already exists — update to correct total based on weeks overdue
                    var daysOverdue = DateOnly.FromDateTime(DateTime.Today).DayNumber - loan.dueDate.DayNumber;
                    var fullWeeksOverdue = daysOverdue / 7;

                    searchForFine.amount = 5.0M + (fullWeeksOverdue * 1.0M);
                    processedFines.Add(searchForFine);
                    dbContext.Fines.Update(searchForFine);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return processedFines;
        }

        public async Task<List<Fine>> getByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Fines
                .Include(f => f.Loan)
                    .ThenInclude(l => l.BookCopy)
                        .ThenInclude(bc => bc!.Book)
                .Where(f => f.userId == userId)
                .OrderByDescending(f => f.fineDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Fine?> payFineAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var fine = await dbContext.Fines
                    .Include(f => f.Loan)
                        .ThenInclude(l => l.BookCopy)
                            .ThenInclude(bc => bc!.Book)
                    .FirstOrDefaultAsync(f => f.id == id, cancellationToken);

                if (fine == null || fine.paid)
                    return null;

                // Mark the fine as paid
                fine.paid = true;
                dbContext.Fines.Update(fine);

                var loan = fine.Loan;

                // Mark the overdue loan as returned
                if (loan.status == LoanStatus.overdue || loan.status == LoanStatus.borrowed)
                {
                    loan.status = LoanStatus.returned;
                    dbContext.Loans.Update(loan);
                }

                // Free the book copy and restore available count
                var bookCopy = loan.BookCopy;
                if (bookCopy != null && bookCopy.status == CopyStatus.OnLoan)
                {
                    bookCopy.status = CopyStatus.Available;
                    dbContext.BookCopies.Update(bookCopy);

                    var book = bookCopy.Book;
                    if (book != null)
                    {
                        book.copiesAvailable += 1;
                        dbContext.Books.Update(book);
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return fine;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
