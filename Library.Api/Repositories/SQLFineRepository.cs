using Library.Api.Data;
using Library.Api.Models.Domain;
using Library.Api.Models.Dto;
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
                var searchForFine = await dbContext.Fines.FirstOrDefaultAsync(f => f.loanId == loan.id, cancellationToken);

                if (searchForFine == null && loan.status == LoanStatus.overdue && loan.dueDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    if (loan.status == LoanStatus.overdue && searchForFine == null)
                    {
                        // Create a fine for the overdue loan
                        var fine = new Fine
                        {
                            id = Guid.NewGuid(),
                            userId = loan.userId,
                            loanId = loan.id,
                            amount = 5.0M, // Example fine amount
                            Loan = loan,
                        };
                        processedFines.Add(fine);
                        await dbContext.Fines.AddAsync(fine, cancellationToken);
                    }
                }
                else if (searchForFine != null)
                {
                    var daysOverdue = DateOnly.FromDateTime(DateTime.Today).DayNumber - loan.dueDate.DayNumber;
                    var fullWeeksOverdue = daysOverdue / 7;

                    if (fullWeeksOverdue > 0)
                    {
                        searchForFine.amount += fullWeeksOverdue * 1.0M; // Add 1.0 for each full overdue week
                        processedFines.Add(searchForFine);
                        dbContext.Fines.Update(searchForFine);
                    }
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
    }
}
