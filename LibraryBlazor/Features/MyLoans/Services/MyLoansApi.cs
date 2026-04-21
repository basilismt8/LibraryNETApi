using LibraryBlazor.Features.MyLoans.Models;
using LibraryBlazor.Http;
using LibraryBlazor.Shared.Models;

namespace LibraryBlazor.Features.MyLoans.Services
{
    public class MyLoansApi
    {
        private readonly ApiClient _api;

        public MyLoansApi(ApiClient api)
        {
            _api = api;
        }

        public Task<ApiResult<IReadOnlyList<MyLoanDto>>> GetMyLoansAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<MyLoanDto>>("api/loans/user/current", cancellationToken);

        public Task<ApiResult> ExtendLoanAsync(Guid loanId, ExtendLoanRequestDto body, CancellationToken cancellationToken = default)
            => _api.PutAsync($"api/loans/{loanId}/extend", new
            {
                dueDate = body.DueDate
            }, cancellationToken);
    }
}
