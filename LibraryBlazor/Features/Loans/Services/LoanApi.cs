using LibraryBlazor.Features.Books.Models;
using LibraryBlazor.Features.Loans.Models;
using LibraryBlazor.Http;
using LibraryBlazor.Shared.Models;

namespace LibraryBlazor.Features.Loans.Services
{
    public class LoanApi
    {
        private readonly ApiClient _api;

        public LoanApi(ApiClient api)
        {
            _api = api;
        }

        public Task<ApiResult<IReadOnlyList<LoanDto>>> GetLoansAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<LoanDto>>("api/loans", cancellationToken);

        public Task<ApiResult> ExtendLoanAsync(Guid loanId, ExtendLoanRequestDto body, CancellationToken cancellationToken = default)
            => _api.PutAsync($"api/loans/{loanId}/extend", new
            {
                dueDate = body.DueDate
            }, cancellationToken);
    }
}
