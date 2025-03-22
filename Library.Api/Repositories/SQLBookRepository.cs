using Library.Api.Data;
using Library.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly LibraryDbContext dbContext;

        public SQLBookRepository(LibraryDbContext dbContext)
        {
            this.dbContext = dbContext;
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
