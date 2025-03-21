using Library.Api.Data;
using Library.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Repositories
{
    public class SQLFineRepository : IFineRepository
    {
        private readonly LibraryDbContext dbContext;

        public SQLFineRepository(LibraryDbContext dbContext )
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Fine>> getAllAsync()
        {
            return await dbContext.Fines.ToListAsync();
        }

        public async Task<Fine?> getByIdAsync(Guid id)
        {
            return await dbContext.Fines.FirstOrDefaultAsync(x => x.id == id);
        }
    }
}
