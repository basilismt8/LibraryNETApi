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
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var createdLoans = new List<Loan>();

                foreach (var bookId in createLoanRequestDto.bookIds)
                {
                    var book = await dbContext.Books.FirstOrDefaultAsync(b => b.id == bookId);

                    if (book == null || book.copiesAvailable <= 0)
                    {
                        // Cancel everything if even one book is invalid
                        await transaction.RollbackAsync();
                        return null;
                    }

                    // Find the first available BookCopy for this book
                    var availableCopy = await dbContext.BookCopies
                        .FirstOrDefaultAsync(bc => bc.bookId == bookId && bc.status == CopyStatus.Available);

                    if (availableCopy == null)
                    {
                        await transaction.RollbackAsync();
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

                    await dbContext.Loans.AddAsync(loan);

                    book.copiesAvailable -= 1;
                    dbContext.Books.Update(book);

                    createdLoans.Add(loan);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return createdLoans;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Loan>? extendLoanPeriodDomainAsync(Guid id, Loan loan)
        {
            var existingLoan = await dbContext.Loans.FirstOrDefaultAsync(x => x.id == id);
            if (existingLoan == null)
            {
                return null;
            }

            existingLoan.dueDate = loan.dueDate;

            dbContext.Loans.Update(existingLoan);

            await dbContext.SaveChangesAsync();
            return existingLoan;
        }

        public async Task<List<Loan>> getAllAsync()
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .ToListAsync();
        }

        public async Task<List<Loan>> getAllLoansByUserIdAsync(Guid userId)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .Where(x => x.userId == userId)
                .ToListAsync();
        }

        public async Task<Loan?> getByIdAsync(Guid id)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<Loan>> GetLoansByUserIdAsync(Guid userId)
        {
            return await dbContext.Loans
                .Include(l => l.BookCopy)
                    .ThenInclude(bc => bc!.Book)
                .Where(l => l.userId == userId)
                .ToListAsync();
        }
    }
}
