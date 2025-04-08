using Library.Api.Data;
using Library.Api.Models.Domain;
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
            await dbContext.Books.AddAsync(book);
            await dbContext.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> DeleteAsync(Guid id)
        {
            var existingBook = await dbContext.Books.FirstOrDefaultAsync(x => x.id == id);

            if (existingBook == null)
            {
                return null;
            }

            var loansWithBook = await dbContext.Loans
                .Where(l => l.bookId == id && (l.status == LoanStatus.borrowed || l.status == LoanStatus.overdue))
                .ToListAsync();

            if (loansWithBook.Any())
            {
                // Here you fetch the user emails (from Auth DB)
                var userIds = loansWithBook.Select(l => l.userId).Distinct().ToList();

                // Assuming you injected UserManager<IdentityUser> and email sender (IEmailSender or similar)
                foreach (var userId in userIds)
                {
                    var user = await userManager.FindByIdAsync(userId.ToString());
                    if (user != null)
                    {
                        var email = user.Email;

                        // Send email logic here (could use IEmailSender or SmtpClient)
                        await emailService.SendEmailAsync(email, "Book Deletion Attempt",
                            $"The book \"{existingBook.title}\" you have borrowed is scheduled for deletion from our library. Pleas return the book");
                    }
                }

                return null; // Do not delete, just notify
            }

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
            return await dbContext.Books.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<Book?> UpdateAsync(Guid id, Book book)
        {
            var existingBook = await dbContext.Books.FirstOrDefaultAsync(x => x.id == id);
            if (existingBook == null) {
                return null;
            }

            existingBook.title = book.title;
            existingBook.copiesAvailable = book.copiesAvailable;
            existingBook.totalCopies = book.totalCopies;

            await dbContext.SaveChangesAsync();
            return book;
        }
    }
}
