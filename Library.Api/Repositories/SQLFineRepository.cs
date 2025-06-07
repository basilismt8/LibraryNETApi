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

        public async Task<Fine?> addFineAsync(AddFineRequestDto addFineRequestDto)
        {


            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.id == addFineRequestDto.loanId);

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

            await dbContext.Fines.AddAsync(fine);

            await dbContext.SaveChangesAsync();
            return fine;
        }

        public async Task<List<Fine>> getAllAsync()
        {
            return await dbContext.Fines.ToListAsync();
        }

        public async Task<Fine?> getByIdAsync(Guid id)
        {
            return await dbContext.Fines.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<Fine?>> processOverdueLoansAsync(Guid id)
        {

            var loansToCheck = await dbContext.Loans
                .Where(l => l.userId == id).ToListAsync();

            var processedFines = new List<Fine>();

            foreach (var loan in loansToCheck)
            {
                var searchForFine = await dbContext.Fines.FirstOrDefaultAsync(f => f.loanId == loan.id);

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
                        await dbContext.Fines.AddAsync(fine);
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

            await dbContext.SaveChangesAsync();
            return processedFines;
        }
    }
}
