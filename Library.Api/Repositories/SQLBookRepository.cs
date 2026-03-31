using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly LibraryDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailServiceInterface emailService;

        public SQLBookRepository(LibraryDbContext dbContext, UserManager<IdentityUser> userManager, IEmailServiceInterface emailService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        public async Task<Book> CreateAsync(Book book)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Add the book
                await dbContext.Books.AddAsync(book);
                await dbContext.SaveChangesAsync();

                // Create X BookCopy records based on totalCopies
                for (int i = 0; i < book.totalCopies; i++)
                {
                    var bookCopy = new BookCopy
                    {
                        id = Guid.NewGuid(),
                        bookId = book.id,
                        copyCode = GenerateUniqueCopyCode(book.id, i + 1),
                        status = CopyStatus.Available
                    };
                    await dbContext.BookCopies.AddAsync(bookCopy);
                }

                // Set copiesAvailable equal to totalCopies
                book.copiesAvailable = book.totalCopies;
                dbContext.Books.Update(book);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return book;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Book?> DeleteAsync(Guid id)
        {
            var existingBook = await dbContext.Books
                .Include(b => b.BookCopies)
                .FirstOrDefaultAsync(x => x.id == id);

            if (existingBook == null)
            {
                return null;
            }

            // Check if any copies are currently on loan
            var copiesOnLoan = await dbContext.BookCopies
                .Where(bc => bc.bookId == id && bc.status == CopyStatus.OnLoan)
                .ToListAsync();

            if (copiesOnLoan.Any())
            {
                // Get loans for copies on loan
                var copyIds = copiesOnLoan.Select(c => c.id).ToList();
                var loansWithBook = await dbContext.Loans
                    .Where(l => copyIds.Contains(l.bookCopyId) && (l.status == LoanStatus.borrowed || l.status == LoanStatus.overdue))
                    .ToListAsync();

                // Notify users
                var userIds = loansWithBook.Select(l => l.userId).Distinct().ToList();

                foreach (var userId in userIds)
                {
                    var user = await userManager.FindByIdAsync(userId.ToString());
                    if (user != null)
                    {
                        var email = user.Email;
                        await emailService.SendEmailAsync(email, "Book Deletion Attempt",
                            $"The book \"{existingBook.title}\" you have borrowed is scheduled for deletion from our library. Please return the book.");
                    }
                }

                return null; // Do not delete, just notify
            }

            // Cascade delete will handle BookCopies and their Loans
            dbContext.Books.Remove(existingBook);
            await dbContext.SaveChangesAsync();

            return existingBook;
        }

        public async Task<List<Book>> getAllAsync()
        {
            return await dbContext.Books.ToListAsync();
        }

        public async Task<Book?> getByIdAsync(Guid id)
        {
            return await dbContext.Books
                .Include(b => b.BookCopies)
                .FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<Loan?> BorrowBookAsync(Guid userId, Guid bookId, DateOnly dueDate)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Find the book
                var book = await dbContext.Books.FirstOrDefaultAsync(b => b.id == bookId);
                if (book == null || book.copiesAvailable <= 0)
                {
                    return null;
                }

                // Find the first available BookCopy for this book
                var availableCopy = await dbContext.BookCopies
                    .FirstOrDefaultAsync(bc => bc.bookId == bookId && bc.status == CopyStatus.Available);

                if (availableCopy == null)
                {
                    return null;
                }

                // Set BookCopy status to OnLoan
                availableCopy.status = CopyStatus.OnLoan;
                dbContext.BookCopies.Update(availableCopy);

                // Decrease Book.copiesAvailable by 1
                book.copiesAvailable -= 1;
                dbContext.Books.Update(book);

                // Create the Loan referencing bookCopyId
                var loan = new Loan
                {
                    id = Guid.NewGuid(),
                    bookCopyId = availableCopy.id,
                    userId = userId,
                    loanDate = DateOnly.FromDateTime(DateTime.Now),
                    dueDate = dueDate,
                    status = LoanStatus.borrowed
                };
                await dbContext.Loans.AddAsync(loan);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return loan;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Book?> ReturnBookAsync(Guid userId, Guid bookCopyId)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Find the BookCopy
                var bookCopy = await dbContext.BookCopies
                    .Include(bc => bc.Book)
                    .FirstOrDefaultAsync(bc => bc.id == bookCopyId);

                if (bookCopy == null || bookCopy.status != CopyStatus.OnLoan)
                {
                    return null;
                }

                // Find the active loan for this copy and user
                var loan = await dbContext.Loans
                    .FirstOrDefaultAsync(l => l.bookCopyId == bookCopyId && l.userId == userId && l.status == LoanStatus.borrowed);

                if (loan == null)
                {
                    return null;
                }

                // Set BookCopy status back to Available
                bookCopy.status = CopyStatus.Available;
                dbContext.BookCopies.Update(bookCopy);

                // Increase Book.copiesAvailable by 1
                var book = bookCopy.Book!;
                book.copiesAvailable += 1;
                dbContext.Books.Update(book);

                // Update Loan status to Returned
                loan.status = LoanStatus.returned;
                dbContext.Loans.Update(loan);

                // Mark any unpaid fine as paid
                var fine = await dbContext.Fines
                    .FirstOrDefaultAsync(f => f.userId == userId && f.loanId == loan.id && !f.paid);
                if (fine != null)
                {
                    fine.paid = true;
                    dbContext.Fines.Update(fine);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return book;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Book?> UpdateAsync(string id, Book book)
        {
            // Convert string → Guid
            if (!Guid.TryParse(id, out Guid guidId))
                return null;

            var existingBook = await dbContext.Books.FirstOrDefaultAsync(x => x.id == guidId);
            if (existingBook == null)
            {
                Console.WriteLine("this book does not exist");
                return null;
            }

            existingBook.title = book.title;
            existingBook.copiesAvailable = book.copiesAvailable;
            existingBook.totalCopies = book.totalCopies;

            await dbContext.SaveChangesAsync();
            return book;
        }

        private static string GenerateUniqueCopyCode(Guid bookId, int copyNumber)
        {
            // Generate a unique copy code: first 8 chars of bookId + sequence number + random suffix
            var bookIdPrefix = bookId.ToString("N")[..8].ToUpper();
            var randomSuffix = Guid.NewGuid().ToString("N")[..4].ToUpper();
            return $"{bookIdPrefix}-{copyNumber:D3}-{randomSuffix}";
        }
    }
}
